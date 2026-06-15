// iPad质量检查系统 - 蓝牙卡尺读取功能模块
// 基于.NET MAUI技术栈，支持三丰、马尔、广陆等主流品牌

using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Maui.Networking;

namespace QualityCheckApp.Services
{
    /// <summary>
    /// 蓝牙卡尺服务接口
    /// </summary>
    public interface IBluetoothCaliperService
    {
        Task<List<BluetoothDevice>> ScanDevicesAsync();
        Task<bool> ConnectAsync(BluetoothDevice device);
        Task<bool> DisconnectAsync();
        Task<MeasurementResult> ReadMeasurementAsync();
        bool IsConnected { get; }
        BluetoothDevice ConnectedDevice { get; }
        event EventHandler<MeasurementResult> MeasurementReceived;
        event EventHandler<BluetoothConnectionEventArgs> ConnectionStatusChanged;
    }

    /// <summary>
    /// 蓝牙设备信息
    /// </summary>
    public class BluetoothDevice
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public CaliperBrand Brand { get; set; }
        public int SignalStrength { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastSeen { get; set; }
    }

    /// <summary>
    /// 卡尺品牌枚举
    /// </summary>
    public enum CaliperBrand
    {
        Unknown,
        Mitutoyo,   // 三丰
        Mahr,       // 马尔
        Guanglu,    // 广陆
        Starrett,   // 施泰力
        Fowler
    }

    /// <summary>
    /// 测量结果
    /// </summary>
    public class MeasurementResult
    {
        public double Value { get; set; }
        public MeasurementUnit Unit { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public CaliperBrand DeviceBrand { get; set; }
        public string DeviceId { get; set; }
        public BatteryLevel BatteryLevel { get; set; }
    }

    /// <summary>
    /// 测量单位
    /// </summary>
    public enum MeasurementUnit
    {
        Millimeter,
        Inch
    }

    /// <summary>
    /// 电池电量
    /// </summary>
    public enum BatteryLevel
    {
        Unknown,
        Critical,   // 低电量
        Low,        // 电量不足
        Medium,     // 电量适中
        High,       // 电量充足
        Full        // 电量满
    }

    /// <summary>
    /// 蓝牙连接事件参数
    /// </summary>
    public class BluetoothConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public BluetoothDevice Device { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// 蓝牙卡尺服务实现
    /// </summary>
    public class BluetoothCaliperService : IBluetoothCaliperService, IDisposable
    {
        private readonly ILogger<BluetoothCaliperService> _logger;
        private readonly IBluetoothAdapter _bluetoothAdapter;
        private readonly Dictionary<CaliperBrand, ICaliperProtocol> _protocols;
        
        private BluetoothDevice _connectedDevice;
        private ICaliperProtocol _currentProtocol;
        private CancellationTokenSource _scanCancellationTokenSource;
        private bool _isScanning = false;

        public BluetoothCaliperService(
            ILogger<BluetoothCaliperService> logger,
            IBluetoothAdapter bluetoothAdapter)
        {
            _logger = logger;
            _bluetoothAdapter = bluetoothAdapter;
            _protocols = InitializeProtocols();
        }

        public bool IsConnected => _connectedDevice != null;
        public BluetoothDevice ConnectedDevice => _connectedDevice;

        public event EventHandler<MeasurementResult> MeasurementReceived;
        public event EventHandler<BluetoothConnectionEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// 扫描蓝牙卡尺设备
        /// </summary>
        public async Task<List<BluetoothDevice>> ScanDevicesAsync()
        {
            var devices = new List<BluetoothDevice>();
            
            try
            {
                _isScanning = true;
                _scanCancellationTokenSource = new CancellationTokenSource();
                
                _logger.LogInformation("开始扫描蓝牙卡尺设备");

                // 检查蓝牙权限
                if (!await _bluetoothAdapter.IsEnabledAsync())
                {
                    throw new InvalidOperationException("蓝牙未启用，请开启蓝牙后重试");
                }

                // 扫描已配对的设备
                var pairedDevices = await _bluetoothAdapter.GetPairedDevicesAsync();
                foreach (var device in pairedDevices)
                {
                    var caliperDevice = CreateCaliperDevice(device);
                    if (caliperDevice != null)
                    {
                        devices.Add(caliperDevice);
                    }
                }

                // 扫描新设备
                await _bluetoothAdapter.StartScanAsync(_scanCancellationTokenSource.Token);
                
                // 监听扫描结果
                _bluetoothAdapter.DeviceDiscovered += (sender, device) =>
                {
                    var caliperDevice = CreateCaliperDevice(device);
                    if (caliperDevice != null && !devices.Any(d => d.Address == caliperDevice.Address))
                    {
                        devices.Add(caliperDevice);
                    }
                };

                // 扫描30秒
                await Task.Delay(30000, _scanCancellationTokenSource.Token);
                
                await _bluetoothAdapter.StopScanAsync();
                
                _logger.LogInformation("扫描完成，发现 {Count} 个卡尺设备", devices.Count);
                
                return devices.OrderByDescending(d => d.SignalStrength).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "扫描蓝牙设备失败");
                throw;
            }
            finally
            {
                _isScanning = false;
                _scanCancellationTokenSource?.Dispose();
            }
        }

        /// <summary>
        /// 连接蓝牙卡尺
        /// </summary>
        public async Task<bool> ConnectAsync(BluetoothDevice device)
        {
            try
            {
                _logger.LogInformation("连接设备: {DeviceName} ({DeviceAddress})", device.Name, device.Address);

                // 断开当前连接
                if (IsConnected)
                {
                    await DisconnectAsync();
                }

                // 连接设备
                var connected = await _bluetoothAdapter.ConnectAsync(device.Address);
                if (!connected)
                {
                    throw new InvalidOperationException("无法连接到设备");
                }

                // 选择协议
                _currentProtocol = _protocols[device.Brand];
                await _currentProtocol.InitializeAsync(device);

                // 启动数据监听
                _currentProtocol.DataReceived += OnDataReceived;
                await _currentProtocol.StartListeningAsync();

                _connectedDevice = device;
                _connectedDevice.IsConnected = true;

                // 触发连接成功事件
                ConnectionStatusChanged?.Invoke(this, new BluetoothConnectionEventArgs
                {
                    IsConnected = true,
                    Device = device
                });

                _logger.LogInformation("设备连接成功: {DeviceName}", device.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "连接设备失败: {DeviceName}", device.Name);
                
                ConnectionStatusChanged?.Invoke(this, new BluetoothConnectionEventArgs
                {
                    IsConnected = false,
                    Device = device,
                    ErrorMessage = ex.Message
                });

                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public async Task<bool> DisconnectAsync()
        {
            try
            {
                if (_currentProtocol != null)
                {
                    _currentProtocol.DataReceived -= OnDataReceived;
                    await _currentProtocol.StopListeningAsync();
                }

                if (_connectedDevice != null)
                {
                    await _bluetoothAdapter.DisconnectAsync(_connectedDevice.Address);
                    _connectedDevice.IsConnected = false;
                    
                    ConnectionStatusChanged?.Invoke(this, new BluetoothConnectionEventArgs
                    {
                        IsConnected = false,
                        Device = _connectedDevice
                    });
                }

                _connectedDevice = null;
                _currentProtocol = null;
                
                _logger.LogInformation("设备断开连接");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "断开连接失败");
                return false;
            }
        }

        /// <summary>
        /// 读取测量值
        /// </summary>
        public async Task<MeasurementResult> ReadMeasurementAsync()
        {
            if (!IsConnected)
            {
                return new MeasurementResult
                {
                    IsValid = false,
                    ErrorMessage = "设备未连接"
                };
            }

            try
            {
                return await _currentProtocol.RequestMeasurementAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取测量值失败");
                return new MeasurementResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private void OnDataReceived(object sender, MeasurementResult measurement)
        {
            measurement.DeviceId = _connectedDevice?.Id;
            measurement.DeviceBrand = _connectedDevice?.Brand ?? CaliperBrand.Unknown;
            measurement.Timestamp = DateTime.Now;

            MeasurementReceived?.Invoke(this, measurement);
        }

        private BluetoothDevice CreateCaliperDevice(IBluetoothDevice device)
        {
            var brand = DetectCaliperBrand(device.Name);
            if (brand == CaliperBrand.Unknown)
                return null;

            return new BluetoothDevice
            {
                Id = device.Id,
                Name = device.Name,
                Address = device.Address,
                Brand = brand,
                SignalStrength = device.SignalStrength,
                LastSeen = DateTime.Now
            };
        }

        private CaliperBrand DetectCaliperBrand(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                return CaliperBrand.Unknown;

            var name = deviceName.ToUpperInvariant();

            if (name.Contains("MITUTOYO") || name.Contains("三丰"))
                return CaliperBrand.Mitutoyo;
            
            if (name.Contains("MAHR") || name.Contains("马尔"))
                return CaliperBrand.Mahr;
            
            if (name.Contains("GUANGLU") || name.Contains("广陆"))
                return CaliperBrand.Guanglu;
            
            if (name.Contains("STARRETT"))
                return CaliperBrand.Starrett;
            
            if (name.Contains("FOWLER"))
                return CaliperBrand.Fowler;

            return CaliperBrand.Unknown;
        }

        private Dictionary<CaliperBrand, ICaliperProtocol> InitializeProtocols()
        {
            return new Dictionary<CaliperBrand, ICaliperProtocol>
            {
                [CaliperBrand.Mitutoyo] = new MitutoyoProtocol(),
                [CaliperBrand.Mahr] = new MahrProtocol(),
                [CaliperBrand.Guanglu] = new GuangluProtocol(),
                [CaliperBrand.Starrett] = new StarrettProtocol(),
                [CaliperBrand.Fowler] = new FowlerProtocol()
            };
        }

        public void Dispose()
        {
            DisconnectAsync().Wait();
            _scanCancellationTokenSource?.Dispose();
        }
    }
}

namespace QualityCheckApp.Protocols
{
    /// <summary>
    /// 卡尺通信协议接口
    /// </summary>
    public interface ICaliperProtocol
    {
        Task InitializeAsync(BluetoothDevice device);
        Task StartListeningAsync();
        Task StopListeningAsync();
        Task<MeasurementResult> RequestMeasurementAsync();
        event EventHandler<MeasurementResult> DataReceived;
    }

    /// <summary>
    /// 三丰卡尺协议实现
    /// </summary>
    public class MitutoyoProtocol : ICaliperProtocol
    {
        private readonly ILogger<MitutoyoProtocol> _logger;
        private IBluetoothSocket _socket;
        private bool _isListening;

        public event EventHandler<MeasurementResult> DataReceived;

        public MitutoyoProtocol()
        {
            _logger = DependencyService.Get<ILogger<MitutoyoProtocol>>();
        }

        public async Task InitializeAsync(BluetoothDevice device)
        {
            _socket = await BluetoothSocket.ConnectAsync(device.Address, MitutoyoConstants.ServiceUuid);
        }

        public async Task StartListeningAsync()
        {
            _isListening = true;
            _ = Task.Run(async () =>
            {
                while (_isListening)
                {
                    try
                    {
                        var data = await _socket.ReadAsync();
                        if (data?.Length > 0)
                        {
                            var measurement = ParseMitutoyoData(data);
                            if (measurement.IsValid)
                            {
                                DataReceived?.Invoke(this, measurement);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "三丰卡尺数据读取失败");
                        await Task.Delay(1000); // 重试延迟
                    }
                }
            });
        }

        public async Task StopListeningAsync()
        {
            _isListening = false;
            await _socket?.CloseAsync();
        }

        public async Task<MeasurementResult> RequestMeasurementAsync()
        {
            try
            {
                // 发送数据请求命令
                await _socket.WriteAsync(MitutoyoConstants.DataRequestCommand);
                
                // 等待响应
                var data = await _socket.ReadAsync(timeout: 5000);
                return ParseMitutoyoData(data);
            }
            catch (Exception ex)
            {
                return new MeasurementResult
                {
                    IsValid = false,
                    ErrorMessage = $"三丰卡尺数据请求失败: {ex.Message}"
                };
            }
        }

        private MeasurementResult ParseMitutoyoData(byte[] data)
        {
            try
            {
                // 三丰卡尺数据格式解析
                var dataString = Encoding.ASCII.GetString(data).Trim();
                
                // 示例格式: "+00012.345mm"
                if (dataString.Length < 10)
                {
                    return new MeasurementResult { IsValid = false, ErrorMessage = "数据格式错误" };
                }

                var sign = dataString[0] == '+' ? 1 : -1;
                var valueStr = dataString.Substring(1, dataString.Length - 3);
                var unitStr = dataString.Substring(dataString.Length - 2);

                if (!double.TryParse(valueStr, out var value))
                {
                    return new MeasurementResult { IsValid = false, ErrorMessage = "数值解析失败" };
                }

                var unit = unitStr.ToLowerInvariant() == "mm" ? MeasurementUnit.Millimeter : MeasurementUnit.Inch;

                return new MeasurementResult
                {
                    Value = value * sign,
                    Unit = unit,
                    IsValid = true,
                    BatteryLevel = ExtractBatteryLevel(data)
                };
            }
            catch (Exception ex)
            {
                return new MeasurementResult
                {
                    IsValid = false,
                    ErrorMessage = $"数据解析错误: {ex.Message}"
                };
            }
        }

        private BatteryLevel ExtractBatteryLevel(byte[] data)
        {
            // 从数据包中提取电池电量信息
            if (data.Length > 15)
            {
                var batteryByte = data[15];
                return batteryByte switch
                {
                    >= 80 => BatteryLevel.Full,
                    >= 60 => BatteryLevel.High,
                    >= 40 => BatteryLevel.Medium,
                    >= 20 => BatteryLevel.Low,
                    _ => BatteryLevel.Critical
                };
            }
            return BatteryLevel.Unknown;
        }
    }

    /// <summary>
    /// 马尔卡尺协议实现
    /// </summary>
    public class MahrProtocol : ICaliperProtocol
    {
        private IBluetoothSocket _socket;
        private bool _isListening;

        public event EventHandler<MeasurementResult> DataReceived;

        public async Task InitializeAsync(BluetoothDevice device)
        {
            _socket = await BluetoothSocket.ConnectAsync(device.Address, MahrConstants.ServiceUuid);
        }

        public async Task StartListeningAsync()
        {
            _isListening = true;
            _ = Task.Run(async () =>
            {
                while (_isListening)
                {
                    try
                    {
                        var data = await _socket.ReadAsync();
                        if (data?.Length > 0)
                        {
                            var measurement = ParseMahrData(data);
                            if (measurement.IsValid)
                            {
                                DataReceived?.Invoke(this, measurement);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(1000);
                    }
                }
            });
        }

        public async Task StopListeningAsync()
        {
            _isListening = false;
            await _socket?.CloseAsync();
        }

        public async Task<MeasurementResult> RequestMeasurementAsync()
        {
            try
            {
                await _socket.WriteAsync(MahrConstants.DataRequestCommand);
                var data = await _socket.ReadAsync(timeout: 5000);
                return ParseMahrData(data);
            }
            catch (Exception ex)
            {
                return new MeasurementResult
                {
                    IsValid = false,
                    ErrorMessage = $"马尔卡尺数据请求失败: {ex.Message}"
                };
            }
        }

        private MeasurementResult ParseMahrData(byte[] data)
        {
            // 马尔卡尺特定的数据格式解析
            // 实现类似三丰的解析逻辑，但适配马尔的数据格式
            return new MeasurementResult { IsValid = false, ErrorMessage = "马尔协议解析待实现" };
        }
    }

    /// <summary>
    /// 广陆卡尺协议实现
    /// </summary>
    public class GuangluProtocol : ICaliperProtocol
    {
        private IBluetoothSocket _socket;
        private bool _isListening;

        public event EventHandler<MeasurementResult> DataReceived;

        public async Task InitializeAsync(BluetoothDevice device)
        {
            _socket = await BluetoothSocket.ConnectAsync(device.Address, GuangluConstants.ServiceUuid);
        }

        public async Task StartListeningAsync()
        {
            _isListening = true;
            _ = Task.Run(async () =>
            {
                while (_isListening)
                {
                    try
                    {
                        var data = await _socket.ReadAsync();
                        if (data?.Length > 0)
                        {
                            var measurement = ParseGuangluData(data);
                            if (measurement.IsValid)
                            {
                                DataReceived?.Invoke(this, measurement);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(1000);
                    }
                }
            });
        }

        public async Task StopListeningAsync()
        {
            _isListening = false;
            await _socket?.CloseAsync();
        }

        public async Task<MeasurementResult> RequestMeasurementAsync()
        {
            try
            {
                await _socket.WriteAsync(GuangluConstants.DataRequestCommand);
                var data = await _socket.ReadAsync(timeout: 5000);
                return ParseGuangluData(data);
            }
            catch (Exception ex)
            {
                return new MeasurementResult
                {
                    IsValid = false,
                    ErrorMessage = $"广陆卡尺数据请求失败: {ex.Message}"
                };
            }
        }

        private MeasurementResult ParseGuangluData(byte[] data)
        {
            // 广陆卡尺特定的数据格式解析
            return new MeasurementResult { IsValid = false, ErrorMessage = "广陆协议解析待实现" };
        }
    }

    // 其他品牌协议实现...
    public class StarrettProtocol : ICaliperProtocol
    {
        public event EventHandler<MeasurementResult> DataReceived;
        public Task InitializeAsync(BluetoothDevice device) => Task.CompletedTask;
        public Task StartListeningAsync() => Task.CompletedTask;
        public Task StopListeningAsync() => Task.CompletedTask;
        public Task<MeasurementResult> RequestMeasurementAsync() => 
            Task.FromResult(new MeasurementResult { IsValid = false, ErrorMessage = "施泰力协议待实现" });
    }

    public class FowlerProtocol : ICaliperProtocol
    {
        public event EventHandler<MeasurementResult> DataReceived;
        public Task InitializeAsync(BluetoothDevice device) => Task.CompletedTask;
        public Task StartListeningAsync() => Task.CompletedTask;
        public Task StopListeningAsync() => Task.CompletedTask;
        public Task<MeasurementResult> RequestMeasurementAsync() => 
            Task.FromResult(new MeasurementResult { IsValid = false, ErrorMessage = "Fowler协议待实现" });
    }
}

namespace QualityCheckApp.Constants
{
    /// <summary>
    /// 三丰卡尺常量
    /// </summary>
    public static class MitutoyoConstants
    {
        public static readonly Guid ServiceUuid = Guid.Parse("00001101-0000-1000-8000-00805F9B34FB");
        public static readonly byte[] DataRequestCommand = { 0x44, 0x41, 0x54, 0x41 }; // "DATA"
    }

    /// <summary>
    /// 马尔卡尺常量
    /// </summary>
    public static class MahrConstants
    {
        public static readonly Guid ServiceUuid = Guid.Parse("00001101-0000-1000-8000-00805F9B34FB");
        public static readonly byte[] DataRequestCommand = { 0x52, 0x45, 0x41, 0x44 }; // "READ"
    }

    /// <summary>
    /// 广陆卡尺常量
    /// </summary>
    public static class GuangluConstants
    {
        public static readonly Guid ServiceUuid = Guid.Parse("00001101-0000-1000-8000-00805F9B34FB");
        public static readonly byte[] DataRequestCommand = { 0x47, 0x45, 0x54 }; // "GET"
    }
}

namespace QualityCheckApp.ViewModels
{
    /// <summary>
    /// 蓝牙卡尺管理ViewModel
    /// </summary>
    public class BluetoothCaliperViewModel : BaseViewModel
    {
        private readonly IBluetoothCaliperService _bluetoothService;
        private readonly ICheckService _checkService;

        public BluetoothCaliperViewModel(
            IBluetoothCaliperService bluetoothService,
            ICheckService checkService)
        {
            _bluetoothService = bluetoothService;
            _checkService = checkService;

            AvailableDevices = new ObservableCollection<BluetoothDevice>();
            MeasurementHistory = new ObservableCollection<MeasurementResult>();

            ScanDevicesCommand = new Command(async () => await ScanDevicesAsync(), () => !IsScanning);
            ConnectDeviceCommand = new Command<BluetoothDevice>(async (device) => await ConnectDeviceAsync(device));
            DisconnectCommand = new Command(async () => await DisconnectAsync());
            ReadMeasurementCommand = new Command(async () => await ReadMeasurementAsync());
            ClearHistoryCommand = new Command(() => MeasurementHistory.Clear());

            // 订阅事件
            _bluetoothService.MeasurementReceived += OnMeasurementReceived;
            _bluetoothService.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        public ObservableCollection<BluetoothDevice> AvailableDevices { get; }
        public ObservableCollection<MeasurementResult> MeasurementHistory { get; }

        private bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                SetProperty(ref _isScanning, value);
                ScanDevicesCommand.ChangeCanExecute();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private BluetoothDevice _selectedDevice;
        public BluetoothDevice SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        private MeasurementResult _currentMeasurement;
        public MeasurementResult CurrentMeasurement
        {
            get => _currentMeasurement;
            set => SetProperty(ref _currentMeasurement, value);
        }

        public Command ScanDevicesCommand { get; }
        public Command<BluetoothDevice> ConnectDeviceCommand { get; }
        public Command DisconnectCommand { get; }
        public Command ReadMeasurementCommand { get; }
        public Command ClearHistoryCommand { get; }

        private async Task ScanDevicesAsync()
        {
            try
            {
                IsScanning = true;
                AvailableDevices.Clear();

                var devices = await _bluetoothService.ScanDevicesAsync();
                foreach (var device in devices)
                {
                    AvailableDevices.Add(device);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("扫描失败", ex.Message, "确定");
            }
            finally
            {
                IsScanning = false;
            }
        }

        private async Task ConnectDeviceAsync(BluetoothDevice device)
        {
            try
            {
                if (device == null) return;

                var connected = await _bluetoothService.ConnectAsync(device);
                if (connected)
                {
                    SelectedDevice = device;
                    IsConnected = true;
                    await Application.Current.MainPage.DisplayAlert("连接成功", $"已连接到 {device.Name}", "确定");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("连接失败", "无法连接到设备", "确定");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("连接错误", ex.Message, "确定");
            }
        }

        private async Task DisconnectAsync()
        {
            try
            {
                await _bluetoothService.DisconnectAsync();
                SelectedDevice = null;
                IsConnected = false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("断开连接失败", ex.Message, "确定");
            }
        }

        private async Task ReadMeasurementAsync()
        {
            try
            {
                var measurement = await _bluetoothService.ReadMeasurementAsync();
                if (measurement.IsValid)
                {
                    CurrentMeasurement = measurement;
                    MeasurementHistory.Insert(0, measurement);
                    
                    // 如果在检查页面，自动填入测量值
                    await AutoFillMeasurementAsync(measurement);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("读取失败", measurement.ErrorMessage, "确定");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("读取错误", ex.Message, "确定");
            }
        }

        private void OnMeasurementReceived(object sender, MeasurementResult measurement)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentMeasurement = measurement;
                MeasurementHistory.Insert(0, measurement);
                
                // 自动填入检查项目
                _ = AutoFillMeasurementAsync(measurement);
            });
        }

        private void OnConnectionStatusChanged(object sender, BluetoothConnectionEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsConnected = e.IsConnected;
                if (e.IsConnected)
                {
                    SelectedDevice = e.Device;
                }
                else
                {
                    SelectedDevice = null;
                }
            });
        }

        private async Task AutoFillMeasurementAsync(MeasurementResult measurement)
        {
            try
            {
                // 获取当前检查项目上下文
                var currentCheckItem = await _checkService.GetCurrentCheckItemAsync();
                if (currentCheckItem != null)
                {
                    // 自动填入测量值
                    await _checkService.FillMeasurementValueAsync(currentCheckItem.Id, measurement.Value, measurement.Unit);
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不影响正常流程
                System.Diagnostics.Debug.WriteLine($"自动填入测量值失败: {ex.Message}");
            }
        }
    }
}

// 平台特定实现 - iOS
#if IOS
using CoreBluetooth;
using Foundation;

namespace QualityCheckApp.Platforms.iOS
{
    public class BluetoothPermissionService
    {
        public static async Task<bool> RequestBluetoothPermissionAsync()
        {
            var manager = new CBCentralManager();
            
            // 等待蓝牙状态更新
            var tcs = new TaskCompletionSource<bool>();
            
            manager.UpdatedState += (sender, e) =>
            {
                switch (manager.State)
                {
                    case CBManagerState.PoweredOn:
                        tcs.SetResult(true);
                        break;
                    case CBManagerState.PoweredOff:
                    case CBManagerState.Unauthorized:
                    case CBManagerState.Unsupported:
                        tcs.SetResult(false);
                        break;
                }
            };

            return await tcs.Task;
        }
    }
}
#endif

// 用法示例页面
namespace QualityCheckApp.Views
{
    /// <summary>
    /// 蓝牙卡尺管理页面
    /// </summary>
    public partial class BluetoothCaliperPage : ContentPage
    {
        private readonly BluetoothCaliperViewModel _viewModel;

        public BluetoothCaliperPage(BluetoothCaliperViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private void InitializeComponent()
        {
            Title = "蓝牙卡尺管理";

            var scrollView = new ScrollView();
            var stackLayout = new StackLayout { Padding = 20 };

            // 设备扫描区域
            var deviceSection = CreateDeviceSection();
            stackLayout.Children.Add(deviceSection);

            // 测量区域
            var measurementSection = CreateMeasurementSection();
            stackLayout.Children.Add(measurementSection);

            // 历史记录区域
            var historySection = CreateHistorySection();
            stackLayout.Children.Add(historySection);

            scrollView.Content = stackLayout;
            Content = scrollView;
        }

        private StackLayout CreateDeviceSection()
        {
            var section = new StackLayout();
            
            var header = new Label
            {
                Text = "蓝牙设备管理",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var scanButton = new Button
            {
                Text = "扫描设备",
                Command = _viewModel.ScanDevicesCommand
            };

            var deviceList = new CollectionView
            {
                ItemsSource = _viewModel.AvailableDevices,
                HeightRequest = 200,
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(100) }
                        }
                    };

                    var nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Name");
                    Grid.SetColumn(nameLabel, 0);

                    var connectButton = new Button
                    {
                        Text = "连接",
                        Command = _viewModel.ConnectDeviceCommand
                    };
                    connectButton.SetBinding(Button.CommandParameterProperty, ".");
                    Grid.SetColumn(connectButton, 1);

                    grid.Children.Add(nameLabel);
                    grid.Children.Add(connectButton);

                    return new ViewCell { View = grid };
                })
            };

            section.Children.Add(header);
            section.Children.Add(scanButton);
            section.Children.Add(deviceList);

            return section;
        }

        private StackLayout CreateMeasurementSection()
        {
            var section = new StackLayout();

            var header = new Label
            {
                Text = "测量控制",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };

            var statusLabel = new Label();
            statusLabel.SetBinding(Label.TextProperty, new Binding("IsConnected", 
                converter: new BoolToStringConverter { TrueValue = "已连接", FalseValue = "未连接" }));

            var currentValueLabel = new Label
            {
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };
            currentValueLabel.SetBinding(Label.TextProperty, "CurrentMeasurement.Value");

            var readButton = new Button
            {
                Text = "读取测量值",
                Command = _viewModel.ReadMeasurementCommand
            };

            var disconnectButton = new Button
            {
                Text = "断开连接",
                Command = _viewModel.DisconnectCommand
            };

            section.Children.Add(header);
            section.Children.Add(statusLabel);
            section.Children.Add(currentValueLabel);
            section.Children.Add(readButton);
            section.Children.Add(disconnectButton);

            return section;
        }

        private StackLayout CreateHistorySection()
        {
            var section = new StackLayout();

            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(80) }
                }
            };

            var header = new Label
            {
                Text = "测量历史",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            };
            Grid.SetColumn(header, 0);

            var clearButton = new Button
            {
                Text = "清空",
                Command = _viewModel.ClearHistoryCommand
            };
            Grid.SetColumn(clearButton, 1);

            headerGrid.Children.Add(header);
            headerGrid.Children.Add(clearButton);

            var historyList = new CollectionView
            {
                ItemsSource = _viewModel.MeasurementHistory,
                HeightRequest = 300,
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(100) },
                            new ColumnDefinition { Width = new GridLength(80) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                        }
                    };

                    var valueLabel = new Label();
                    valueLabel.SetBinding(Label.TextProperty, "Value");
                    Grid.SetColumn(valueLabel, 0);

                    var unitLabel = new Label();
                    unitLabel.SetBinding(Label.TextProperty, "Unit");
                    Grid.SetColumn(unitLabel, 1);

                    var timeLabel = new Label { FontSize = 12 };
                    timeLabel.SetBinding(Label.TextProperty, new Binding("Timestamp", 
                        stringFormat: "{0:HH:mm:ss}"));
                    Grid.SetColumn(timeLabel, 2);

                    grid.Children.Add(valueLabel);
                    grid.Children.Add(unitLabel);
                    grid.Children.Add(timeLabel);

                    return new ViewCell { View = grid };
                })
            };

            section.Children.Add(headerGrid);
            section.Children.Add(historyList);

            return section;
        }
    }

    /// <summary>
    /// 布尔值转字符串转换器
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueValue : FalseValue;
            }
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
