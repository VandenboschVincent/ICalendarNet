


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

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5011/22H2/2022Update)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.100-rc.1.24452.12

```
| SERIALIZATION                                  | Mean          | Error       | StdDev      | Median        | Gen0      | Gen1      | Gen2     | Allocated   |
|---------------------------------------- |--------------:|------------:|------------:|--------------:|----------:|----------:|---------:|------------:|
| DeserializeCalendar                     |     13.668 μs |   0.2459 μs |   0.4797 μs |     13.491 μs |    3.3875 |    0.1221 |        - |    20.89 KB |
| SerializeCalendar                       |      5.697 μs |   0.1136 μs |   0.2849 μs |      5.666 μs |    2.8381 |    0.0534 |        - |    17.39 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      1.639 μs |   0.0299 μs |   0.0378 μs |      1.631 μs |    0.6065 |    0.0019 |        - |     3.73 KB |
| Deserialize_And_Serialize_all_Calendars (150 calendars) |  7,384.326 μs | 143.6299 μs | 176.3904 μs |  7,337.092 μs |  531.2500 |  429.6875 | 125.0000 |  2986.76 KB |
| Deserialize_And_Serialize_Event         |      2.305 μs |   0.0450 μs |   0.0701 μs |      2.266 μs |    0.6523 |    0.0038 |        - |     4.01 KB |
| Deserialize_And_Serialize_Big_Calendar (1,078 events) | 24,471.842 μs | 488.4794 μs | 986.7525 μs | 24,261.273 μs | 3406.2500 | 1656.2500 | 562.5000 | 20489.48 KB |

```
When using ICal.Net
```
| SERIALIZATION                                           | Mean          | Error        | StdDev       | Gen0       | Gen1      | Gen2      | Allocated    |
|------------------------------------------------- |--------------:|-------------:|-------------:|-----------:|----------:|----------:|-------------:|
| ICal_Net_DeserializeCalendar                     |      79.87 μs |     0.707 μs |     0.626 μs |    30.7617 |    5.3711 |         - |    191.42 KB |
| ICal_Net_SerializeCalendar                       |      22.93 μs |     0.428 μs |     0.783 μs |     9.2773 |    0.7324 |         - |      57.4 KB |
| ICal_Net_Deserialize_And_Serialize_all_Calendars |            NA |           NA |           NA |         NA |        NA |        NA |           NA |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 174,481.37 μs | 3,461.040 μs | 6,831.756 μs | 20500.0000 | 6500.0000 | 1500.0000 | 121901.07 KB |

Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob
```
## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.