using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.IO
{
    class NetStream : BinaryReader
    {
        private NetworkStream _stream;

        private MemoryStream _inBuffer;
        private MemoryStream _outBuffer;
        public NetStream(NetworkStream stream) : base(stream)
        {
            _stream = stream;
            _inBuffer = new MemoryStream();
            _outBuffer = new MemoryStream();
        }

        public bool IsDataAvailable()
        {
            return _stream.DataAvailable;
        }
    }
}
