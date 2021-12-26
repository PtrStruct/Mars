using Serilog;
using Server.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _outPackets.TryDequeue(out var p);
            return p.Payload;
        }

        void MessageReceived(NetStream stream)
        {
            BroadcastReady = true;
            var length = stream.ReadInt32();
            var payload = stream.ReadBytes(length);
            Log.Information(Encoding.ASCII.GetString(payload));
            _outPackets.Enqueue(new Packet 
            {
                OpCode = 2,
                Length = length,
                Payload = payload
            });
        }


    }
}