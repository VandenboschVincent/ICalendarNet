BenchMarking
| Method                                             | Mean         | Error        | StdDev       | Gen0      | Gen1      | Gen2      | Allocated   |
|--------------------------------------------------- |-------------:|-------------:|-------------:|----------:|----------:|----------:|------------:|
| BenchMark_Load_And_Serialize_Calendar              | 125,705.2 us |  2,462.16 us |  2,835.43 us |  750.0000 |  250.0000 |         - |   4996.1 KB |
| BenchMark_Load_And_Serialize_all_Calendars         | 270,312.8 us |  3,478.11 us |  2,715.48 us | 2000.0000 |  500.0000 |         - | 14014.64 KB |
| BenchMark_Load_And_Serialize_Events                |     187.7 us |      3.68 us |      5.50 us |    2.9297 |         - |         - |    12.82 KB |
| BenchMark_Load_And_Serialize_Online_Calendar_Async | 921,467.0 us | 24,178.34 us | 64,536.87 us | 8000.0000 | 3000.0000 | 1000.0000 | 48267.38 KB |