// iPad质量检查系统 - 扫码功能模块
// 基于.NET MAUI技术栈

using Microsoft.Maui.Authentication.WebUI;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using System.Collections.ObjectModel;

namespace QualityCheckApp.Services
{
    /// <summary>
    /// 扫码服务接口
    /// </summary>
    public interface IBarcodeService
    {
        Task<BarcodeResult> StartScanAsync();
        Task<bool> ValidateBarcodeAsync(string barcode, BarcodeType type);
        BarcodeType DetectBarcodeType(string barcode);
    }

    /// <summary>
    /// 条码类型枚举
    /// </summary>
    public enum BarcodeType
    {
        ProductCD,      // 商品CD
        PalletNumber,   // 托盘号
        SerialNumber,   // 作番
        Unknown
    }

    /// <summary>
    /// 扫码结果类
    /// </summary>
    public class BarcodeResult
    {
        public string Value { get; set; }
        public BarcodeType Type { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ScanTime { get; set; }
    }

    /// <summary>
    /// 扫码服务实现
    /// </summary>
    public class BarcodeService : IBarcodeService
    {
        private readonly ILogger<BarcodeService> _logger;
        private readonly IDataService _dataService;
        
        public BarcodeService(ILogger<BarcodeService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        /// <summary>
        /// 启动扫码
        /// </summary>
        public async Task<BarcodeResult> StartScanAsync()
        {
            try
            {
                var scanPage = new BarcodeScanPage();
                var result = await Application.Current.MainPage.Navigation.PushModalAsync(scanPage);
                
                return await scanPage.GetScanResultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "扫码启动失败");
                return new BarcodeResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "扫码启动失败: " + ex.Message 
                };
            }
        }

        /// <summary>
        /// 验证条码
        /// </summary>
        public async Task<bool> ValidateBarcodeAsync(string barcode, BarcodeType type)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            try
            {
                switch (type)
                {
                    case BarcodeType.ProductCD:
                        return await ValidateProductCDAsync(barcode);
                    case BarcodeType.PalletNumber:
                        return await ValidatePalletNumberAsync(barcode);
                    case BarcodeType.SerialNumber:
                        return await ValidateSerialNumberAsync(barcode);
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "条码验证失败: {Barcode}", barcode);
                return false;
            }
        }

        /// <summary>
        /// 检测条码类型
        /// </summary>
        public BarcodeType DetectBarcodeType(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return BarcodeType.Unknown;

            // 商品CD：15位字符，以CH开头
            if (barcode.Length == 15 && barcode.StartsWith("CH"))
                return BarcodeType.ProductCD;

            // 托盘号：以TP开头，后跟8位数字
            if (barcode.Length == 10 && barcode.StartsWith("TP") && 
                int.TryParse(barcode.Substring(2), out _))
                return BarcodeType.PalletNumber;

            // 作番：10位数字
            if (barcode.Length == 10 && long.TryParse(barcode, out _))
                return BarcodeType.SerialNumber;

            return BarcodeType.Unknown;
        }

        private async Task<bool> ValidateProductCDAsync(string productCD)
        {
            // 调用API验证商品CD是否存在
            var product = await _dataService.GetProductByCodeAsync(productCD);
            return product != null;
        }

        private async Task<bool> ValidatePalletNumberAsync(string palletNumber)
        {
            // 验证托盘号格式和存在性
            var pallet = await _dataService.GetPalletByNumberAsync(palletNumber);
            return pallet != null;
        }

        private async Task<bool> ValidateSerialNumberAsync(string serialNumber)
        {
            // 验证作番
            var workOrder = await _dataService.GetWorkOrderBySerialAsync(serialNumber);
            return workOrder != null;
        }
    }
}

namespace QualityCheckApp.Views
{
    /// <summary>
    /// 扫码页面
    /// </summary>
    public partial class BarcodeScanPage : ContentPage
    {
        private readonly TaskCompletionSource<BarcodeResult> _scanResultTask;
        private CameraBarcodeReaderView _barcodeReader;
        private bool _isScanning = false;

        public BarcodeScanPage()
        {
            InitializeComponent();
            _scanResultTask = new TaskCompletionSource<BarcodeResult>();
            InitializeBarcodeReader();
        }

        private void InitializeBarcodeReader()
        {
            _barcodeReader = new CameraBarcodeReaderView
            {
                Options = new BarcodeReaderOptions
                {
                    Formats = BarcodeFormats.All,
                    AutoRotate = true,
                    Multiple = false
                }
            };

            _barcodeReader.BarcodesDetected += OnBarcodesDetected;
            
            // 添加到页面布局
            var grid = new Grid();
            grid.Children.Add(_barcodeReader);
            
            // 添加扫描框和提示
            var scanFrame = CreateScanFrame();
            var instructionLabel = new Label
            {
                Text = "请将条码对准扫描框",
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#80000000"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 0, 0, 100),
                Padding = new Thickness(20, 10)
            };

            grid.Children.Add(scanFrame);
            grid.Children.Add(instructionLabel);
            
            // 添加工具栏
            var toolbar = CreateToolbar();
            grid.Children.Add(toolbar);

            Content = grid;
        }

        private Frame CreateScanFrame()
        {
            var frame = new Frame
            {
                BorderColor = Colors.Red,
                BackgroundColor = Colors.Transparent,
                HasShadow = false,
                WidthRequest = 250,
                HeightRequest = 250,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0
            };

            return frame;
        }

        private StackLayout CreateToolbar()
        {
            var toolbar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 50, 0, 0),
                Children =
                {
                    new Button
                    {
                        Text = "关闭",
                        BackgroundColor = Color.FromArgb("#80000000"),
                        TextColor = Colors.White,
                        Command = new Command(CloseScanner)
                    },
                    new Button
                    {
                        Text = "手动输入",
                        BackgroundColor = Color.FromArgb("#80000000"),
                        TextColor = Colors.White,
                        Command = new Command(ShowManualInput)
                    }
                }
            };

            return toolbar;
        }

        private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (_isScanning || e.Results?.Length == 0)
                return;

            _isScanning = true;

            var barcode = e.Results[0];
            var barcodeService = DependencyService.Get<IBarcodeService>();
            
            var result = new BarcodeResult
            {
                Value = barcode.Value,
                Type = barcodeService.DetectBarcodeType(barcode.Value),
                ScanTime = DateTime.Now
            };

            // 验证条码
            result.IsValid = await barcodeService.ValidateBarcodeAsync(result.Value, result.Type);
            
            if (!result.IsValid)
            {
                result.ErrorMessage = "无效的条码或数据库中不存在该记录";
                
                // 显示错误提示并继续扫描
                await DisplayAlert("扫码失败", result.ErrorMessage, "继续扫描");
                _isScanning = false;
                return;
            }

            // 扫码成功，关闭页面并返回结果
            _scanResultTask.SetResult(result);
            await Navigation.PopModalAsync();
        }

        private async void CloseScanner()
        {
            var result = new BarcodeResult 
            { 
                IsValid = false, 
                ErrorMessage = "用户取消扫码" 
            };
            
            _scanResultTask.SetResult(result);
            await Navigation.PopModalAsync();
        }

        private async void ShowManualInput()
        {
            var input = await DisplayPromptAsync(
                "手动输入", 
                "请输入条码:", 
                placeholder: "扫描条码或手动输入");

            if (!string.IsNullOrWhiteSpace(input))
            {
                var barcodeService = DependencyService.Get<IBarcodeService>();
                var result = new BarcodeResult
                {
                    Value = input.Trim(),
                    Type = barcodeService.DetectBarcodeType(input.Trim()),
                    ScanTime = DateTime.Now
                };

                result.IsValid = await barcodeService.ValidateBarcodeAsync(result.Value, result.Type);
                
                if (!result.IsValid)
                {
                    result.ErrorMessage = "无效的条码";
                    await DisplayAlert("输入错误", result.ErrorMessage, "重试");
                    return;
                }

                _scanResultTask.SetResult(result);
                await Navigation.PopModalAsync();
            }
        }

        public Task<BarcodeResult> GetScanResultAsync()
        {
            return _scanResultTask.Task;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _barcodeReader?.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _barcodeReader?.Stop();
        }
    }
}

namespace QualityCheckApp.ViewModels
{
    /// <summary>
    /// 检查页面ViewModel - 集成扫码功能
    /// </summary>
    public class CheckPageViewModel : BaseViewModel
    {
        private readonly IBarcodeService _barcodeService;
        private readonly ICheckService _checkService;
        
        public CheckPageViewModel(IBarcodeService barcodeService, ICheckService checkService)
        {
            _barcodeService = barcodeService;
            _checkService = checkService;
            
            ScanProductCodeCommand = new Command(async () => await ScanProductCodeAsync());
            ScanPalletNumberCommand = new Command(async () => await ScanPalletNumberAsync());
        }

        private string _productCode;
        public string ProductCode
        {
            get => _productCode;
            set => SetProperty(ref _productCode, value);
        }

        private string _palletNumber;
        public string PalletNumber
        {
            get => _palletNumber;
            set => SetProperty(ref _palletNumber, value);
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        public Command ScanProductCodeCommand { get; }
        public Command ScanPalletNumberCommand { get; }

        /// <summary>
        /// 扫描商品CD
        /// </summary>
        private async Task ScanProductCodeAsync()
        {
            try
            {
                var result = await _barcodeService.StartScanAsync();
                
                if (result.IsValid)
                {
                    ProductCode = result.Value;
                    
                    // 自动加载商品信息
                    await LoadProductInfoAsync(result.Value);
                }
                else if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    await Application.Current.MainPage.DisplayAlert("扫码失败", result.ErrorMessage, "确定");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("错误", "扫码功能异常: " + ex.Message, "确定");
            }
        }

        /// <summary>
        /// 扫描托盘号
        /// </summary>
        private async Task ScanPalletNumberAsync()
        {
            try
            {
                var result = await _barcodeService.StartScanAsync();
                
                if (result.IsValid && result.Type == BarcodeType.PalletNumber)
                {
                    PalletNumber = result.Value;
                    
                    // 自动加载托盘检查列表
                    await LoadPalletCheckListAsync(result.Value);
                }
                else if (result.IsValid && result.Type != BarcodeType.PalletNumber)
                {
                    await Application.Current.MainPage.DisplayAlert("扫码错误", "请扫描正确的托盘条码", "确定");
                }
                else if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    await Application.Current.MainPage.DisplayAlert("扫码失败", result.ErrorMessage, "确定");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("错误", "扫码功能异常: " + ex.Message, "确定");
            }
        }

        private async Task LoadProductInfoAsync(string productCode)
        {
            // 实现商品信息加载逻辑
            var product = await _checkService.GetProductInfoAsync(productCode);
            // 更新UI...
        }

        private async Task LoadPalletCheckListAsync(string palletNumber)
        {
            // 实现托盘检查列表加载逻辑
            var checkItems = await _checkService.GetPalletCheckItemsAsync(palletNumber);
            // 更新UI...
        }
    }
}

// 平台特定实现 - iOS
#if IOS
using AVFoundation;
using UIKit;

namespace QualityCheckApp.Platforms.iOS
{
    public class BarcodePermissionService
    {
        public static async Task<bool> RequestCameraPermissionAsync()
        {
            var status = AVCaptureDevice.GetAuthorizationStatus(AVMediaTypes.Video);
            
            switch (status)
            {
                case AVAuthorizationStatus.Authorized:
                    return true;
                case AVAuthorizationStatus.NotDetermined:
                    var granted = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaTypes.Video);
                    return granted;
                default:
                    return false;
            }
        }
    }
}
#endif
