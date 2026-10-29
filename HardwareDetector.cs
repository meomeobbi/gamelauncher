using System;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace NativeGameHub
{
    public class HardwareSpecs
    {
        public string Cpu { get; set; } = "CPU";
        public string Gpu { get; set; } = "GPU";
        public string Ram { get; set; } = "RAM";
    }

    public static class HardwareDetector
    {
        public static HardwareSpecs Detect()
        {
            var specs = new HardwareSpecs();

            // 1. Detect CPU
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    string rawName = obj["Name"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(rawName))
                    {
                        specs.Cpu = FormatCpuName(rawName);
                        break;
                    }
                }
            }
            catch
            {
                specs.Cpu = "Intel i7";
            }

            // 2. Detect GPU
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                var gpus = searcher.Get().Cast<ManagementObject>()
                    .Select(o => o["Name"]?.ToString() ?? "")
                    .Where(n => !string.IsNullOrEmpty(n) && 
                                !n.Contains("Virtual", StringComparison.OrdinalIgnoreCase) && 
                                !n.Contains("Basic Display", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Prefer discrete GPU (NVIDIA / AMD) if available
                string bestGpu = gpus.FirstOrDefault(g => g.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) || 
                                                          g.Contains("Radeon", StringComparison.OrdinalIgnoreCase) || 
                                                          g.Contains("GeForce", StringComparison.OrdinalIgnoreCase)) 
                                 ?? gpus.FirstOrDefault() ?? "Intel GPU";

                specs.Gpu = FormatGpuName(bestGpu);
            }
            catch
            {
                specs.Gpu = "Intel UHD";
            }

            // 3. Detect RAM
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (var obj in searcher.Get())
                {
                    if (ulong.TryParse(obj["TotalPhysicalMemory"]?.ToString(), out ulong bytes))
                    {
                        double gb = Math.Round((double)bytes / (1024 * 1024 * 1024));
                        specs.Ram = $"{gb:0}GB";
                        break;
                    }
                }
            }
            catch
            {
                specs.Ram = "16GB";
            }

            return specs;
        }

        private static string FormatCpuName(string raw)
        {
            string clean = raw.Replace("(R)", "").Replace("(TM)", "").Trim();

            // Match i3/i5/i7/i9 models
            var match = Regex.Match(clean, @"(i[3579]-\w+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }

            // Match Ryzen models
            var ryzenMatch = Regex.Match(clean, @"(Ryzen \d \w+)", RegexOptions.IgnoreCase);
            if (ryzenMatch.Success)
            {
                return ryzenMatch.Value;
            }

            // Fallback shorten
            clean = Regex.Replace(clean, @"\d+th Gen |Intel |AMD |Core |Processor", "", RegexOptions.IgnoreCase).Trim();
            if (clean.Length > 16) clean = clean.Substring(0, 16);
            return clean;
        }

        private static string FormatGpuName(string raw)
        {
            string clean = raw.Replace("(R)", "").Replace("(TM)", "").Trim();

            // NVIDIA RTX / GTX
            var rtxMatch = Regex.Match(clean, @"(RTX\s*\d{3,4}\s*(?:Ti|Super)?)", RegexOptions.IgnoreCase);
            if (rtxMatch.Success) return rtxMatch.Value;

            var gtxMatch = Regex.Match(clean, @"(GTX\s*\d{3,4}\s*(?:Ti)?)", RegexOptions.IgnoreCase);
            if (gtxMatch.Success) return gtxMatch.Value;

            // Intel UHD / Iris Xe / Arc
            if (clean.Contains("UHD", StringComparison.OrdinalIgnoreCase)) return "Intel UHD";
            if (clean.Contains("Iris", StringComparison.OrdinalIgnoreCase)) return "Iris Xe";
            if (clean.Contains("Arc", StringComparison.OrdinalIgnoreCase)) return "Intel Arc";
            if (clean.Contains("Radeon", StringComparison.OrdinalIgnoreCase))
            {
                var radMatch = Regex.Match(clean, @"(RX\s*\d{3,4}\s*(?:XT)?)", RegexOptions.IgnoreCase);
                if (radMatch.Success) return radMatch.Value;
                return "AMD Radeon";
            }

            if (clean.Length > 14) clean = clean.Substring(0, 14);
            return clean;
        }
    }
}
