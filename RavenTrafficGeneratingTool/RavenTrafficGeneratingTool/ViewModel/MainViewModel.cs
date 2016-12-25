using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using RavenTrafficGeneratingTool.Helpers;
using RavenTrafficGeneratingTool.Messages;
using RavenTrafficGeneratingTool.Services;

namespace RavenTrafficGeneratingTool.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private readonly ITrafficService _trafficService;
        private string _databaseUrl;
        private string _messageText;
        private string _fullText;
        private int _timesPerMinute;
        private bool _startIsEnabled = true;
        private bool _stopIsEnabled;
        private bool _clearIsEnabled;
        private bool _isDatabaseUrlFocused;
        private bool _isAutoScrollEnabled;
        private ICommand _keepDownCommand;
        private ICommand _startCommand;
        private ICommand _stopCommand;
        private ICommand _windowLoadedCommand;
        private ICommand _clearCommand;

        public MainViewModel(IConfigurationService configurationService, ITrafficService trafficService)
        {
            _configurationService = configurationService;
            _trafficService = trafficService;
            Messenger.Default.Register<RandomMessage>(this, message =>
            {
                if (!ClearIsEnabled)
                {
                    ClearIsEnabled = true;
                }
                MessageText = message.Text + "\n";
            });
        }

        public string DatabaseUrl
        {
            get { return _databaseUrl; }
            set
            {
                Set(ref _databaseUrl, value);
                if (!CheckDatabaseUrl())
                {
                    throw new ValidationException("Invalid URL");
                }
            }
        }

        public string MessageText
        {
            get { return _messageText; }
            set { Set(ref _messageText, value); }
        }

        public int TimesPerMinute
        {
            get { return _timesPerMinute; }
            set { Set(ref _timesPerMinute, value); }
        }

        public bool StartIsEnabled
        {
            get { return _startIsEnabled; }
            set { Set(ref _startIsEnabled, value); }
        }

        public bool StopIsEnabled
        {
            get { return _stopIsEnabled; }
            set { Set(ref _stopIsEnabled, value); }
        }

        public bool ClearIsEnabled
        {
            get { return _clearIsEnabled; }
            set { Set(ref _clearIsEnabled, value); }
        }

        public string FullText
        {
            get { return _fullText; }
            set { Set(ref _fullText, value); }
        }

        public bool IsAutoScrollEnabled
        {
            get { return _isAutoScrollEnabled; }
            set { Set(ref _isAutoScrollEnabled, value); }
        }

        public bool IsDatabaseUrlFocused
        {
            get { return _isDatabaseUrlFocused; }
            set { Set(ref _isDatabaseUrlFocused, value); }
        }

        public ICommand KeepDownCommand
        {
            get
            {
                return _keepDownCommand ?? (_keepDownCommand = new RelayCommand<bool>(isEnabled =>
                {
                    IsAutoScrollEnabled = AutoScrollBehavior.IsEnabled = isEnabled;
                    _configurationService.SetValue("IsAutoScrollEnabled", IsAutoScrollEnabled.ToString());
                }));
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new RelayCommand(() =>
                {
                    StartIsEnabled = false;
                    StopIsEnabled = true;
                    _trafficService.Start(DatabaseUrl, TimesPerMinute);
                }));
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new RelayCommand(() =>
                {
                    StartIsEnabled = true;
                    StopIsEnabled = false;
                    _trafficService.Stop();
                }));
            }
        }

        public ICommand WindowLoadedCommand
        {
            get
            {
                return _windowLoadedCommand ?? (_windowLoadedCommand = new RelayCommand(() =>
                {
                    string url = _configurationService.GetValue("DatabaseUrl");
                    string timesPerMinuteStr = _configurationService.GetValue("timesPerMinute");
                    string isAutoScrollEnabledStr = _configurationService.GetValue("IsAutoScrollEnabled");
                    if (!String.IsNullOrEmpty(url))
                    {
                        DatabaseUrl = url;
                    }
                    int timesPerMinute;
                    if (Int32.TryParse(timesPerMinuteStr, out timesPerMinute))
                    {
                        TimesPerMinute = timesPerMinute;
                    }
                    bool isAutoScrollEnabled;
                    if (Boolean.TryParse(isAutoScrollEnabledStr, out isAutoScrollEnabled))
                    {
                        IsAutoScrollEnabled = AutoScrollBehavior.IsEnabled = isAutoScrollEnabled;
                    }
                    IsDatabaseUrlFocused = true;
                }));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(() =>
                {
                    // hack for cleaning textBox
                    FullText = null;
                    FullText = String.Empty;
                    ClearIsEnabled = false;
                }));
            }
        }

        public override void Cleanup()
        {
            ((IDisposable)_trafficService)?.Dispose();
            base.Cleanup();
        }

        private bool CheckDatabaseUrl()
        {
            Uri uriResult;
            return String.IsNullOrEmpty(DatabaseUrl) || (Uri.TryCreate(DatabaseUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps));
        }
    }
}