using System;
using System.Collections.Generic;
using System.IO;

namespace NativeGameHub
{
    public class GameInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Developer { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Release { get; set; } = "";
        public string Description { get; set; } = "";
        public string ExecutablePath { get; set; } = "";
        public string Arguments { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";
        public string Folder { get; set; } = "";
        public string CoverUrl { get; set; } = "";
        public string HeroUrl { get; set; } = "";
        public string ThemeColor { get; set; } = "#00F2FE";
        public List<string> Tags { get; set; } = new();

        public bool IsInstalled => File.Exists(ExecutablePath);
        public string StatusBadgeText => IsInstalled ? "● SẴN SÀNG" : "○ CHƯA CÀI";
        public string StatusBadgeColor => IsInstalled ? "#51CF66" : "#FCC419";
    }
}
