using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp39
{
    public sealed class Profile
    {
        public List<ValueType> SampleTypes { get; set; }

        public List<Sample> Samples { get; set; }

        public List<Mapping> Mappings { get; set; }

        public List<Location> Locations { get; set; }

        public List<Function> Functions { get; set; }

        public List<string> StringTable { get; set; }

        public long DropFrames { get; set; }

        public long KeepFrames { get; set; }

        public long TimeNanos { get; set; }

        public long DurationNanos { get; set; }

        public ValueType PeriodType { get; set; }

        public long Period { get; set; }

        public List<long> Comments { get; set; }

        public long DefaultSampleType { get; set; }
    }
}
