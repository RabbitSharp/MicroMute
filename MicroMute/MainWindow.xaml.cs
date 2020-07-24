using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.CoreAudioApi;
using NHotkey;
using NHotkey.Wpf;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace MicroMute
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private NotifyIcon? _ni;
        private readonly AutostartSetting _autostartSetting;
        private bool _isMicMuted;
        private bool _autostartIsChecked;
        private string? _windowTitle;

        public MainWindow()
        {
            InitializeComponent();
            _autostartSetting = new AutostartSetting();
            _autostartIsChecked = _autostartSetting.GetCurrentState();
            InitializeHotkeys();
            InitializeTray();
            GetMicStatus();
            DataContext = this;
        }

        private void InitializeTray()
        {
            _ni = new NotifyIcon
            {
                Icon = CreateTrayIcon("microphone.ico"),
                Visible = true
            };

            _ni.DoubleClick += delegate
            {
                Show();
                WindowState = WindowState.Normal;
            };

            Hide();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        private void InitializeHotkeys()
        {
            var pauseKey = new KeyGesture(Key.Pause);
            HotkeyManager.Current.AddOrReplace("ToggleMute", pauseKey, ToggleMicMute);
        }

        private void ToggleMicMute(object? sender, HotkeyEventArgs e)
        {
            ToggleMicMute();
        }

        private void ToggleMicMute(object sender, RoutedEventArgs e)
        {
            ToggleMicMute();
        }

        private void ToggleMicMute()
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            _isMicMuted = !_isMicMuted;
            foreach (var device in devices)
            {
                device.AudioEndpointVolume.Mute = _isMicMuted;
            }
            SetStatus();
        }

        private void GetMicStatus()
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            _isMicMuted = devices.Any(x => x.AudioEndpointVolume.Mute);
            SetStatus();
        }

        private void SetStatus()
        {
            if (_isMicMuted)
            {
                _ni!.Icon = CreateTrayIcon("microphone_muted.ico");
                Background = new BrushConverter().ConvertFrom("#FFed3c46") as SolidColorBrush;
                ButtonMute.Content = "Unmute Microphone";
                Icon = new BitmapImage(new Uri("pack://application:,,,/microphone_muted.ico"));
            }
            else
            {
                _ni!.Icon = CreateTrayIcon("microphone.ico");
                Background = new BrushConverter().ConvertFrom("#FF5696A7") as SolidColorBrush;
                ButtonMute.Content = "Mute Microphone";
                Icon = new BitmapImage(new Uri("pack://application:,,,/microphone.ico"));
            }
        }

        private Icon CreateTrayIcon(string fileName)
        {
            var iconUri = new Uri($"pack://application:,,,/{fileName}");
            using var iconStream = Application.GetResourceStream(iconUri)?.Stream;
            if (iconStream == null)
            {
                throw new IOException($"'{fileName}' could not be found.");
            }
            return new Icon(iconStream);
        }

        public string WindowTitle => _windowTitle ??= $"MicroMute {GetVersion()}";

        private string? GetVersion()
        {
            var customAttribute = typeof(MainWindow).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return customAttribute?.InformationalVersion;
        }

        public bool AutostartIsChecked
        {
            get => _autostartIsChecked;
            set
            {
                _autostartIsChecked = value;
                if (_autostartIsChecked)
                {
                    _autostartSetting.EnableAutostart();
                }
                else
                {
                    _autostartSetting.DisableAutostart();
                }
                OnPropertyChanged(nameof(AutostartIsChecked));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string? propertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}