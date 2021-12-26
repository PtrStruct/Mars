using Serilog;
using Server.Entities;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    class Kernel
    {
        private TcpListener _listener;
        private PlayerHandler _handler;

        public Kernel()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7171);
            _handler = new PlayerHandler(128);
        }

        public void Invoke()
        {
            _listener.Start();
            new Thread(AcceptInboundConnections).Start();
            new Thread(Update).Start();
            Log.Information("Kernel has been invoked.");
        }

        void AcceptInboundConnections()
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                _handler.AddPlayer(new Player(client));
            }

        }

        void Update()
        {
            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();
                _handler.Update();
                sw.Stop();

                var sleepdelta = 600 - (int)sw.ElapsedMilliseconds;
                if (sleepdelta > 0)
                    Thread.Sleep(sleepdelta);
                else
                    Log.Warning($"Tick elapsed: {sw.ElapsedMilliseconds}ms.\nServer cant't keep up!");
                sw.Reset();
            }
        }
    }
}