using System;
using System.Drawing;

namespace ListDebuggerVisualizer
{
    [Serializable]
    public class ListTypeItemSettings
    {
        public string Name { get; set; }
        public Point Location { get; set; }
        public Size Size { get; set; }
        public string GridSettingsFile { get; set; }
    }
}
