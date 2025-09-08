


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

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.5768/23H2/2023Update/SunValley3)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.304

```
| SERIALIZATION                                  | Mean         | Error       | StdDev      | Median       | Gen0      | Gen1      | Gen2     | Allocated  |
|---------------------------------------- |-------------:|------------:|------------:|-------------:|----------:|----------:|---------:|-----------:|
| DeserializeCalendar                     |    14.718 μs |   0.2912 μs |   0.5176 μs |    14.590 μs |    3.4027 |    0.1221 |        - |   20.89 KB |
| SerializeCalendar                       |     5.971 μs |   0.1762 μs |   0.5111 μs |     5.850 μs |    2.7847 |    0.0534 |        - |   17.07 KB |
| Deserialize_And_Serialize_Tiny_Calendar |     1.851 μs |   0.0408 μs |   0.1182 μs |     1.828 μs |    0.6065 |         - |        - |    3.73 KB |
| Deserialize_And_Serialize_all_Calendars (150 calendars) | 7,926.388 μs | 154.5271 μs | 407.0861 μs | 7,829.672 μs |  531.2500 |  421.8750 | 125.0000 | 2986.98 KB |
| Deserialize_And_Serialize_Event         |     2.619 μs |   0.0789 μs |   0.2251 μs |     2.549 μs |    0.6523 |         - |        - |    4.01 KB |
| Deserialize_And_Serialize_Big_Calendar (1,078 events)  | 9,718.046 μs | 188.8740 μs | 325.7984 μs | 9,713.112 μs | 1578.1250 | 1421.8750 | 437.5000 | 8781.35 KB |


```
When using ICal.Net
```
| SERIALIZATION                                           | Mean         | Error      | StdDev     | Median       | Gen0      | Gen1      | Gen2     | Allocated   |
|------------------------------------------------- |-------------:|-----------:|-----------:|-------------:|----------:|----------:|---------:|------------:|
| ICal_Net_DeserializeCalendar                     |     85.40 μs |   2.965 μs |   8.603 μs |     82.64 μs |   26.6113 |    3.5400 |        - |    163.3 KB |
| ICal_Net_SerializeCalendar                       |     20.56 μs |   0.540 μs |   1.559 μs |     20.53 μs |    7.0801 |    0.2441 |        - |    43.44 KB |
| ICal_Net_Deserialize_And_Serialize_all_Calendars |           NA |         NA |         NA |           NA |        NA |        NA |       NA |          NA |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 57,538.05 μs | 856.205 μs | 759.004 μs | 57,759.25 μs | 9500.0000 | 2000.0000 | 750.0000 | 56450.86 KB |



Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob
```
## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.