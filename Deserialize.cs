using Google.FlatBuffers;
using Google.Protobuf;
using LibraryTest1;
using NetEdf.src;
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
        using FileStream fs = new FileStream(Path.Combine(FilePath,"ProtobufTestWithoutRec.bdf"), FileMode.Open, FileAccess.Read);
        using TextWriter w = new StreamWriter(Path.Combine(FilePath, "protobufreadWithoutRec.txt"));
        using var buffered = new BufferedStream(fs, 64 * 1024);
        var cis = new CodedInputStream(buffered);
        ByteString buffer = null;
        int check = 0;
        while (!cis.IsAtEnd)
        {
            buffer = cis.ReadBytes();
            var item = OmegaDataForProto.Parser.ParseFrom(buffer);
            PrintToTextStream(w, ref item);
            check += (item.Time == 1 && item.Press == 2 && item.Temp == 3 && item.Vbat == 4) ? 0 : 1;
        }
        if (check > 1)
            Console.WriteLine($"error count={check}");
    }
    public static void PrintToTextStream(TextWriter w, ref OmegaDataBatch omegaData, int check)
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
            check += (read.Time == 1 && read.Press == 2 && read.Temp == 3 && read.Vbat == 4) ? 0 : 1;
        }
    }
    public static void ProtobufRead()
    {
        using var src = File.OpenRead(Path.Combine(FilePath, "ProtobufTest.bdf"));
        using TextWriter w = new StreamWriter(Path.Combine(FilePath, "protobufread.txt"));
        var dataList = OmegaDataBatch.Parser.ParseFrom(src);
        int check = 0;
        PrintToTextStream(w, ref dataList, check);
        if (check > 1)
            Console.WriteLine($"error count={check}");
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
        using FileStream fs = new FileStream(Path.Combine(FilePath,"FlactBufWitoutVec.bdf"), FileMode.Open, FileAccess.Read);
        using TextWriter w = new StreamWriter(Path.Combine(FilePath, "FlactBufWitoutVec.txt"));
        byte[] bytes = File.ReadAllBytes(Path.Combine(FilePath, "FlactBufWitoutVec.bdf"));
        byte[] buf = new byte[36];
        int check = 0;
        ByteBuffer buffer = new ByteBuffer(bytes);
        while (fs.Read(buf, 0, buf.Length) > 0)
        {
            var omegaData = OmegaData.GetRootAsOmegaData(buffer);
            PrintToTextStream(w, ref omegaData);
            buffer.Position += 36;
            check += (omegaData.Time == 1 && omegaData.Press == 2 && omegaData.Temp == 3 && omegaData.Vbat == 4) ? 0 : 1;
        }
        if (check > 1)
            Console.WriteLine($"error count={check}");
    }

    public static void PrintToTextStream(TextWriter w, ref OmegaDataList read, ref int len, int check)
    {
        for (int i = 0; i < len; ++i)
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
            check += (item.Time == 1 && item.Press == 2 && item.Temp == 3 && item.Vbat == 4) ? 0 : 1;
        }

    }
    public static void FlatBuffRead()
    {
        using TextWriter w = new StreamWriter(Path.Combine(FilePath,"flatBuf.txt"));
        byte[] bytes = File.ReadAllBytes(Path.Combine(FilePath, "FlatBuffer.bdf"));
        var dataList = OmegaDataList.GetRootAsOmegaDataList(new ByteBuffer(bytes));

        int length = dataList.ItemsLength;
        int check = 0;
        PrintToTextStream(w, ref dataList, ref length, check);
        if(check > 1)
            Console.WriteLine($"error count={check}");
    }



    public static void EDFRead()
    {
        using var file = new FileStream(Path.Combine(FilePath, "EDFTest.bdf"), FileMode.Open);
        using var reader = new BinReader(file);

        using TextWriter w = new StreamWriter(Path.Combine(FilePath, "EDFRead.txt"));

        int check = 0;

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
                    check += (read.Time == 1 && read.Press == 2 && read.Temp == 3 && read.Vbat == 4) ? 0 : 1;
                }
            }
        }
        catch (EndOfStreamException ex)
        {
        }
        if(check > 1)
            Console.WriteLine($"error count={check}");
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
