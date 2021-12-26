namespace Server.IO
{
    public class Packet
    {
        public int OpCode { get; set; }
        public int Length { get; set; }
        public byte[]? Payload { get; set; }
    }
}