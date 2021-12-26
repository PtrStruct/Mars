using Serilog;
using Server.Core;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                             .WriteTo.Console()
                             .WriteTo.Debug()
                             .WriteTo.File("Log.txt")
                             .CreateLogger();


            var kernel = new Kernel();
            kernel.Invoke();
        }
    }
}