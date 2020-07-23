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
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            var regValue = key.GetValue(_fileName);
            return regValue != null;
        }

        public bool ToggleState()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            var regValue = key.GetValue(_fileName);
            if (regValue == null)
            {
                key.SetValue(_fileName, _filePath);
                return true;
            }

            key.DeleteValue(_fileName);
            return false;
        }
    }
}