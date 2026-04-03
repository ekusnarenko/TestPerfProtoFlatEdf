using BenchmarkDotNet.Running;
using NetEdf.src;

namespace LibraryTest;

internal class Program
{

    static void Main(string[] args)
    {
        /*
          var config = ManualConfig.CreateEmpty()
              .AddLogger(ConsoleLogger.Default) // Вывод в консоль
              .AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray())
              .AddExporter(DefaultConfig.Instance.GetExporters().ToArray())
              .AddJob(Job.Default
                  .WithRuntime(BenchmarkDotNet.Environments.CoreRuntime.Core10_0) 
                  .WithId("JitJob")
      );
          */
        var serialize = BenchmarkRunner.Run<TestPerfSer>();


        /*
         | Method        | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
         |-------------- |---------:|--------:|--------:|------:|--------:|--------:|-------:|----------:|------------:|
         | SerProtobuf   | 254.8 us | 4.93 us | 6.91 us |  0.38 |    0.01 |  1.9531 |      - |  16.27 KB |        0.03 |
         | SerFlatBuffer | 269.0 us | 2.74 us | 2.29 us |  0.41 |    0.01 | 10.7422 | 0.9766 |  91.15 KB |        0.14 |
         | SerEdf        | 663.5 us | 9.05 us | 7.56 us |  1.00 |    0.02 | 78.1250 | 0.9766 | 642.29 KB |        1.00 |
        */
        /*
         | Method                | Mean     | Error   | StdDev   | Median   | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
         |---------------------- |---------:|--------:|---------:|---------:|------:|--------:|---------:|--------:|-----------:|------------:|
         | SerProtobuf           | 251.3 us | 4.93 us |  4.61 us | 250.3 us |  0.40 |    0.01 |   1.9531 |       - |   16.51 KB |        0.03 |
         | SerProtobufWithoutRec | 448.3 us | 8.90 us | 20.97 us | 439.6 us |  0.71 |    0.03 | 500.0000 | 15.6250 | 4090.59 KB |        6.37 |
         | SerFlatBuffer         | 259.3 us | 3.71 us |  3.29 us | 258.9 us |  0.41 |    0.01 |  10.7422 |  0.9766 |   91.27 KB |        0.14 |
         | SerFlatBuffWithoutRec | 349.7 us | 6.43 us |  6.02 us | 348.7 us |  0.55 |    0.01 |   8.3008 |       - |   68.37 KB |        0.11 |
         | SerEdf                | 630.6 us | 7.92 us |  7.41 us | 627.4 us |  1.00 |    0.02 |  78.1250 |  0.9766 |   642.5 KB |        1.00 |
        */

        //var deserialize = BenchmarkRunner.Run<TestPerfDes>();

        /*
                | Method     | Mean     | Error   | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
                |----------- |---------:|--------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
                | DesEdf     | 732.4 us | 6.09 us |  5.40 us |  1.00 |    0.01 | 48.8281 | 3.9063 | 411.84 KB |        1.00 |
                | DesFlatBuf | 471.3 us | 5.60 us |  4.97 us |  0.64 |    0.01 | 29.2969 | 3.9063 | 245.16 KB |        0.60 |
                | DesProto   | 498.9 us | 9.89 us | 11.00 us |  0.68 |    0.02 | 34.1797 | 3.9063 | 281.24 KB |        0.68 |
        */

        //using var convert = new BinToTxtConverter("C:/Users/Katy/Documents/Projects/EDFTest.bdf", "C:/Users/Katy/Documents/Projects/ConvertEDF.tdf");
        //convert.Execute();
    }
}
