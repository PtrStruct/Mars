using Serilog;
using Server.Entities;
using System.Collections.Concurrent;
using System.Text;

namespace Server.IO
{
    class PacketHandler
    {
        private delegate void PacketHandlerDelegate(NetStream stream);
        private ConcurrentDictionary<byte, PacketHandlerDelegate> _packets;
        private ConcurrentQueue<Packet> _outPackets;

        Player _player;
        public bool BroadcastReady { get; set; }

        public PacketHandler(Player player)
        {
            _player = player;
            _outPackets = new ConcurrentQueue<Packet>();
            InitializePackets();
        }

        private void InitializePackets()
        {
            Log.Information("Initializing Network Packets..");
            _packets = new ConcurrentDictionary<byte, PacketHandlerDelegate>();
            _packets.TryAdd(1, MessageReceived);
        }

        public void Invoke(NetStream stream)
        {
            PacketHandlerDelegate packet;

            var opCode = stream.ReadByte();
            Log.Information($"[{_player.Id}] - Received OpCode: {opCode}");
            if (_packets.TryGetValue(opCode, out packet))
            {
                Log.Information($"Invoking: {packet.Method.Name}");
                packet.Invoke(stream);
            }
        }

        public byte[] Fetch()
        {
            var ms = new MemoryStream();
            _outPackets.TryDequeue(out var p);
            
            if (p != null)
            {
                ms.WriteByte((byte)p.OpCode);
                ms.Write(BitConverter.GetBytes(p.Length), 0, BitConverter.GetBytes(p.Length).Length);
                ms.Write(p.Payload);
                return ms.ToArray();
            }
            return null;
        }

        void MessageReceived(NetStream stream)
        {
            var length = stream.ReadInt32();
            var payload = stream.ReadBytes(length);
            Log.Information($"ASCII: {Encoding.ASCII.GetString(payload)}");
            Log.Information($"Length: {length}");
            _outPackets.Enqueue(new Packet
            {
                OpCode = 2,
                Length = length,
                Payload = payload
            });
        }
    }
}