using Google.FlatBuffers;
using Google.Protobuf;
using LibraryTest1;
using NetEdf.src;
using NetEdf;
using EdfBinGenerator;
using static LibraryTest.TestPerfSer;


namespace LibraryTest;


public class Serialize
{
    private static string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    //Protobuf
    public static void SerializeProtobuf(OmegaDataV11ForProto[] s)
    {
        var batch = new OmegaDataBatch();
        batch.Records.AddRange(s);

        using (var w = File.Create(Path.Combine(FilePath, "ProtobufTest.bdf")))
        {
            batch.WriteTo(w);
        }
    }

    public static void SerializeProtoWithoutRec(OmegaDataForProto[] s)
    {
        const int bufferSize = 64 * 1024;

        using var fs = new FileStream(Path.Combine(FilePath, "ProtobufTestWithoutRec.bdf"), FileMode.Create,
            FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan);
        using var buffered = new BufferedStream(fs, bufferSize);
        var cos = new CodedOutputStream(buffered);

        foreach (var item in s)
        {
            int size = item.CalculateSize();
            cos.WriteUInt32((uint)size);
            item.WriteTo(cos);

        }

        cos.Flush();
        buffered.Flush();
    }

    //FlatBuffer
    public static void SerializeFlatBufferWithoutVec()
    {
        using var write = File.Create(Path.Combine(FilePath, "FlactBufWitoutVec.bdf"));
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);

        var rnd = new Random();
        for (var i = 0; i < 1000; i++)
        {
            builder.Clear();

            OmegaData.StartOmegaData(builder);
            OmegaData.AddTime(builder, 1);
            OmegaData.AddPress(builder, 2);
            OmegaData.AddTemp(builder, 3);
            OmegaData.AddVbat(builder, 4);
            var endOffset = OmegaData.EndOmegaData(builder);

            OmegaData.FinishOmegaDataBuffer(builder, endOffset);
            var bytes = builder.SizedByteArray();
            write.Write(bytes);
        }
    }

    public static void SerializeFlatBuffer()
    {
        string fileName = Path.Combine(FilePath, "FlatBuffer.bdf");

        var random = new Random();

        var builder = new FlatBufferBuilder(1024);
        var offsets = new Offset<OmegaDataV1_1>[1000];

        for (int i = 0; i < 1000; ++i)
        {
            offsets[i] = OmegaDataV1_1.CreateOmegaDataV1_1(
                builder,
                1,
                2,
                3,
                4);
        }

        var vectorOffset = OmegaDataList.CreateItemsVector(builder, offsets);
        OmegaDataList.StartOmegaDataList(builder);
        OmegaDataList.AddItems(builder, vectorOffset);
        var rootOffset = OmegaDataList.EndOmegaDataList(builder);
        builder.Finish(rootOffset.Value);

        byte[] finalBuffer = builder.DataBuffer.ToSizedArray();
        File.WriteAllBytes(fileName, finalBuffer);
    }

    //EDF
    public static void SerializeEDF(BinWriter.CalcFunc? calc = null
        , BinWriter.CreateEnumeratorFunc? decompose = null)
    {
        TypeInf comlexVarInf = new()
        {
            Type = PoType.Struct,
            Name = "ComplexVariable1",
            Childs =
            [
                new(PoType.Int64, "time"),
                new()
                {
                    Type = PoType.Struct, Name = "State", Dims = [3],
                    Childs =
                    [
                        new(PoType.Int8, "text"),
                        new(PoType.Struct, "Pos")
                        {
                            Childs =
                            [
                                new(PoType.Int32, "x"),
                                new(PoType.Int32, "y"),
                            ]
                        },
                        new(PoType.Double, "Temp", [2, 2]),
                    ]
                }
            ]
        };
        var cv = new ComplexVariable1()
        {
            Time = -123,
            State =
            [
                new()
                {
                    Text = 1, Pos = new() { x = 11, y = 12 }, Temp = new double[2, 2] { { 1.1, 1.2 }, { 1.3, 1.4 } }
                },
                new()
                {
                    Text = 2, Pos = new() { x = 21, y = 22 }, Temp = new double[2, 2] { { 2.1, 2.2 }, { 2.3, 2.4 } }
                },
                new()
                {
                    Text = 3, Pos = new() { x = 31, y = 32 }, Temp = new double[2, 2] { { 3.1, 3.2 }, { 3.3, 3.4 } }
                },
            ]
        };
        using (var file = File.Create(Path.Combine(FilePath, "EDFTest.bdf"))) 
        using (var w = new BinWriter(file)) 
        {
            w.Write(new TypeRec() { Inf = comlexVarInf });
            if (calc is not null)
                w.Calc = calc;
            if(decompose is not null)
                w.CreateEnumerator = decompose;
            for(int i = 0; i < 1000; ++i)
                w.Write(cv);
        }
    }
}

/*public static void SerializeBinaryWr(OMEGA_DATA_V1_1[] s)
{
    using var w = new BinaryWriter(File.Create(Path.Combine(FilePath, "BinaryWriterFile.bdf")));
    for (int i = 0; i < s.Length; i++)
    {
        w.Write(s[i].Time);
        w.Write(s[i].Press);
        w.Write(s[i].Temp);
        w.Write(s[i].Vbat);
    }
    w.Close();
}

public static void SerializeFileWr(OMEGA_DATA_V1_1[] s)
{
    using var w = new FileStream(Path.Combine(FilePath, "FileStream.bdf"), FileMode.Create, FileAccess.Write);
    for (int i = 0; i < s.Length; ++i)
    {
        //var bb = MemoryMarshal.Cast<OMEGA_DATA_V1_1, byte>(s.AsSpan(1));
        //w.Write(bb);
        w.Write(BitConverter.GetBytes(s[i].Time));
        w.Write(BitConverter.GetBytes(s[i].Press));
        w.Write(BitConverter.GetBytes(s[i].Temp));
        w.Write(BitConverter.GetBytes(s[i].Vbat));
    }
    w.Close();
}

public static void SerializeMarshal(OMEGA_DATA_V1_1[] s)
{
    using var w = new FileStream(Path.Combine(FilePath, "Marshall.bdf"), FileMode.Create, FileAccess.Write);
    for (int i = 0; i < s.Length; ++i)
    {
        w.Write(MemoryMarshal.Cast<OMEGA_DATA_V1_1, byte>(s.AsSpan(i, 1)));
    }
    w.Close();
}

public static void SerializeMarshal2(OMEGA_DATA_V1_1[] s)
{
    using var w = new FileStream(Path.Combine(FilePath, "Marshall2.bdf"), FileMode.Create, FileAccess.Write);
    w.Write(MemoryMarshal.Cast<OMEGA_DATA_V1_1, byte>(s.AsSpan()));
}
}
*/
