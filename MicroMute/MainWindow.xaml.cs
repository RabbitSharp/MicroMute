using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.CoreAudioApi;
using NHotkey;
using NHotkey.Wpf;
using System.Drawing;
using System.Windows.Forms;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace MicroMute
{
    public partial class MainWindow : Window
    {
        private NotifyIcon _ni;
        public MainWindow()
        {
            InitializeComponent();
            InitializeHotkeys();
            InitializeTray();
        }

        private void InitializeTray()
        {
            _ni = new NotifyIcon
            {
                Icon = new Icon("microphone.ico"),
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
            var pageDownKey = new KeyGesture(Key.PageDown);
            HotkeyManager.Current.AddOrReplace("Mute", pauseKey, OnMute);
            HotkeyManager.Current.AddOrReplace("Unmute", pageDownKey, OnUnmute);
        }

        private void OnUnmute(object? sender, HotkeyEventArgs e)
        {
            _ni.Icon = new Icon("microphone.ico");
        }

        private void OnMute(object? sender, HotkeyEventArgs e)
        {
            _ni.Icon = new Icon("microphone_muted.ico");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var test = new MMDeviceEnumerator();
            var col = test.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            //MediaCommands.MuteVolume
            foreach (var device in col)
            {
                device.AudioEndpointVolume.Mute = true;
            }
        }


        public void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            textBlock1.Text = "You Entered: " + textBox1.Text;
        }
    }
}
