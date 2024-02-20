BenchMarking
| Method                                             | Mean         | Error        | StdDev       | Median       | Gen0      | Gen1      | Gen2      | Allocated   |
|--------------------------------------------------- |-------------:|-------------:|-------------:|-------------:|----------:|----------:|----------:|------------:|
| BenchMark_Load_And_Serialize_Calendar              | 123,398.5 us |  2,404.50 us |  3,126.53 us | 121,995.7 us | 1000.0000 |  250.0000 |         - |  4996.23 KB |
| BenchMark_Load_And_Serialize_all_Calendars         | 264,974.6 us |  4,648.41 us | 10,300.55 us | 262,089.6 us | 2500.0000 |  500.0000 |         - | 15377.68 KB |
| BenchMark_Load_And_Serialize_Events                |     180.3 us |      3.58 us |      6.28 us |     178.2 us |    2.9297 |         - |         - |    12.82 KB |
| BenchMark_Load_And_Serialize_Online_Calendar_Async | 910,194.4 us | 19,184.46 us | 51,866.30 us | 893,859.6 us | 8000.0000 | 3000.0000 | 1000.0000 | 48262.73 KB |