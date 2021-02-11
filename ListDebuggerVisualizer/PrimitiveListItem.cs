using System.Collections.Generic;
using System.Diagnostics;


[assembly: DebuggerVisualizer(typeof(ListDebuggerVisualizer.ListDebuggerVisualizerClient), typeof(ListDebuggerVisualizer.VisualizerJsonObjectSource), Target = typeof(List<>), 
    Description = "List Debugger Visualizer")]

namespace ListDebuggerVisualizer
{
    public class PrimitiveListItem
    {
        public string Value { get; set; }
    }
}
