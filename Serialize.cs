using Google.FlatBuffers;
using Google.Protobuf;
using NetEdf.src;
using static LibraryTest.TestPerfSer;
using LibraryTest1;


namespace LibraryTest;


public class Serialize
{

    public static string PathFile = "C:/Users/Katy/Documents/Projects/";

    //Protobuf
    public static void SerializeProtobuf(OmegaDataV11ForProto[] s)
    {
        var batch = new OmegaDataBatch();
        batch.Records.AddRange(s);

        using (var w = File.Create(PathFile + "ProtobufTest.bdf"))
        {
            batch.WriteTo(w);
        }
    }

    public static void SerializeProtoWithoutRec(OmegaDataForProto[] s)
    {
        //var omegaData = new OmegaDataForProto();

        using (var w = File.Create(PathFile + "ProtobufTestWithoutRec.bdf"))
        {
            foreach (var item in s)
            {
                item.WriteDelimitedTo(w);
            }
        }
    }

    //FlatBuffer
    public static void SerializeFlatBufferWithoutVec()
    {
        using var write = File.Create(PathFile + "FlactBufWitoutVec.bdf");
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        for (var i = 0; i < 1000; i++)
        {
            builder.Clear();

            uint time = 0x11;
            int press = 0x22;
            int temp = 0x33;
            uint vbat = 0x44;

            Offset<OmegaData> offset = OmegaData.CreateOmegaData(builder,
                time, press, temp, vbat);

            builder.FinishSizePrefixed(offset.Value);
            var bytes = builder.SizedByteArray();
            write.Write(bytes);
        }
    }
    public static void SerializeFlatBuffer()
    {
        //   string folderPath = @"C:\Users\Katy\Documents\Projects";
        string fileName = Path.Combine(PathFile, "FlatBuffer.bdf");

        var random = new Random();

        var builder = new FlatBufferBuilder(1024);
        var offsets = new Offset<OmegaDataV1_1>[1000];

        for (int i = 0; i < 1000; ++i)
        {
            uint time = 0x11;
            int press = 0x22;
            int temp = 0x33;
            uint vbat = 0x44;

            offsets[i] = OmegaDataV1_1.CreateOmegaDataV1_1(
            builder,
            time,
            press,
            temp,
            vbat);
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
    public static void SerializeEDF(OMEGA_DATA_V1_1forEdf[] s)
    {
        var typeRec = new TypeRec()
        {
            Inf = new()
            {
                Type = PoType.Struct,
                Name = "OMEGA_DATA_V1_1forEdf",
                Childs =
                [
                    new(PoType.UInt32, "Time"),
                    new(PoType.Int32, "Press"),
                    new(PoType.Int32, "Temp"),
                    new(PoType.UInt32, "Vbat")

                ]
            }
        };

        using (var file = File.Create(PathFile + "EDFTest.bdf"))
        using (var w = new BinWriter(file))
        {
            w.Write(typeRec);
            foreach (var item in s)
                w.Write(item);
        }

    }
}
