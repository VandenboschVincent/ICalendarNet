


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
    using CalSerializor serializor = new CalSerializor()
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

BenchmarkDotNet v0.13.12, Windows 10
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 8.0.204

```
| SERIALIZATION                                  | Mean          | Error       | StdDev      | Median        | Gen0      | Gen1      | Gen2     | Allocated   |
|---------------------------------------- |--------------:|------------:|------------:|--------------:|----------:|----------:|---------:|------------:|
| DeserializeCalendar                     |     12.656 μs |   0.1432 μs |   0.1195 μs |     12.675 μs |    3.4180 |    0.1221 |        - |    20.94 KB |
| SerializeCalendar                       |      3.129 μs |   0.0623 μs |   0.0612 μs |      3.125 μs |    1.9684 |    0.0267 |        - |    12.06 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      1.406 μs |   0.0280 μs |   0.0444 μs |      1.382 μs |    0.6065 |    0.0019 |        - |     3.73 KB |
| Deserialize_And_Serialize_all_Calendars (150 calendars) |  6,945.098 μs | 136.4880 μs | 195.7470 μs |  6,970.355 μs |  539.0625 |  437.5000 | 125.0000 |  2997.37 KB |
| Deserialize_And_Serialize_Event         |      2.014 μs |   0.0187 μs |   0.0165 μs |      2.008 μs |    0.6523 |         - |        - |     4.01 KB |
| Deserialize_And_Serialize_Big_Calendar (1,078 events) | 30,689.179 μs | 274.9266 μs | 243.7153 μs | 30,682.292 μs | 4062.5000 | 2156.2500 | 906.2500 | 22656.61 KB |
```
When using ICal.Net
```
| SERIALIZATION                                           | Mean          | Error        | StdDev       | Median        | Gen0       | Gen1      | Gen2      | Allocated    |
|------------------------------------------------- |--------------:|-------------:|-------------:|--------------:|-----------:|----------:|----------:|-------------:|
| ICal_Net_DeserializeCalendar                     |      83.09 μs |     2.861 μs |     8.116 μs |      80.33 μs |    30.7617 |    5.3711 |         - |    191.42 KB |
| ICal_Net_SerializeCalendar                       |      23.99 μs |     0.634 μs |     1.819 μs |      23.58 μs |     9.3994 |    0.7324 |         - |     58.03 KB |
| ICal_Net_Deserialize_And_Serialize_all_Calendars |            NA |           NA |           NA |            NA |         NA |        NA |        NA |           NA |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 181,662.18 μs | 3,605.294 μs | 5,613.008 μs | 179,544.67 μs | 22500.0000 | 8000.0000 | 2000.0000 | 131715.69 KB |

Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob
```
## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.