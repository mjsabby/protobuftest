using System;
using System.Diagnostics.Tracing;

namespace ConsoleApp39
{
    using System.Collections.Generic;
    using System.IO;

    using Google.Protobuf;

    internal sealed class Utf8StringConversionEventSource : EventSource
    {
        public void Utf8ToString(string incoming, Utf8String converted)
        {
            this.WriteEvent(1, incoming, converted);
        }

        public void StringToUtf8(Utf8String incoming, string converted)
        {
            this.WriteEvent(2, incoming, converted);
        }
    }

    public sealed class Utf8String
    {
        private long length;

        private byte[] data;

        public Utf8String(int value)  //constructor
        {
            this.value = value;
        }

        [Obsolete("Implicit conversion of a Utf8String to string is a warning", error: false)]
        public static implicit operator string(Utf8String incoming)
        {
            return string.Empty;
        }

        [Obsolete("Implicit conversion of a string to Utf8String is a warning", error: false)]
        public static implicit operator Utf8String(string incoming)
        {
            return null;
        }

        static public explicit operator int(RomanNumeral roman)
        {
            return roman.value;
        }

        static public implicit operator string(RomanNumeral roman)
        {
            return ("Conversion to string is not implemented");
        }
    }

    struct BinaryNumeral
    {
        private int value;

        public BinaryNumeral(int value)  //constructor
        {
            this.value = value;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var profile = new Profile();

            RomanNumeral y = new RomanNumeral();
            BinaryNumeral x = y;

            using (var fs = new FileStream(@"C:\users\muks\desktop\perf.pb", FileMode.Open, FileAccess.Read))
            {
                using (var cis = new CodedInputStream(fs))
                {
                    var valueTypeList = new List<ValueType>();
                    var samplesList = new List<Sample>();
                    var mappingList = new List<Mapping>();
                    var locationList = new List<Location>();
                    var stringTable = new List<string>();

                    profile.SampleTypes = valueTypeList;
                    profile.Samples = samplesList;
                    profile.Locations = locationList;
                    profile.StringTable = stringTable;
                    profile.Mappings = mappingList;

                    while (true)
                    {
                        var tag = cis.ReadTag();
                        if (tag > 0)
                        {
                            var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                            switch (fieldNumber)
                            {
                                case 1:
                                    valueTypeList.Add(HandleValueType(cis));
                                    break;
                                case 2:
                                    samplesList.Add(HandleSample(cis));
                                    break;
                                case 3:
                                    mappingList.Add(HandleMapping(cis));
                                    break;
                                case 4:
                                    locationList.Add(HandleLocation(cis));
                                    break;
                                case 5:
                                    cis.SkipLastField();
                                    break;
                                case 6:
                                    var strLength = cis.ReadLength();
                                    stringTable.Add(System.Text.Encoding.UTF8.GetString(cis.ReadRawBytes(strLength), 0, strLength));
                                    break;
                                default:
                                    cis.SkipLastField();
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < profile.Mappings.Count; ++i)
            {
                var mapping = profile.Mappings[i];

                var filename = profile.StringTable[(int)mapping.FileNameIndex];
                var rangeBegin = mapping.MemoryStart;
                var rangeEnd = mapping.MemoryLimit;

                Console.WriteLine($"Filename: {filename}, Range Start: {rangeBegin}, Rang End: {rangeEnd}");
            }
        }

        private static Label HandleLabel(CodedInputStream cis)
        {
            var label = new Label();

            long pos = cis.ReadLength();
            pos += cis.Position;
            while (pos - cis.Position > 0)
            {
                var tag = cis.ReadTag();
                if (tag > 0)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                    switch (fieldNumber)
                    {
                        case 1:
                            label.Key = cis.ReadInt64();
                            break;
                        case 2:
                            label.Str = cis.ReadInt64();
                            break;
                        case 3:
                            label.Num = cis.ReadInt64();
                            break;
                        case 4:
                            label.NumUnit = cis.ReadInt64();
                            break;
                        default:
                            cis.SkipLastField();
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            return label;
        }

        private static Mapping HandleMapping(CodedInputStream cis)
        {
            var mapping = new Mapping();

            long pos = cis.ReadLength();
            pos += cis.Position;
            while (pos - cis.Position > 0)
            {
                var tag = cis.ReadTag();
                if (tag > 0)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                    switch (fieldNumber)
                    {
                        case 1:
                            mapping.Id = cis.ReadUInt64();
                            break;
                        case 2:
                            mapping.MemoryStart = cis.ReadUInt64();
                            break;
                        case 3:
                            mapping.MemoryLimit = cis.ReadUInt64();
                            break;
                        case 4:
                            mapping.FileOffset = cis.ReadUInt64();
                            break;
                        case 5:
                            mapping.FileNameIndex = cis.ReadInt64();
                            break;
                        case 6:
                            mapping.BuildIdIndex = cis.ReadInt64();
                            break;
                        case 7:
                            mapping.HasFunctions = cis.ReadInt32() != 0;
                            break;
                        case 8:
                            mapping.HasFilenames = cis.ReadInt32() != 0;
                            break;
                        case 9:
                            mapping.HasLineNumbers = cis.ReadInt32() != 0;
                            break;
                        case 10:
                            mapping.HasInlineFrames = cis.ReadInt32() != 0;
                            break;
                        default:
                            cis.SkipLastField();
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            return mapping;
        }

        private static Sample HandleSample(CodedInputStream cis)
        {
            var sample = new Sample();
            var lables = new List<Label>();
            sample.Labels = lables;

            var locationIds = new List<ulong>();
            sample.LocationIds = locationIds;

            var values = new List<long>();
            sample.Values = values;

            long pos = cis.ReadLength();
            pos += cis.Position;
            while (pos - cis.Position > 0)
            {
                var tag = cis.ReadTag();
                if (tag > 0)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                    switch (fieldNumber)
                    {
                        case 1:
                        {
                            long posLocations = cis.ReadLength();
                            posLocations += cis.Position;
                            while (posLocations - cis.Position > 0)
                            {
                                locationIds.Add(cis.ReadUInt64());
                            }

                            break;
                        }
                        case 2:
                        {
                            long posValues = cis.ReadLength();
                            posValues += cis.Position;
                            while (posValues - cis.Position > 0)
                            {
                                values.Add(cis.ReadInt64());
                            }

                            break;
                        }
                        case 3:
                            lables.Add(HandleLabel(cis));
                            break;
                        default:
                            cis.SkipLastField();
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            return sample;
        }

        private static Line HandleLine(CodedInputStream cis)
        {
            var line = new Line();

            long pos = cis.ReadLength();
            pos += cis.Position;
            while (pos - cis.Position > 0)
            {
                var tag = cis.ReadTag();
                if (tag > 0)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                    switch (fieldNumber)
                    {
                        case 1:
                            line.FunctionId = cis.ReadUInt64();
                            break;
                        case 2:
                            line.LineNumber = cis.ReadInt64();
                            break;
                        default:
                            cis.SkipLastField();
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            return line;
        }

        private static Location HandleLocation(CodedInputStream cis)
        {
            var location = new Location();

            var lines = new List<Line>();
            location.Lines = lines;

            long pos = cis.ReadLength();
            pos += cis.Position;
            while (pos - cis.Position > 0)
            {
                var tag = cis.ReadTag();
                if (tag > 0)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                    switch (fieldNumber)
                    {
                        case 1:
                            location.Id = cis.ReadUInt64();
                            break;
                        case 2:
                            location.MappingId = cis.ReadUInt64();
                            break;
                        case 3:
                            location.Address = cis.ReadUInt64();
                            break;
                        case 4:
                            lines.Add(HandleLine(cis));
                            break;
                        case 5:
                            location.IsFolded = cis.ReadInt32() != 0;
                            break;
                        default:
                            cis.SkipLastField();
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            return location;
        }

        private static ValueType HandleValueType(CodedInputStream cis)
        {
            var valueType = new ValueType();

            long pos = cis.ReadLength();
            pos += cis.Position;
            while (pos - cis.Position > 0)
            {
                var tag = cis.ReadTag();

                if (tag > 0)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);

                    switch (fieldNumber)
                    {
                        case 1:
                            valueType.Type = cis.ReadInt64();
                            break;
                        case 2:
                            valueType.Unit = cis.ReadInt64();
                            break;
                        default:
                            cis.SkipLastField();
                            break;
                    }
                }
            }

            return valueType;
        }
    }
}