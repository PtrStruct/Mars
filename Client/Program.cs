using Serilog;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Program
    {
        private static TcpClient tcpClient;
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                             .WriteTo.Console()
                             .WriteTo.Debug()
                             .WriteTo.File("Log.txt")
                             .CreateLogger();

            tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 7171);

            new Thread(ReceiveData).Start();
            new Thread(WriteDate).Start();

        }

        static void WriteDate()
        {
            while (true)
            {
                Console.Write("Message: ");
                var data = Console.ReadLine();

                var ms = new MemoryStream();
                ms.WriteByte(1); /* OpCode */
                ms.Write(BitConverter.GetBytes(data.Length)); /* Packet Length */
                ms.Write(Encoding.ASCII.GetBytes(data)); /* Payload */
                tcpClient.Client.Send(ms.ToArray());

                Log.Information($"Length: {data.Length}");
            }
        }


        static void ReceiveData()
        {
            while (true)
            {
                var stream = tcpClient.GetStream();
                var opcode = stream.ReadByte();
                var length = stream.ReadByte() +
                             stream.ReadByte() +
                             stream.ReadByte() +
                             stream.ReadByte();

                var buffer = new byte[length];
                stream.Read(buffer, 0, length);
                Log.Information($"OpCode: {opcode} - Msg: {Encoding.ASCII.GetString(buffer)} - Length: {length}");
            }
        }
    }
}