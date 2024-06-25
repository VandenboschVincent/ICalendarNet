BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4291/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.204
  [Host]     : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.4 (8.0.424.16909), X64 RyuJIT AVX2

| Method                                              | Mean          | Error         | StdDev        | Median        | Gen0      | Gen1      | Gen2     | Allocated   |
|---------------------------------------------------- |--------------:|--------------:|--------------:|--------------:|----------:|----------:|---------:|------------:|
| BenchMark_Load_And_Serialize_Calendar               |    514.263 us |    10.1807 us |    24.5875 us |    511.290 us |  103.5156 |   41.0156 |        - |   640.03 KB |
| BenchMark_Load_And_Serialize_Tiny_Calendar          |      2.232 us |     0.3319 us |     0.9251 us |      1.867 us |    0.6084 |    0.0019 |        - |     3.74 KB |
| BenchMark_Load_And_Serialize_all_Calendars          |  6,679.383 us |   128.3720 us |   171.3729 us |  6,668.860 us |  539.0625 |  437.5000 | 125.0000 |  2996.33 KB |
| BenchMark_Load_And_Serialize_Events                 |      3.529 us |     0.4480 us |     1.1803 us |      3.164 us |    0.6523 |    0.0038 |        - |     4.02 KB |
| BenchMark_Load_And_Serialize_American_Awerness_Days | 38,598.039 us | 1,385.1851 us | 4,040.6501 us | 37,832.762 us | 3769.2308 | 1769.2308 | 615.3846 | 22656.07 KB |

How to use:
`using var httpClient = new HttpClient();
string icalvar = await httpClient.GetStringAsync(icalString);

Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);
CalendarEvent calEvent = calendar.GetEvents().First();`