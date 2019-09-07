Tools that you may need:
* Visual Studio 2019 (No specific edition)
* Optional: [Markdown Editor](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor).
  This helps you when editing the **.md** files.
* Optional: [VSColorOutput](https://marketplace.visualstudio.com/items?itemName=MikeWard-AnnArbor.VSColorOutput).
  This colors the entries in the **Output** window making it easier to read.

Simple steps to get up and running code
1. Load and build IronSmalltalk\DLR\DLR.sln in Visual Studio
2. Load and build IronSmalltalk.sln in Visual Studio
3. The Testing solution folder contains both unit test and a TestPlayground UI project that let you expemiment with compiler components.

#### Project and MSBuild Files ####
Most projects are ___.Net SDK Project___ files. 
Those project reference common **.props** and **.targets** files containg
properties and options common to all projects.

The shared files are named **Directory.Build.props** and **Directory.Build.targets**.
Several of those files exist in the **src** folder and the subfolders. For more information,
see: https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2019
