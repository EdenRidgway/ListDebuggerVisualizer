using ListDebuggerVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListDebuggerVisualizer2019.Tests
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<TestObject> testObjects = new List<TestObject>()
            {
                new TestObject() { Id = 1, Name = "Test 1", Loss = 123.23 },
                new TestObject() { Id = 2, Name = "Test 2", Loss = 563.13, IsActive = true, CreatedDate = DateTime.Now }
            };

            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(testObjects, typeof(ListDebuggerVisualizerClient), typeof(VisualizerJsonObjectSource));
            visualizerHost.ShowVisualizer();
        }

        class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Loss { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}
