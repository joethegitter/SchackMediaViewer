# SchackMediaViewer
"Slide Show" app for MikeScha, with screen saver.

Visual Studio Solution "V1" contains the following projects:
- TechTest_WPF_HwndSource: just some idea testing.
- TechTest_WPF_ScSvStub: testing the SchackSaver Stub in WPF. Work in progress.
- Winforms_ScSVStub: the screen saver stub we use to launch the Schack Media Viewer as a screen saver. 
   - For debug version, edit the PATH variable in Program.cs, rebuild the Winforms_ScSVStubproject, and put the ScSVStub.scr file from your \Bin\Debug folder into the Windows/System32 directory on your Windows machine. In the Screen Saver control panel, choose ScSVStub to view the Schack Media Viewer app as a screen saver in the control panel, and set it as your default screen saver.
   - For retail version, rebuild the Winforms_ScSVStubproject, and put the ScSVStub.scr file from your \Bin\Retail folder into the Windows/System32 directory on your Windows machine. Place the  Schack Media Viewer app into the Windows/System32 directory on your Windows machine as well.
