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
        private List<Packet> _outPackets;

        Player _player;
        public bool BroadcastReady { get; set; }

        public PacketHandler(Player player)
        {
            _player = player;
            _outPackets = new List<Packet>();
            InitializePackets();
        }

        private void InitializePackets()
        {
            Log.Information("Initializing Network Packets..");
            _packets = new ConcurrentDictionary<byte, PacketHandlerDelegate>();
            _packets.TryAdd(1, MessageReceived);
        }

        /// <summary>
        /// Reads at most 5 packets each tick.
        /// </summary>
        /// <param name="stream"></param>
        public void Invoke(NetStream stream)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!stream.IsDataAvailable()) continue;

                var opCode = stream.ReadByte();
                Log.Information($"[{_player.Id}] - Received OpCode: {opCode}");
                if (_packets.TryGetValue(opCode, out PacketHandlerDelegate packet))
                {
                    Log.Information($"Invoking: {packet.Method.Name}");
                    packet.Invoke(stream);
                }
            }
            
        }

        /// <summary>
        /// Process 2 packets per OpCode
        /// </summary>
        /// <returns></returns>
        public List<byte[]> Fetch()
        {
            List<byte[]> outData = new List<byte[]>();
            var packets = _outPackets.GroupBy(x => x.OpCode);
            var ms = new MemoryStream();

            foreach (var group in packets)
            {

                var toSend = group.Take(2).ToList();
                
                for (int i = 0; i < toSend.Count; i++)
                {
                    ms.WriteByte((byte)toSend[i].OpCode);
                    ms.Write(BitConverter.GetBytes(toSend[i].Length));
                    ms.Write(toSend[i].Payload);
                    outData.Add(ms.ToArray());
                    ms.SetLength(0);
                }

                foreach (var item in toSend)
                {
                    _outPackets.Remove(item);
                }
            }

            return outData;
        }

        void MessageReceived(NetStream stream)
        {
            var length = stream.ReadInt32();
            var payload = stream.ReadBytes(length);
            Log.Information($"ASCII: {Encoding.ASCII.GetString(payload)}");
            Log.Information($"Length: {length}");
            _outPackets.Add(new Packet
            {
                OpCode = 2,
                Length = length,
                Payload = payload
            });
        }
    }
}