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
        var rnd = new Random();
        _dataforEdf = new OMEGA_DATA_V1_1forEdf[Count];
        _dataforProto = new OmegaDataV11ForProto[Count];
        _dataProtoWithoutRec = new OmegaDataForProto[Count];

        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        for (int i = 0; i < Count; ++i)
        {
            var protoWithoutRec = new OmegaDataForProto();
            var proto = new OmegaDataV11ForProto();
            protoWithoutRec.Time = proto.Time = _dataforEdf[i].Time = (uint)rnd.Next(1, 100);
            protoWithoutRec.Press = proto.Press = _dataforEdf[i].Press = rnd.Next(1, 100);
            protoWithoutRec.Temp = proto.Temp = _dataforEdf[i].Temp = rnd.Next(1, 100);
            protoWithoutRec.Vbat = proto.Vbat = _dataforEdf[i].Vbat = (uint)rnd.Next(1, 100);
            _dataforProto[i] = proto;
            _dataProtoWithoutRec[i] = protoWithoutRec;
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