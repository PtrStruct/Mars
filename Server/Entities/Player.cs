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
            if (!_stream.IsDataAvailable()) return;
            _handler.Invoke(_stream);
        }

        public void Broadcast(Player[] toPlayers)
        {
            var data = _handler.Fetch();
            if (data == null) return;

            for (int i = 0; i < toPlayers.Length; i++)
            {
                toPlayers[i].Send(data);
            }
        }

        public void Send(byte[] payload)
        {
            _client.Client.Send(payload);
        }
    }
}