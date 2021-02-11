using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


[assembly: DebuggerVisualizer(typeof(ListDebuggerVisualizer.ListDebuggerVisualizerClient), typeof(ListDebuggerVisualizer.VisualizerJsonObjectSource), Target = typeof(List<>), Description = "List Debugger Visualizer")]
namespace ListDebuggerVisualizer
{
    public class ListDebuggerVisualizerClient : DialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            try
            {
                ShowVisualizer(objectProvider);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception getting object data: " + ex.Message);
            }
        }

        private static void ShowVisualizer(IVisualizerObjectProvider objectProvider)
        {
            CosturaUtility.Initialize();

            var jsonStream = objectProvider.GetData();
            var reader = new StreamReader(jsonStream);
            string json = reader.ReadToEnd();
            var container = Newtonsoft.Json.JsonConvert.DeserializeObject<VisualizerDataContainer>(json);

            if (container.TypeName == "string")
            {
                var prim = new List<PrimitiveListItem>();

                foreach (string strItem in container.Data.Cast<string>())
                {
                    var listItem = new PrimitiveListItem();
                    listItem.Value = strItem;
                    prim.Add(listItem);
                }

                ListDebuggerVisualizerClient.ShowVisualizerForm(prim, container.TypeName);
            }
            else if (container.IsPrimitive)
            {
                var primitiveList = new List<PrimitiveListItem>();
                foreach (object objItem in container.Data.Cast<object>())
                {
                    var primitiveListItem = new PrimitiveListItem();
                    primitiveListItem.Value = objItem?.ToString() ?? "";
                    primitiveList.Add(primitiveListItem);
                }

                ListDebuggerVisualizerClient.ShowVisualizerForm(primitiveList, container.TypeName);
            }
            else
            {
                ListDebuggerVisualizerClient.ShowVisualizerForm(container.Data, container.TypeName);
            }
        }

        public static void ShowVisualizerForm(IList data, string typeName)
        {
            var form = new MainForm();
            form.Model = data;
            form.SetupSummaries();
            form.ListType = typeName;
            form.ShowDialog();
        }
    }

}
