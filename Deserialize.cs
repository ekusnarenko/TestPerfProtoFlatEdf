using Google.FlatBuffers;
using NetEdf.src;
using System.Text;
using static LibraryTest.TestPerfSer;

namespace LibraryTest;

public class Deserialize
{
    public static void WriteToFile<T>(IEnumerable<T> ls, string fileName)
    {
        string path = "C:/Users/Katy/Documents/Projects/";

        using (var w = new StreamWriter(path + fileName))
        {
            foreach (var item in ls)
            {
                w.WriteLine(item);
            }
        }

    }

    public static void ProtobufReadWithoutRec()
    {
        //var omegaData = new OmegaDataV11ForProto[1000];
        using var src = File.OpenRead(@"C:\Users\Katy\Documents\Projects\ProtobufTestWithoutRec.bdf");
        using TextWriter w = new StreamWriter("C:/Users/Katy/Documents/Projects/protobufreadWithoutRec.txt");

        OmegaDataV11ForProto? read = null;
        //    byte[] buf = new byte[16];
        //  OmegaDataV11ForProto[] ms = null;
        while (src.Length > src.Position)
        {
            read = OmegaDataV11ForProto.Parser.ParseDelimitedFrom(src);
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
        using var src = File.OpenRead(@"C:\Users\Katy\Documents\Projects\ProtobufTest.bdf");
        using TextWriter w = new StreamWriter("C:/Users/Katy/Documents/Projects/protobufread.txt");
        //int i = 0;        
        var dataList = OmegaDataBatch.Parser.ParseFrom(src);

       // WriteToFile(dataList.Records, "protobufread.txt");
        foreach (var read in dataList.Records)
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
    public static void FlatBuffRead()
    {
        byte[] bytes = File.ReadAllBytes(@"C:\Users\Katy\Documents\Projects\FlatBuffer.bdf");

        var dataList = OmegaDataList.GetRootAsOmegaDataList(new ByteBuffer(bytes));

        int length = dataList.ItemsLength;

        List<(UInt32, Int32, Int32, UInt32)> structure = new();

        for (int i = 0; i < length; ++i)
        {
            var item = dataList.Items(i).Value;

            var time = item.Time;
            var press = item.Press;
            var temp = item.Temp;
            var Vbat = item.Vbat;

            structure.Add((time, press, temp, Vbat));
        }

        WriteToFile(structure, "flatBuf.txt");
    }



    public static void EDFRead()
    {
        using var file = new FileStream(@"C:\Users\Katy\Documents\Projects\EDFTest.bdf", FileMode.Open);
        using var reader = new BinReader(file);

        using TextWriter w = new StreamWriter("C:/Users/Katy/Documents/Projects/EDFRead.txt");

        reader.ReadBlock();
        var rec = reader.ReadInfo();
        //List<(UInt32, Int32, Int32, UInt32)>? arrRead = new();
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
            //Console.WriteLine($"file end msg={ex}");
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
