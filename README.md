# Implementation of Smalltalk on the DLR. 

The immediate goal of the project is to create a Smalltalk dialect that: 
1. Is X3J20 (ANSI INCITS 319-1998) Compliant Smalltalk 
2. Is First Class Member of the DLR Family 
3. Has an easy / transparent interop with .Net 
4. Has Decent Performance

Future goals include: 
1. A GUI interface layer; at some point hopefully having a system that could target either XAML or XNA. 
2. Better integration with .Net languages, class libraries and tools.

There are 3 things you can do if you would like to help out with this project: 
1. Become familiar with X3J20 (http://webstore.ansi.org/RecordDetail.aspx?sku=ANSIINCITS319-1998+(R2007)) 
2. Study how the DLR works. There are some useful links in the Documentation directory of the source download. 
3. Start playing!

Simple steps to get up and running code
1. Load and build IronSmalltalk\DLR\DLR.sln in Visual Studio
2. Load and build IronSmalltalk.sln in Visual Studio
3. The Testing solution folder contains both unit test and a TestPlayground UI project that let you expemiment with compiler components.
