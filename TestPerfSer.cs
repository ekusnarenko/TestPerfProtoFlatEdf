using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using Google.FlatBuffers;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using LibraryTest1;
using System.Security.Cryptography.X509Certificates;

namespace LibraryTest;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class TestPerfSer
{
   // [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public struct OMEGA_DATA_V1_1forEdf
    {
        public UInt32 Time { get; set; }
        public Int32 Press { get; set; }
        public Int32 Temp { get; set; }
        public UInt32 Vbat { get; set; }
    }


    //[Params(500)]
    public int Count { get; set; } = 1000;
    private OMEGA_DATA_V1_1forEdf[] _dataforEdf;
    private OmegaDataV11ForProto[] _dataforProto;
    private OmegaDataForProto[] _dataProtoWithoutRec;
    

    [GlobalSetup]
    public void Setup()
    {
        _dataforEdf = new OMEGA_DATA_V1_1forEdf[Count];
        _dataforProto = new OmegaDataV11ForProto[Count];
        _dataProtoWithoutRec = new OmegaDataForProto[Count];
      
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        //var random = new Random();
        for (int i = 0; i < Count; ++i)
        {
            var protoWithoutRec = new OmegaDataForProto();
            var proto = new OmegaDataV11ForProto();
            protoWithoutRec.Time = proto.Time = _dataforEdf[i].Time = 0x11;
            protoWithoutRec.Press = proto.Press = _dataforEdf[i].Press = 0x22;
            protoWithoutRec.Temp = proto.Temp = _dataforEdf[i].Temp = 0x33;
            protoWithoutRec.Vbat = proto.Vbat = _dataforEdf[i].Vbat = 0x44;
            _dataforProto[i] = proto;
            _dataProtoWithoutRec[i] = protoWithoutRec;
            //s[i].Time = (uint)random.Next(1, 1000000);
            //s[i].Press = random.Next(1, 200000);
            //s[i].Temp = random.Next(1, 40000);
            //s[i].Vbat = 20000;
        }
    }


    [Benchmark]
    public void SerProtobuf() => Serialize.SerializeProtobuf(_dataforProto);

    [Benchmark]
    public void SerProtobufWithoutRec() => Serialize.SerializeProtoWithoutRec(_dataProtoWithoutRec);

    [Benchmark]
    public void SerFlatBuffer() => Serialize.SerializeFlatBuffer();

    [Benchmark]
    public void SerFlatBuffWithoutRec() => Serialize.SerializeFlatBufferWithoutVec();

    [Benchmark(Baseline = true)]
    public void SerEdf() => Serialize.SerializeEDF(_dataforEdf);

}
