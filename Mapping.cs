using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp39
{
    public sealed class Mapping
    {
        public ulong Id { get; set; }

        public ulong MemoryStart { get; set; }

        public ulong MemoryLimit { get; set; }

        public ulong FileOffset { get; set; }

        public long FileNameIndex { get; set; }

        public long BuildIdIndex { get; set; }

        public bool HasFunctions { get; set; }

        public bool HasFilenames { get; set; }

        public bool HasLineNumbers { get; set; }

        public bool HasInlineFrames { get; set; }
    }
}