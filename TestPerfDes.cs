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

    //[Benchmark]
    //public void DesFlatBufWithoutRec() => Deserialize.FlatBuffWithoutRecRead();

    //[Benchmark]
    //public void DesProto() => Deserialize.ProtobufRead();

    //[Benchmark]
    //public void DesProtoWithoutRec() => Deserialize.ProtobufReadWithoutRec();

    //[Benchmark]
    //public void DesBinReader() => Deserialize.ReadBinRead();

    //[Benchmark]
    //public void DesFileStReader() => Deserialize.ReadFileSt();

    [Benchmark]
    public void DesMarshall() => Deserialize.ReadMarshal();

}
