﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Models {
    public class SettingsModel {
        /// <summary>
        /// If elements should follow the newer rounded Windows 11 style.
        /// </summary>
        public Boolean CornerRadius { get; set; } = true;

        /// <summary>
        /// If the Start screen should use the desktop wallpaper.
        /// </summary>
        public Boolean UseDesktopWallpaper { get; set; } = true;

        /// <summary>
        /// If the ActionBar is enabled.
        /// </summary>
        public Boolean EnableActionBar { get; set; } = true;

        /// <summary>
        /// If the StatusBar is enabled.
        /// </summary>
        public Boolean EnableStatusBar { get; set; } = false;
    }
}
