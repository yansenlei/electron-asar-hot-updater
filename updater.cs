// updater.cs
//
// compile to updater.exe using: mcs updater.cs
//
// Takes two arguments; a source file and a destination file
// Waits 5 seconds, then will move source to destination, as long as source exists

using System;

public class CommandLine
{
  public static void Main(string[] args)
  {
    string updateAsar = "";
    string appAsar = "";
    
    // wait for the Electron app to close (5 secs)
    System.Threading.Thread.Sleep(5000);
    
    if (args.Length == 2) 
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
    } 
    // we need two filepaths - the update.asar and the app.asar
    else 
    {
      Environment.Exit(1);
    }
  }
}
