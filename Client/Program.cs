using Serilog;
using System.Net.Sockets;
using System.Text;

namespace Client
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

            Log.Information("Test");

            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 7171);

            while (true)
            {
                Console.Write("Message: ");
                var data = Console.ReadLine();

                var ms = new MemoryStream();
                ms.WriteByte(1);
                ms.Write(BitConverter.GetBytes(data.Length));
                ms.Write(Encoding.ASCII.GetBytes(data));
                client.Client.Send(ms.ToArray());
            }
        }
    }
}