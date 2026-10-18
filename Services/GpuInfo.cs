using System;
using Microsoft.Win32;

namespace DeepLocal.Services
{
    public static class GpuInfo
    {
        public static long? DedicatedVramBytes()
        {
            try
            {
                using var video = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Video");
                if (video == null) return null;

                long best = 0;
                foreach (var guid in video.GetSubKeyNames())
                {
                    using var adapter = video.OpenSubKey(guid + @"\0000");
                    if (adapter?.GetValue("HardwareInformation.qwMemorySize") is long bytes && bytes > best)
                        best = bytes;
                }
                return best > 0 ? best : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
