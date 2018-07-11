namespace ConsoleApp39
{
    using System.Collections.Generic;

    public sealed class Location
    {
        public ulong Id { get; set; }

        public ulong MappingId { get; set; }

        public ulong Address { get; set; }

        public List<Line> Lines { get; set; }

        public bool IsFolded { get; set; }
    }
}