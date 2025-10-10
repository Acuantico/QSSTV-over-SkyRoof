using Serilog;
using VE3NEA;

namespace SkyRoof
{
  internal static class Program
  {
    [STAThread]
    static void Main()
    {
      using (new SingleInstanceEnforcer())
      {
        try
        {
          ExceptionLogger.Initialize();
          ApplicationConfiguration.Initialize();
          Application.Run(new MainForm());
        }
        finally
        {
          Log.Information("Stopping SkyRoof\r\n-----------------------\r\n");
          Log.CloseAndFlush();
        }
      }
    }
  }
}