using Google.FlatBuffers;
using Google.Protobuf;
using LibraryTest1;
using NetEdf.src;
using System.Text;
using static LibraryTest.TestPerfSer;

namespace LibraryTest;

public class Deserialize
{
    private static string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static void PrintToTextStream(TextWriter w, ref OmegaDataForProto read)
    {
        w.Write(read.Time);
        w.Write(';');
        w.Write(read.Press);
        w.Write(';');
        w.Write(read.Temp);
        w.Write(';');
        w.Write(read.Vbat);
        w.Write("\n");
    }

    public static void ProtobufReadWithoutRec()
    {
        using FileStream fs = new FileStream(FilePath + "ProtobufTestWithoutRec.bdf", FileMode.Open, FileAccess.Read);
        using TextWriter w = new StreamWriter(FilePath + "protobufreadWithoutRec.txt");
        using var buffered = new BufferedStream(fs, 64 * 1024);
        var cis = new CodedInputStream(buffered);
        ByteString buffer = null;
        
        while (!cis.IsAtEnd)
        {
            buffer = cis.ReadBytes();
            var item = OmegaDataForProto.Parser.ParseFrom(buffer);
            PrintToTextStream(w, ref item);
        }
    }
    public static void PrintToTextStream(TextWriter w, ref OmegaDataBatch omegaData)
    {
        foreach (var read in omegaData.Records)
        {
            w.Write(read.Time);
            w.Write(';');
            w.Write(read.Press);
            w.Write(';');
            w.Write(read.Temp);
            w.Write(';');
            w.Write(read.Vbat);
            w.Write("\n");
        }
    }
    public static void ProtobufRead()
    {
        using var src = File.OpenRead(FilePath + "ProtobufTest.bdf");
        using TextWriter w = new StreamWriter(FilePath + "protobufread.txt");
        var dataList = OmegaDataBatch.Parser.ParseFrom(src);

        PrintToTextStream(w, ref dataList);
    }
    public static void PrintToTextStream(TextWriter w, ref OmegaData omegaData)
    {
            w.Write(omegaData.Time);
            w.Write(';');
            w.Write(omegaData.Press);
            w.Write(';');
            w.Write(omegaData.Temp);
            w.Write(';');
            w.Write(omegaData.Vbat);
            w.Write("\n");
    }
    public static void FlatBuffWithoutRecRead()
    {
        using FileStream fs = new FileStream(FilePath + "FlactBufWitoutVec.bdf", FileMode.Open, FileAccess.Read);
        using TextWriter w = new StreamWriter(FilePath + "FlactBufWitoutVec.txt");
        byte[] bytes = File.ReadAllBytes(FilePath + "FlactBufWitoutVec.bdf");
        byte[] buf = new byte[36];
     
        ByteBuffer buffer = new ByteBuffer(bytes);
        while (fs.Read(buf, 0, buf.Length) > 0)
        {
            var omegaData = OmegaData.GetRootAsOmegaData(buffer);
            PrintToTextStream(w, ref omegaData);
            buffer.Position += 36;
        }
    }

    public static void PrintToTextStream(TextWriter w, ref OmegaDataList read, ref int len)
    {
        for(int i = 0; i < len; ++i)
        {
            var item = read.Items(i).Value;
            w.Write(item.Time);
            w.Write(';');
            w.Write(item.Press);
            w.Write(';');
            w.Write(item.Temp);
            w.Write(';');
            w.Write(item.Vbat);
            w.Write("\n");
        }
    
    }
    public static void FlatBuffRead()
    {
        using TextWriter w = new StreamWriter(FilePath + "flatBuf.txt");
        byte[] bytes = File.ReadAllBytes(FilePath + "FlatBuffer.bdf");
        var dataList = OmegaDataList.GetRootAsOmegaDataList(new ByteBuffer(bytes));

        int length = dataList.ItemsLength;

        PrintToTextStream(w, ref dataList, ref length);
    }



    public static void EDFRead()
    {
        using var file = new FileStream(FilePath + "EDFTest.bdf", FileMode.Open);
        using var reader = new BinReader(file);

        using TextWriter w = new StreamWriter(FilePath + "EDFRead.txt");

        reader.ReadBlock();
        var rec = reader.ReadInfo();
        OMEGA_DATA_V1_1forEdf read;
        try
        {
            while (reader.ReadBlock() && BlockType.VarData == reader.GetBlockType())
            {
                while (reader.TryRead(out read) == EdfErr.IsOk)
                {
                    PrintToTextStream(w, ref read);
                }
            }
        }
        catch (EndOfStreamException ex)
        {
        }
    }
    public static void PrintToTextStream(TextWriter w, ref OMEGA_DATA_V1_1forEdf read)
    {
        w.Write(read.Time);
        w.Write(';');
        w.Write(read.Press);
        w.Write(';');
        w.Write(read.Temp);
        w.Write(';');
        w.Write(read.Vbat);
        w.Write("\n");
    }
}
