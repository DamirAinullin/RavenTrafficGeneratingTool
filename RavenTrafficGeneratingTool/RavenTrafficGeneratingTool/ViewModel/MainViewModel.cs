using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using RavenTrafficGeneratingTool.Helpers;
using RavenTrafficGeneratingTool.Services;

namespace RavenTrafficGeneratingTool.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _databaseUrl;
        private int _timesPerMinute;
        private bool _startIsEnabled;
        private bool _stopIsEnabled;
        private string _fullText;
        private readonly IConfigurationService _configurationService;
        private bool _isAutoScrollEnabled;
        private ICommand _keepDownCommand;
        private ICommand _startCommand;
        private ICommand _stopCommand;
        private ICommand _windowLoadedCommand;
        private bool _isDatabaseUrlFocused;

        public MainViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
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

                }));
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new RelayCommand(() =>
                {

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

        private bool CheckDatabaseUrl()
        {
            Uri uriResult;
            return String.IsNullOrEmpty(DatabaseUrl) || (Uri.TryCreate(DatabaseUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps));
        }
    }
}