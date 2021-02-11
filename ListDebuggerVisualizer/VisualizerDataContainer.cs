using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


[assembly: DebuggerVisualizer(typeof(ListDebuggerVisualizer.ListDebuggerVisualizerClient), typeof(ListDebuggerVisualizer.VisualizerJsonObjectSource), Target = typeof(List<>),
                     Description = "List Debugger Visualizer")]

namespace ListDebuggerVisualizer
{
    public class VisualizerDataContainer
    {
        public string TypeName { get; set; }
        public IList Data { get; set; }
        public bool IsPrimitive { get; set; }
    }

}
