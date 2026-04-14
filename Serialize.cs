using Google.FlatBuffers;
using Google.Protobuf;
using NetEdf.src;
using static LibraryTest.TestPerfSer;
using LibraryTest1;


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

        using var fs = new FileStream(Path.Combine(FilePath,"ProtobufTestWithoutRec.bdf"), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan);
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
        using var write = File.Create(Path.Combine(FilePath,"FlactBufWitoutVec.bdf"));
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
    public static void SerializeEDF(OMEGA_DATA_V1_1[] s)
    {
        var typeRec = new TypeRec()
        {
            Inf = new()
            {
                Type = PoType.Struct,
                Name = "OMEGA_DATA_V1_1",
                Childs =
                [
                    new(PoType.UInt32, "Time"),
                    new(PoType.Int32, "Press"),
                    new(PoType.Int32, "Temp"),
                    new(PoType.UInt32, "Vbat")

                ]
            }
        };
        using (var file = File.Create(Path.Combine(FilePath, "EDFTest.bdf")))
        using (var w = new BinWriter(file))
        {
            w.Write(typeRec);
            foreach (var item in s)
                w.Write(item);
        }
    }

    public static void SerializeBinaryWr(OMEGA_DATA_V1_1[] s)
    {
        using var w = new BinaryWriter(File.Create(Path.Combine(FilePath, "BinaryWriterFile.bdf")));
        for(int i = 0; i < s.Length; i++)
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
        for(int i = 0; i < s.Length; ++i)
        {
            w.Write(BitConverter.GetBytes(s[i].Time));
            w.Write(BitConverter.GetBytes(s[i].Press));
            w.Write(BitConverter.GetBytes(s[i].Temp));
            w.Write(BitConverter.GetBytes(s[i].Vbat));
        }
        w.Close();
    }
}
