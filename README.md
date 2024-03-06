BenchMarking

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
AMD Ryzen 9 5900HX with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

| Method                                             | Mean          | Error         | StdDev        | Median        | Gen0      | Gen1      | Gen2     | Allocated   |
|--------------------------------------------------- |--------------:|--------------:|--------------:|--------------:|----------:|----------:|---------:|------------:|
| BenchMark_Load_And_Serialize_Calendar              |  22,596.07 us |    110.650 us |    103.502 us |  22,568.01 us |  562.5000 |  312.5000 |  62.5000 |  5051.86 KB |
| BenchMark_Load_And_Serialize_Tiny_Calendar         |  17,961.78 us |    139.014 us |    130.034 us |  17,922.72 us |  250.0000 |  125.0000 |        - |  2356.38 KB |
| BenchMark_Load_And_Serialize_all_Calendars         |  34,388.77 us |    693.686 us |    994.864 us |  34,718.89 us | 1562.5000 |  937.5000 | 250.0000 | 13944.39 KB |
| BenchMark_Load_And_Serialize_Events                |      11.25 us |      0.119 us |      0.105 us |      11.27 us |    1.6785 |    0.0153 |        - |     13.8 KB |
| BenchMark_Load_And_Serialize_Online_Calendar_Async | 466,155.39 us | 15,616.878 us | 42,221.139 us | 453,799.80 us | 3000.0000 | 1000.0000 |        - | 32255.41 KB |