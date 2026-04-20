using System.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Google.FlatBuffers;
using NetEdf.src;
using EdfBinGenerator;

namespace LibraryTest;

//[NetEdf.DecomposeGenerator]
public partial struct OMEGA_DATA_V1_1
{
    public UInt32 Time { get; set; }
    public UInt32 Press { get; set; }
    public UInt32 Temp { get; set; }
    public UInt32 Vbat { get; set; }
    
    public IEnumerable<object> Decompose()
    {
        yield return Time;
        yield return Press;
        yield return Temp;
        yield return Vbat;
    }
}


[NetEdf.DecomposeGenerator]
partial class ComplexVariable1
{
    public long Time { get; set; }
    [NetEdf.DecomposeGenerator]
    public partial class StateT
    {
        public sbyte Text { get; set; }
        [NetEdf.DecomposeGenerator]
        public partial class PosT
        {
            public int x { get; set; }
            public int y { get; set; }
        };
        public PosT Pos { get; set; }
        public double[,] Temp { get; set; }
    };
    public StateT[] State { get; set; }
    
    public IEnumerable<object> Decompose1()
    {
        yield return Time;
        foreach (var item1 in State)
        {
            yield return item1.Text;
            yield return item1.Pos.x;
            yield return item1.Pos.y;
            foreach (var item2 in item1.Temp)
            {
                yield return item2;
            }
        }
    }
};

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]

public class TestPerfSer
{
    // [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    //[Params(500)]
    public int Count { get; set; } = 1000;
   // private OMEGA_DATA_V1_1[] _data;
    private OmegaDataV11ForProto[] _dataforProto;
    private OmegaDataForProto[] _dataProtoWithoutRec;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random();
     //   _data = new OMEGA_DATA_V1_1[Count];
        _dataforProto = new OmegaDataV11ForProto[Count];
        _dataProtoWithoutRec = new OmegaDataForProto[Count];

        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        for (int i = 0; i < Count; ++i)
        {
            var protoWithoutRec = new OmegaDataForProto();
            var proto = new OmegaDataV11ForProto();
            protoWithoutRec.Time = proto.Time = (uint)1;//_data[i].Time = 
            protoWithoutRec.Press = proto.Press = 2;
            protoWithoutRec.Temp = proto.Temp = 3;
            protoWithoutRec.Vbat = proto.Vbat = (uint)4;//_data[i].Vbat = 
            _dataforProto[i] = proto;
            _dataProtoWithoutRec[i] = protoWithoutRec;
            //_data[i].Press = (uint)2;
            //_data[i].Temp = (uint)3;
        }
    }


    // [Benchmark]
    // public void SerProtobuf() => Serialize.SerializeProtobuf(_dataforProto);
    //
    // [Benchmark]
    // public void SerProtobufWithoutRec() => Serialize.SerializeProtoWithoutRec(_dataProtoWithoutRec);
    //
    // [Benchmark]
    // public void SerFlatBuffer() => Serialize.SerializeFlatBuffer();
    //
    // [Benchmark]
    // public void SerFlatBuffWithoutRec() => Serialize.SerializeFlatBufferWithoutVec();
    
    [Benchmark(Baseline = true)]
    //[Benchmark]
    public void SerEdf() => Serialize.SerializeEDF();

    
    [Benchmark]
    public void SerEdf_nonCRC()
    {
        Serialize.SerializeEDF((sp, acc) => 0);
    }
    
    [Benchmark]
    public void SerEdf_nonCRC_nonDecompose()
    {
        IEnumerator<object> MyDecomposer(object obj)
        {
            var data = (ComplexVariable1)obj;
            return (new object[]
            { 
                data.Time, 
                data.State[0].Text, 
                data.State[0].Pos.x,
                data.State[0].Pos.y, 
                data.State[0].Temp[0,0], data.State[0].Temp[0,1], 
                data.State[0].Temp[1,0], data.State[0].Temp[1,1],
    
                data.State[1].Text,
                data.State[1].Pos.x, 
                data.State[1].Pos.y,
                data.State[1].Temp[0,0], data.State[1].Temp[0,1], 
                data.State[1].Temp[1,0], data.State[1].Temp[1,1],
                
                data.State[2].Text,
                data.State[2].Pos.x, 
                data.State[2].Pos.y,
                data.State[2].Temp[0,0], data.State[2].Temp[0,1], 
                data.State[2].Temp[1,0], data.State[2].Temp[1,1]
            }
            .AsEnumerable().GetEnumerator());
        }
        ;
        Serialize.SerializeEDF( (sp, acc) => 0, MyDecomposer);
    }
    
    [Benchmark]
    public void SerEdfDecomGen()
    {
        IEnumerator<object> MyDecomposer(object obj)
        {
            return ((ComplexVariable1)obj).Decompose().AsEnumerable().GetEnumerator();
        } 
        Serialize.SerializeEDF((sp, acc) => 0, MyDecomposer);
        
    }
    // [Benchmark]
    // public void SerEdfDecomGen2()
    // {
    //     IEnumerator<object> MyDecomposer2(object obj)
    //     {
    //         return ((ComplexVariable1)obj).Decompose().GetEnumerator();
    //     }
    //     Serialize.SerializeEDF( (sp, acc) => 0, MyDecomposer2);
    // }

    
    // [Benchmark]
    // public void SerBinaryWr() => Serialize.SerializeBinaryWr(_data);

    // [Benchmark]
    // public void SerStreamWr() => Serialize.SerializeFileWr(_data);
    //
    // [Benchmark]
    // public void SerMarshall() => Serialize.SerializeMarshal(_data);
    //
    // [Benchmark]
    // public void SerMarshall2() => Serialize.SerializeMarshal2(_data);

}