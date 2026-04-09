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
         | Method                | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
         |---------------------- |---------:|--------:|--------:|------:|--------:|--------:|-------:|----------:|------------:|
         | SerProtobuf           | 249.6 us | 4.30 us | 6.57 us |  0.40 |    0.01 |  1.9531 |      - |  16.51 KB |        0.03 |
         | SerProtobufWithoutRec | 231.7 us | 2.23 us | 2.08 us |  0.37 |    0.00 | 16.1133 |      - | 132.71 KB |        0.21 |
         | SerFlatBuffer         | 255.2 us | 4.38 us | 4.10 us |  0.40 |    0.01 | 10.7422 | 0.9766 |  91.27 KB |        0.14 |
         | SerFlatBuffWithoutRec | 345.6 us | 6.14 us | 9.19 us |  0.55 |    0.02 |  8.3008 |      - |  68.44 KB |        0.11 |
         | SerEdf                | 631.4 us | 8.09 us | 6.32 us |  1.00 |    0.01 | 78.1250 | 0.9766 |  642.5 KB |        1.00   
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

         var deserialize = BenchmarkRunner.Run<TestPerfDes>();

        /*
        | Method               | Mean     | Error   | StdDev  | Ratio | Gen0    | Gen1   | Allocated | Alloc Ratio |
        |--------------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
        | DesEdf               | 694.0 us | 6.78 us | 6.01 us |  1.00 | 23.4375 |      - | 199.77 KB |        1.00 |
        | DesFlatBuf           | 428.9 us | 3.97 us | 3.32 us |  0.62 |  3.9063 |      - |  33.21 KB |        0.17 |
        | DesFlatBufWithoutRec | 572.6 us | 6.69 us | 5.93 us |  0.83 |  5.8594 |      - |  49.34 KB |        0.25 |
        | DesProto             | 432.7 us | 5.76 us | 5.10 us |  0.62 |  7.8125 | 0.9766 |  69.25 KB |        0.35 |
        | DesProtoWithoutRec   | 393.0 us | 2.08 us | 1.94 us |  0.57 | 41.0156 | 9.7656 | 337.06 KB |        1.69 |
         */
        /*
        | Method               | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1    | Allocated  | Alloc Ratio |
        |--------------------- |---------:|---------:|---------:|------:|--------:|---------:|--------:|-----------:|------------:|
        | DesEdf               | 676.4 us |  9.19 us |  8.15 us |  1.00 |    0.02 |  23.4375 |       - |  199.77 KB |        1.00 |
        | DesFlatBuf           | 483.6 us |  9.42 us | 16.74 us |  0.72 |    0.03 |   3.9063 |       - |   33.21 KB |        0.17 |
        | DesFlatBufWithoutRec | 621.5 us | 10.54 us |  8.80 us |  0.92 |    0.02 |   5.8594 |       - |   49.34 KB |        0.25 |
        | DesProto             | 492.0 us |  9.70 us | 11.92 us |  0.73 |    0.02 |   7.8125 |  0.9766 |   69.25 KB |        0.35 |
        | DesProtoWithoutRec   | 956.0 us | 18.35 us | 24.50 us |  1.41 |    0.04 | 523.4375 | 35.1563 | 4279.97 KB |       21.42 |
        */

    }
}
