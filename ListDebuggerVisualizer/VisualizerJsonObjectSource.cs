using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ListDebuggerVisualizer
{

    public class VisualizerJsonObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var itemType = target.GetType().GetProperty("Item").PropertyType;
            var container = new VisualizerDataContainer();
            container.TypeName = itemType.Name;
            container.Data = (IList)target;
            container.IsPrimitive = itemType.IsPrimitive;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(container);
            var writer = new StreamWriter(outgoingData);
            writer.WriteLine(json);
            writer.Flush();
        }
    }

}
