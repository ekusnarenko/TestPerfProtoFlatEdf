using BenchmarkDotNet.Attributes;

namespace LibraryTest;

[MemoryDiagnoser]
//[SimpleJob(RuntimeMoniker.HostProcess)]
public class TestPerfDes
{
    //[Benchmark(Baseline = true)]
    //public void DesEdf() => Deserialize.EDFRead();

    //[Benchmark]
    //public void DesFlatBuf() => Deserialize.FlatBuffRead();

    [Benchmark]
    public void DesProto() => Deserialize.ProtobufRead();

    [Benchmark]
    public void DesProtoWithoutRec() => Deserialize.ProtobufReadWithoutRec();

}
