using Server.IO;
using System.Net.Sockets;

namespace Server.Entities
{
    class Player
    {
        TcpClient _client;
        NetStream _stream;
        PacketHandler _handler;
        public int Id { get; set; }

        public Player(TcpClient client)
        {
            _client = client;
            _stream = new NetStream(client.GetStream());
            _handler = new PacketHandler(this);
        }

        public void Update()
        {
            
            _handler.Invoke(_stream);
        }

        public void Broadcast(Player[] toPlayers)
        {
            var data = _handler.Fetch();
            if (data.Count < 1) return;

            for (int i = 0; i < toPlayers.Length; i++)
            {
                foreach (var item in data)
                {
                    Console.WriteLine($"Total packets: {data.Count}");
                    toPlayers[i].Send(item);
                }
            }
        }

        public void Send(byte[] payload)
        {
            _client.Client.Send(payload);
        }
    }
}