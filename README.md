# Visual Studio List Debugger Visualizer
This is forked version of the Visual Studio List debugger that Antonia Bakula wrote with the following changes:
* Replaces the Telerik RadGridView with the standard windows forms DataGridView
* Improves the Excel export to handle int, double and date type exports and formats intead of treating everything as text
* The Excel export launches Excel after the export.

The Visual Studio debugger visualizer for List<T> or more precisly if inspected object implement IList.

Installation available on Visual Studio Marketplace: https://marketplace.visualstudio.com/items?itemName=AntonioBakula.vs2019-list-visualizer

### Binaries download:

GitHub releases for this project.

### Installation:
Simply copy visualizer dll to your Visual Studio Visualizers folder, default folder is:

* VS 2019 -> c:\Users\<username>\Documents\Visual Studio 2019\Visualizers

### Usage:
Types that implement IList will get little magnifier icon in Debugger  Locals / Variables Watch, like this:
![alt text](http://www.antoniob.com/EasyEdit/UserFiles/ListVisualizer/list-visualizer-usage.png "Visual Studio List Debugger Visualizer usage")

Click on magnifier icon and if IList members are marked as Serializable, their contents will be shown in grid, like on screenshots below.

### Screenshots:

![alt text](/Documentation/ListDebuggerVisualizer.png "Visual Studio List Debugger Visualizer usage")
