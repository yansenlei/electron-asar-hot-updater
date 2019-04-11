// updater.cs
//
// compile to updater.exe using: mcs updater.cs
//
// Takes two arguments; a source file and a destination file
// Waits 5 seconds, then will move source to destination, as long as source exists

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

public class CommandLine
{
  [DllImport("kernel32.dll")]
  static extern IntPtr GetConsoleWindow();

  [DllImport("user32.dll")]
  static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  const int SW_HIDE = 0;
  public static void Main(string[] args)
  {
    // Hide this console window
    var handle = GetConsoleWindow();
    ShowWindow(handle, 0);

    string updateAsar = "";
    string appAsar = "";
  
    // wait for the Electron app to close (5 secs)
    System.Threading.Thread.Sleep(5000);
    if (args.Length >= 2) 
    {
      updateAsar = args[0];
      appAsar    = args[1];
     
      System.IO.FileInfo fileInfoSrc = new System.IO.FileInfo(updateAsar);
      System.IO.FileInfo fileInfoDest = new System.IO.FileInfo(appAsar);
      
      // the update asar doesn't exist
      if(!fileInfoSrc.Exists) Environment.Exit(1); 
      
      if(fileInfoDest.Exists)                                        
      {
          System.IO.File.Copy(updateAsar,appAsar, true);
          System.IO.File.Delete(updateAsar);                 
      } 
      else
      {
          System.IO.File.Move(updateAsar,appAsar);                 
      }

      if (args.Length == 3) {
        Process appProcess = new Process();
        try
        {
          appProcess.StartInfo.FileName = args[2];
          appProcess.Start();
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    } 
    // we need two filepaths - the update.asar and the app.asar
    else 
    {
      Environment.Exit(1);
    }
  }
}

