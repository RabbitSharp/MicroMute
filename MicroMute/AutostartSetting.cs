using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace MicroMute
{
    public class AutostartSetting
    {
        private readonly string _filePath;
        private readonly string _fileName;

        public AutostartSetting()
        {
            using var currentProcess = Process.GetCurrentProcess();
            _filePath = currentProcess.MainModule.FileName;
            _fileName = Path.GetFileNameWithoutExtension(_filePath);
        }

        public bool GetCurrentState()
        {
            var regValue = GetRegValue();
            if (regValue == null)
                return false;

            if (regValue != _filePath) 
                UpdateRegValue(_filePath);

            return true;
        }

        public void EnableAutostart()
        {
            UpdateRegValue(_filePath);
        }

        public void DisableAutostart()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            key.DeleteValue(_fileName);
        }

        private string? GetRegValue()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            return key.GetValue(_fileName) as string;
        }

        private void UpdateRegValue(string value)
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            key.SetValue(_fileName, value);
        }
    }
}