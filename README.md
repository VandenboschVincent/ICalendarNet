BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4291/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.204
  [Host]     : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT AVX2


| Method                                             | Mean           | Error         | StdDev         | Gen0      | Gen1      | Gen2     | Allocated   |
|--------------------------------------------------- |---------------:|--------------:|---------------:|----------:|----------:|---------:|------------:|
| BenchMark_Load_And_Serialize_Calendar              |   6,649.155 us |   128.3272 us |    344.7426 us |  453.1250 |  187.5000 |  15.6250 |  2898.11 KB |
| BenchMark_Load_And_Serialize_Tiny_Calendar         |   5,858.414 us |   115.8786 us |    108.3929 us |  375.0000 |  140.6250 |  23.4375 |   2342.8 KB |
| BenchMark_Load_And_Serialize_all_Calendars         |  13,417.124 us |   264.0735 us |    352.5306 us |  812.5000 |  484.3750 | 156.2500 |  5333.84 KB |
| BenchMark_Load_And_Serialize_Events                |       2.103 us |     0.0294 us |      0.0260 us |    0.5798 |    0.0038 |        - |     3.57 KB |
| BenchMark_Load_And_Serialize_Online_Calendar_Async | 344,424.782 us | 7,159.8950 us | 20,772.1537 us | 3000.0000 | 1000.0000 |        - | 24017.17 KB |