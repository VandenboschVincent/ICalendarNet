


## ICalendarNet
ICalendarNet is an iCalendar (RFC 5545) class library for .NET aimed at providing RFC 5545 compliance, while providing full compatibility with popular calendaring applications and libraries.
Credits go to [Rianjs](https://github.com/rianjs/ical.net) for providing a ton of info and the baseline of this project.


## Available for
* Net Standard 2.1
* Net 8.0

## Roadmap:

 - [x] Serialization
 - [x] Deserialization
 - [x] RFC 5545 compliancy
 - [ ] Make it easier to create/edit occurency
 - [ ] Make it easier to create/edit alarms

## How to use:

dotnet add packages [ICalendarNet](https://www.nuget.org/packages/ICalendarNet)

How to deserialize an get events
```csharp
using var httpClient = new HttpClient();
string icalvar = await httpClient.GetStringAsync("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10");

Calendar? calendar = calSerializor.DeserializeCalendar(icalvar);
CalendarEvent calEvent = calendar.GetEvents().First();
```

Hot to create a new calendar and serialize
```csharp
private static string SimpleCalendar()
{
    Calendar calendar = new Calendar();
    CalSerializor serializor = new CalSerializor();
    //Add an event
    CalendarEvent calendarEvent = new CalendarEvent()
    {
        DTSTART = DateTimeOffset.UtcNow,
        DTEND = DateTimeOffset.UtcNow.AddHours(1),
        Location = "The Exceptionally Long Named Meeting Room",
        Priority = 0
    };
    calendarEvent.SetAttachments(new List<CalendarAttachment>()
    {
        //Add url attachment
        new CalendarAttachment(new Uri("ldap://example.com:3333/o=eExample Industries,c=3DUS??(cn=3DBJohn Smith)"), ""),
        //Add byte attachment
        new CalendarAttachment(Encoding.UTF8.GetBytes(""), "application/msword")
    });
    calendar.SubComponents.Add(calendarEvent);
    //Add an alarm
    calendar.SubComponents.Add(
       //Display info when triggered
       new CalendarAlarm(trigger: new CalendarTrigger(TimeSpan.FromMinutes(-108)),
           notification: "Reminder water plants"));
   calendar.SubComponents.Add(
       //Send an email when triggered
       new CalendarAlarm(trigger: new CalendarTrigger(DateTime.Now.AddYears(1)),
           emailAdresses: ["test@gmail.com"],
           subject: "Test email subject",
           body: @"Dear John,

				   Please water the plants.

				   Regards
				   Mr Smith"));
    //Add a t_odo
    calendar.SubComponents.Add(
        new CalendarTodo()
        {
            DTSTART = DateTimeOffset.UtcNow,
            Completed = DateTimeOffset.UtcNow.AddHours(1),
            Location = "The Exceptionally Long Named Meeting Room"
        });
    //Add a freebusy
    calendar.SubComponents.Add(
        new CalendarFreeBusy()
        {
            DTSTART = DateTimeOffset.UtcNow,
            DTEND = DateTimeOffset.UtcNow.AddHours(1),
        });
    return serializor.SerializeCalendar(calendar);
}
```

## Benchmarking

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8037/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5900HX with Radeon Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104

```

| Method                                  | Mean          | Error       | StdDev      | Gen0      | Gen1      | Gen2     | Allocated  |
|---------------------------------------- |--------------:|------------:|------------:|----------:|----------:|---------:|-----------:|
| DeserializeCalendar                     |     21.726 μs |   0.4394 μs |   0.4512 μs |    2.5330 |    0.0916 |        - |   20.78 KB |
| SerializeCalendar                       |      5.991 μs |   0.1186 μs |   0.1981 μs |    1.9760 |         - |        - |   16.17 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      2.023 μs |   0.0390 μs |   0.0507 μs |    0.4463 |         - |        - |    3.67 KB |
| Deserialize_And_Serialize_150_Calendars |  8,596.719 μs | 157.1398 μs | 204.3261 μs |  421.8750 |  328.1250 | 125.0000 | 2933.21 KB |
| Deserialize_And_Serialize_Event         |      3.448 μs |   0.0684 μs |   0.0936 μs |    0.4883 |         - |        - |    4.02 KB |
| Deserialize_And_Serialize_Big_Calendar  | 12,902.920 μs | 249.4064 μs | 332.9504 μs | 1359.3750 | 1265.6250 | 453.1250 | 9279.69 KB |



```
When using ICal.Net
```

| Method                                           | Mean         | Error        | StdDev       | Gen0      | Gen1      | Gen2     | Allocated   |
|------------------------------------------------- |-------------:|-------------:|-------------:|----------:|----------:|---------:|------------:|
| ICal_Net_DeserializeCalendar                     |     83.41 μs |     1.630 μs |     2.002 μs |   19.5313 |    2.4414 |        - |   160.66 KB |
| ICal_Net_SerializeCalendar                       |     20.59 μs |     0.377 μs |     0.403 μs |    5.2490 |    0.2441 |        - |    43.04 KB |
| ICal_Net_Deserialize_And_Serialize_150_Calendars |           NA |           NA |           NA |        NA |        NA |       NA |          NA |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 65,557.41 μs | 1,289.293 μs | 2,514.667 μs | 7500.0000 | 2000.0000 | 750.0000 | 58389.96 KB |



Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob
```
## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.