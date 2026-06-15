// iPad质量检查系统 - 语音识别功能模块
// 基于.NET MAUI技术栈，集成iOS语音识别能力

using Microsoft.CognitiveServices.Speech;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace QualityCheckApp.Services
{
    /// <summary>
    /// 语音识别服务接口
    /// </summary>
    public interface ISpeechRecognitionService
    {
        Task<SpeechRecognitionResult> StartRecognitionAsync(SpeechRecognitionOptions options = null);
        Task<bool> StartContinuousRecognitionAsync();
        Task StopContinuousRecognitionAsync();
        bool IsRecognizing { get; }
        event EventHandler<SpeechRecognitionResult> SpeechRecognized;
        event EventHandler<string> RecognitionError;
    }

    /// <summary>
    /// 语音识别选项
    /// </summary>
    public class SpeechRecognitionOptions
    {
        public string Language { get; set; } = "zh-CN"; // 默认中文
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public bool EnablePartialResults { get; set; } = true;
        public SpeechRecognitionMode Mode { get; set; } = SpeechRecognitionMode.Interactive;
        public List<string> Keywords { get; set; } = new(); // 关键词列表
    }

    /// <summary>
    /// 语音识别模式
    /// </summary>
    public enum SpeechRecognitionMode
    {
        Interactive,    // 交互模式（短语音）
        Dictation,      // 听写模式（长语音）
        Conversation    // 对话模式
    }

    /// <summary>
    /// 语音识别结果
    /// </summary>
    public class SpeechRecognitionResult
    {
        public string Text { get; set; }
        public double Confidence { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime Timestamp { get; set; }
        public RecognizedIntent Intent { get; set; }
    }

    /// <summary>
    /// 识别意图
    /// </summary>
    public class RecognizedIntent
    {
        public string Action { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
        public double Confidence { get; set; }
    }

    /// <summary>
    /// 语音识别服务实现
    /// </summary>
    public class SpeechRecognitionService : ISpeechRecognitionService, IDisposable
    {
        private readonly ILogger<SpeechRecognitionService> _logger;
        private readonly SpeechConfig _speechConfig;
        private SpeechRecognizer _recognizer;
        private bool _isRecognizing = false;
        private TaskCompletionSource<SpeechRecognitionResult> _recognitionTask;

        public bool IsRecognizing => _isRecognizing;

        public event EventHandler<SpeechRecognitionResult> SpeechRecognized;
        public event EventHandler<string> RecognitionError;

        public SpeechRecognitionService(ILogger<SpeechRecognitionService> logger)
        {
            _logger = logger;
            
            // 初始化Azure语音服务配置
            _speechConfig = SpeechConfig.FromSubscription(
                "YOUR_SPEECH_KEY", 
                "YOUR_SPEECH_REGION");
            
            _speechConfig.SpeechRecognitionLanguage = "zh-CN";
        }

        /// <summary>
        /// 开始单次语音识别
        /// </summary>
        public async Task<SpeechRecognitionResult> StartRecognitionAsync(SpeechRecognitionOptions options = null)
        {
            try
            {
                options ??= new SpeechRecognitionOptions();
                
                _logger.LogInformation("开始语音识别，语言: {Language}", options.Language);

                // 检查麦克风权限
                if (!await CheckMicrophonePermissionAsync())
                {
                    return new SpeechRecognitionResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "麦克风权限未授权"
                    };
                }

                // 配置识别器
                _speechConfig.SpeechRecognitionLanguage = options.Language;
                using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

                // 设置关键词识别
                if (options.Keywords?.Any() == true)
                {
                    await SetupKeywordRecognitionAsync(recognizer, options.Keywords);
                }

                _recognitionTask = new TaskCompletionSource<SpeechRecognitionResult>();
                var startTime = DateTime.Now;

                // 配置事件处理
                recognizer.Recognized += (sender, e) =>
                {
                    var result = ProcessRecognitionResult(e, startTime);
                    _recognitionTask.SetResult(result);
                };

                recognizer.Canceled += (sender, e) =>
                {
                    var errorMsg = e.Reason == CancellationReason.Error ? e.ErrorDetails : "识别被取消";
                    _recognitionTask.SetResult(new SpeechRecognitionResult
                    {
                        IsSuccess = false,
                        ErrorMessage = errorMsg
                    });
                };

                // 开始识别
                await recognizer.StartContinuousRecognitionAsync();
                
                // 等待结果或超时
                var timeoutTask = Task.Delay(options.Timeout);
                var completedTask = await Task.WhenAny(_recognitionTask.Task, timeoutTask);

                await recognizer.StopContinuousRecognitionAsync();

                if (completedTask == timeoutTask)
                {
                    return new SpeechRecognitionResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "识别超时"
                    };
                }

                return await _recognitionTask.Task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "语音识别失败");
                return new SpeechRecognitionResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 开始连续语音识别
        /// </summary>
        public async Task<bool> StartContinuousRecognitionAsync()
        {
            try
            {
                if (_isRecognizing)
                {
                    await StopContinuousRecognitionAsync();
                }

                if (!await CheckMicrophonePermissionAsync())
                {
                    RecognitionError?.Invoke(this, "麦克风权限未授权");
                    return false;
                }

                var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                _recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

                // 配置事件处理
                _recognizer.Recognized += OnSpeechRecognized;
                _recognizer.Recognizing += OnSpeechRecognizing;
                _recognizer.Canceled += OnSpeechCanceled;

                await _recognizer.StartContinuousRecognitionAsync();
                _isRecognizing = true;

                _logger.LogInformation("连续语音识别已启动");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动连续语音识别失败");
                RecognitionError?.Invoke(this, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 停止连续语音识别
        /// </summary>
        public async Task StopContinuousRecognitionAsync()
        {
            try
            {
                if (_recognizer != null && _isRecognizing)
                {
                    await _recognizer.StopContinuousRecognitionAsync();
                    _recognizer.Dispose();
                    _recognizer = null;
                    _isRecognizing = false;
                    
                    _logger.LogInformation("连续语音识别已停止");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止语音识别失败");
            }
        }

        private void OnSpeechRecognized(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrEmpty(e.Result.Text))
            {
                var result = new SpeechRecognitionResult
                {
                    Text = e.Result.Text,
                    IsSuccess = true,
                    Timestamp = DateTime.Now,
                    Duration = e.Result.Duration,
                    Confidence = CalculateConfidence(e.Result),
                    Intent = ParseIntent(e.Result.Text)
                };

                SpeechRecognized?.Invoke(this, result);
                _logger.LogInformation("识别结果: {Text}", result.Text);
            }
        }

        private void OnSpeechRecognizing(object sender, SpeechRecognitionEventArgs e)
        {
            // 实时识别结果（部分结果）
            if (!string.IsNullOrEmpty(e.Result.Text))
            {
                var partialResult = new SpeechRecognitionResult
                {
                    Text = e.Result.Text,
                    IsSuccess = true,
                    Timestamp = DateTime.Now,
                    Confidence = 0.5 // 部分结果置信度较低
                };
                
                // 可以触发部分结果事件
            }
        }

        private void OnSpeechCanceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            var errorMsg = e.Reason == CancellationReason.Error ? e.ErrorDetails : "识别被取消";
            RecognitionError?.Invoke(this, errorMsg);
            _logger.LogWarning("语音识别取消: {Reason}", errorMsg);
        }

        private SpeechRecognitionResult ProcessRecognitionResult(SpeechRecognitionEventArgs e, DateTime startTime)
        {
            var duration = DateTime.Now - startTime;
            
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                return new SpeechRecognitionResult
                {
                    Text = e.Result.Text,
                    IsSuccess = true,
                    Duration = duration,
                    Timestamp = DateTime.Now,
                    Confidence = CalculateConfidence(e.Result),
                    Intent = ParseIntent(e.Result.Text)
                };
            }
            else
            {
                return new SpeechRecognitionResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"识别失败: {e.Result.Reason}",
                    Duration = duration,
                    Timestamp = DateTime.Now
                };
            }
        }

        private double CalculateConfidence(Microsoft.CognitiveServices.Speech.SpeechRecognitionResult result)
        {
            // Azure Speech Service 通常提供置信度信息
            // 这里使用简化的计算方法
            if (result.Text.Length > 0)
            {
                return Math.Min(0.95, 0.7 + (result.Text.Length * 0.01));
            }
            return 0.0;
        }

        private RecognizedIntent ParseIntent(string text)
        {
            var intent = new RecognizedIntent();
            
            // 质量检查相关的意图识别
            if (text.Contains("合格") || text.Contains("通过") || text.Contains("OK"))
            {
                intent.Action = "MARK_PASS";
                intent.Confidence = 0.9;
            }
            else if (text.Contains("不合格") || text.Contains("不通过") || text.Contains("NG") || text.Contains("不良"))
            {
                intent.Action = "MARK_FAIL";
                intent.Confidence = 0.9;
            }
            else if (text.Contains("记录") || text.Contains("备注"))
            {
                intent.Action = "ADD_NOTE";
                intent.Parameters["text"] = text;
                intent.Confidence = 0.8;
            }
            else if (text.Contains("下一个") || text.Contains("下一项"))
            {
                intent.Action = "NEXT_ITEM";
                intent.Confidence = 0.85;
            }
            else if (text.Contains("保存") || text.Contains("提交"))
            {
                intent.Action = "SAVE";
                intent.Confidence = 0.9;
            }
            else
            {
                intent.Action = "GENERAL_INPUT";
                intent.Parameters["text"] = text;
                intent.Confidence = 0.6;
            }

            return intent;
        }

        private async Task SetupKeywordRecognitionAsync(SpeechRecognizer recognizer, List<string> keywords)
        {
            // 为特定关键词设置识别增强
            var keywordModel = KeywordRecognitionModel.FromFile("keywords.table");
            await recognizer.StartKeywordRecognitionAsync(keywordModel);
        }

        private async Task<bool> CheckMicrophonePermissionAsync()
        {
#if IOS
            return await MicrophonePermissionService.RequestPermissionAsync();
#else
            return true; // 其他平台暂时返回true
#endif
        }

        public void Dispose()
        {
            StopContinuousRecognitionAsync().Wait();
            _speechConfig?.Dispose();
        }
    }
}

namespace QualityCheckApp.ViewModels
{
    /// <summary>
    /// 语音识别集成的检查页面ViewModel
    /// </summary>
    public class VoiceEnabledCheckViewModel : BaseViewModel
    {
        private readonly ISpeechRecognitionService _speechService;
        private readonly ICheckService _checkService;

        public VoiceEnabledCheckViewModel(
            ISpeechRecognitionService speechService,
            ICheckService checkService)
        {
            _speechService = speechService;
            _checkService = checkService;

            StartVoiceRecognitionCommand = new Command(async () => await StartVoiceRecognitionAsync());
            StopVoiceRecognitionCommand = new Command(async () => await StopVoiceRecognitionAsync());
            VoiceInputCommand = new Command(async () => await VoiceInputAsync());

            // 订阅语音识别事件
            _speechService.SpeechRecognized += OnSpeechRecognized;
            _speechService.RecognitionError += OnRecognitionError;
        }

        private bool _isListening;
        public bool IsListening
        {
            get => _isListening;
            set => SetProperty(ref _isListening, value);
        }

        private string _voiceText;
        public string VoiceText
        {
            get => _voiceText;
            set => SetProperty(ref _voiceText, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public Command StartVoiceRecognitionCommand { get; }
        public Command StopVoiceRecognitionCommand { get; }
        public Command VoiceInputCommand { get; }

        /// <summary>
        /// 开始连续语音识别
        /// </summary>
        private async Task StartVoiceRecognitionAsync()
        {
            try
            {
                var started = await _speechService.StartContinuousRecognitionAsync();
                if (started)
                {
                    IsListening = true;
                    StatusMessage = "正在监听语音输入...";
                }
                else
                {
                    StatusMessage = "语音识别启动失败";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"语音识别错误: {ex.Message}";
            }
        }

        /// <summary>
        /// 停止语音识别
        /// </summary>
        private async Task StopVoiceRecognitionAsync()
        {
            try
            {
                await _speechService.StopContinuousRecognitionAsync();
                IsListening = false;
                StatusMessage = "语音识别已停止";
            }
            catch (Exception ex)
            {
                StatusMessage = $"停止语音识别错误: {ex.Message}";
            }
        }

        /// <summary>
        /// 单次语音输入
        /// </summary>
        private async Task VoiceInputAsync()
        {
            try
            {
                StatusMessage = "请说话...";
                
                var options = new SpeechRecognitionOptions
                {
                    Language = "zh-CN",
                    Timeout = TimeSpan.FromSeconds(10),
                    Mode = SpeechRecognitionMode.Interactive
                };

                var result = await _speechService.StartRecognitionAsync(options);
                
                if (result.IsSuccess)
                {
                    VoiceText = result.Text;
                    StatusMessage = $"识别成功 (置信度: {result.Confidence:P1})";
                    
                    // 处理识别结果
                    await ProcessVoiceInputAsync(result);
                }
                else
                {
                    StatusMessage = $"识别失败: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"语音输入错误: {ex.Message}";
            }
        }

        private async void OnSpeechRecognized(object sender, SpeechRecognitionResult result)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                VoiceText = result.Text;
                StatusMessage = $"识别: {result.Text}";
                
                await ProcessVoiceInputAsync(result);
            });
        }

        private void OnRecognitionError(object sender, string error)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StatusMessage = $"识别错误: {error}";
                IsListening = false;
            });
        }

        /// <summary>
        /// 处理语音输入结果
        /// </summary>
        private async Task ProcessVoiceInputAsync(SpeechRecognitionResult result)
        {
            if (result.Intent == null) return;

            try
            {
                switch (result.Intent.Action)
                {
                    case "MARK_PASS":
                        await _checkService.MarkCurrentItemAsPassAsync();
                        StatusMessage = "已标记为合格";
                        break;

                    case "MARK_FAIL":
                        await _checkService.MarkCurrentItemAsFailAsync();
                        StatusMessage = "已标记为不合格";
                        break;

                    case "ADD_NOTE":
                        if (result.Intent.Parameters.TryGetValue("text", out var noteText))
                        {
                            await _checkService.AddNoteToCurrentItemAsync(noteText);
                            StatusMessage = "已添加备注";
                        }
                        break;

                    case "NEXT_ITEM":
                        await _checkService.MoveToNextItemAsync();
                        StatusMessage = "已移动到下一项";
                        break;

                    case "SAVE":
                        await _checkService.SaveCurrentCheckAsync();
                        StatusMessage = "已保存检查结果";
                        break;

                    case "GENERAL_INPUT":
                        // 通用文本输入，可以填入当前焦点字段
                        await HandleGeneralTextInputAsync(result.Text);
                        break;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"处理语音指令失败: {ex.Message}";
            }
        }

        private async Task HandleGeneralTextInputAsync(string text)
        {
            // 根据当前上下文决定如何处理文本输入
            var currentField = await _checkService.GetCurrentFocusFieldAsync();
            
            switch (currentField?.Type)
            {
                case "measurement":
                    // 尝试解析数值
                    if (TryParseNumber(text, out var number))
                    {
                        await _checkService.SetMeasurementValueAsync(number);
                        StatusMessage = $"已输入测量值: {number}";
                    }
                    break;

                case "note":
                    await _checkService.SetNoteAsync(text);
                    StatusMessage = "已输入备注";
                    break;

                default:
                    StatusMessage = $"语音输入: {text}";
                    break;
            }
        }

        private bool TryParseNumber(string text, out double number)
        {
            number = 0;
            
            // 中文数字转换
            text = text.Replace("点", ".")
                      .Replace("零", "0")
                      .Replace("一", "1")
                      .Replace("二", "2")
                      .Replace("三", "3")
                      .Replace("四", "4")
                      .Replace("五", "5")
                      .Replace("六", "6")
                      .Replace("七", "7")
                      .Replace("八", "8")
                      .Replace("九", "9");

            return double.TryParse(text, out number);
        }
    }
}

// 平台特定实现 - iOS
#if IOS
using AVFoundation;
using Speech;

namespace QualityCheckApp.Platforms.iOS
{
    public static class MicrophonePermissionService
    {
        public static async Task<bool> RequestPermissionAsync()
        {
            // 请求麦克风权限
            var micStatus = AVAudioSession.SharedInstance().RequestRecordPermission();
            if (!micStatus)
            {
                return false;
            }

            // 请求语音识别权限
            var speechStatus = await SFSpeechRecognizer.RequestAuthorizationAsync();
            return speechStatus == SFSpeechRecognizerAuthorizationStatus.Authorized;
        }
    }
}
#endif

namespace QualityCheckApp.Views
{
    /// <summary>
    /// 语音识别控制UI组件
    /// </summary>
    public class VoiceRecognitionControl : ContentView
    {
        private readonly VoiceEnabledCheckViewModel _viewModel;
        private Button _micButton;
        private Label _statusLabel;
        private Label _voiceTextLabel;

        public VoiceRecognitionControl(VoiceEnabledCheckViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10
            };

            // 麦克风按钮
            _micButton = new Button
            {
                Text = "🎤",
                FontSize = 24,
                WidthRequest = 60,
                HeightRequest = 60,
                BackgroundColor = Colors.LightBlue,
                CornerRadius = 30
            };
            _micButton.SetBinding(Button.CommandProperty, nameof(_viewModel.VoiceInputCommand));

            // 连续识别开关
            var toggleButton = new Button
            {
                Text = "连续识别",
                WidthRequest = 100
            };
            toggleButton.SetBinding(Button.CommandProperty, new Binding(
                nameof(_viewModel.IsListening),
                converter: new BoolToCommandConverter
                {
                    TrueCommand = _viewModel.StopVoiceRecognitionCommand,
                    FalseCommand = _viewModel.StartVoiceRecognitionCommand
                }));

            // 状态标签
            _statusLabel = new Label
            {
                FontSize = 14,
                TextColor = Colors.Gray
            };
            _statusLabel.SetBinding(Label.TextProperty, nameof(_viewModel.StatusMessage));

            // 识别文本显示
            _voiceTextLabel = new Label
            {
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Blue
            };
            _voiceTextLabel.SetBinding(Label.TextProperty, nameof(_viewModel.VoiceText));

            stackLayout.Children.Add(_micButton);
            stackLayout.Children.Add(toggleButton);

            var mainLayout = new StackLayout
            {
                Children = { stackLayout, _statusLabel, _voiceTextLabel },
                Spacing = 5
            };

            Content = mainLayout;
        }
    }

    /// <summary>
    /// 布尔值转命令转换器
    /// </summary>
    public class BoolToCommandConverter : IValueConverter
    {
        public Command TrueCommand { get; set; }
        public Command FalseCommand { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueCommand : FalseCommand;
            }
            return FalseCommand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
