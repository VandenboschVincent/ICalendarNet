


## ICalendarNet
ICalendarNet is an iCalendar (RFC 5545) class library for .NET aimed at providing RFC 5545 compliance, while providing full compatibility with popular calendaring applications and libraries.
Credits go to [Rianjs](https://github.com/rianjs/ical.net) for providing a ton of info and the baseline of this project.


## Available for
* Net Standard 2.1
* Net net5.0/net6.0/net7.0/net8.0/net9.0/net10.0;
(please use net8.0 or highter)

## Roadmap:

 - [x] Serialization
 - [x] Deserialization
 - [x] RFC 5545 compliancy
 - [x] Make it easier to create/edit occurency
 - [x] Make it easier to create/edit alarms
 - [x] Timezones fully implemented
 - [x] BASE64 encoding
 - [x] Including specific occurences with RECURRENCE-ID and UID
 - [x] Building calendar(s) (incl recurring) between dates

## How to use:

dotnet add packages [ICalendarNet](https://www.nuget.org/packages/ICalendarNet)

How to deserialize an get events
```csharp
using var httpClient = new HttpClient();
string icalvar = await httpClient.GetStringAsync("https://www.webcal.guru/en-US/download_calendar?calendar_instance_id=10");

Calendar? calendar = Calendar.LoadCalendar(icalvar);
CalendarEvent calEvent = calendar.GetEvents().First();
```

How to create a new calendar and serialize
```csharp
private static string SimpleCalendar()
{
    Calendar calendar = new Calendar();
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
    return CalSerializor.SerializeCalendar(calendar);
}
```

How to get the upcomming recurring events
```csharp
Calendar? calendar = Calendar.LoadCalendar(icalvar);
foreach (var calEvent in calendar.GetEvents())
{
	var rrule = new CalendarRecurrenceRule(string.Empty)
	{
		Frequency = FrequencyType.Monthly,
		Interval = 2,
		ByDay = [new WeekDay(DayOfWeek.Sunday)],
		BySetPosition = [4]
	};
	calEvent.SetRecurrenceRule(rrule);
    var recurrence = calEvent.GetRecurrence(10, DateTimeOffset.UtcNow); //Get the upcomming 10 events
}
```

How to get the expanded calendar for a lets say a year (expanded incl recurring items)
```csharp
DateTimeOffset RangeStart = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
DateTimeOffset RangeEnd = new(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);
Calendar? calendar = Calendar.LoadCalendar(icalvar);
var items = calendar?.ExpandCalendar(RangeStart, RangeEnd)

//all event items
var eventItems = items.OfType<CalendarEvent>();

//all Todo
var todoItems = items.OfType<CalendarTodo>();

//all journal items
var journalItems = items.OfType<CalendarJournal>();
```

## Benchmarking

All_Calendars = about 150 ical files

Big_Calendar = https://www.webcal.guru/en/event_list/culture_awareness +-430kb ical file
```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5900HX with Radeon Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.204
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3


```
| Method                                  | Mean          | Error       | StdDev      | Gen0      | Gen1      | Gen2     | Allocated  |
|---------------------------------------- |--------------:|------------:|------------:|----------:|----------:|---------:|-----------:|
| DeserializeCalendar                     |     20.389 μs |   0.3866 μs |   0.4136 μs |    1.8616 |    0.0610 |        - |    15.3 KB |
| SerializeCalendar                       |      1.509 μs |   0.0283 μs |   0.0552 μs |    0.5493 |    0.0019 |        - |    4.49 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      2.028 μs |   0.0369 μs |   0.0379 μs |    0.4463 |         - |        - |    3.66 KB |
| Deserialize_And_Serialize_all_Calendars | 11,808.863 μs | 233.8408 μs | 439.2098 μs | 1140.6250 | 1078.1250 | 125.0000 | 8754.97 KB |
| Deserialize_And_Serialize_Event         |      3.487 μs |   0.0470 μs |   0.0417 μs |    0.4845 |         - |        - |    3.98 KB |
| Deserialize_And_Serialize_Big_Calendar  |  7,715.103 μs | 150.3429 μs | 251.1892 μs |  992.1875 |  875.0000 | 343.7500 | 7006.71 KB |

When using ICal.Net

| Method                                           | Mean         | Error        | StdDev       | Median       | Gen0      | Gen1      | Gen2     | Allocated   |
|------------------------------------------------- |-------------:|-------------:|-------------:|-------------:|----------:|----------:|---------:|------------:|
| ICal_Net_DeserializeCalendar                     |     84.88 μs |     1.696 μs |     2.377 μs |     85.20 μs |   19.5313 |    2.4414 |        - |   160.66 KB |
| ICal_Net_SerializeCalendar                       |     20.55 μs |     0.401 μs |     0.521 μs |     20.53 μs |    5.2490 |    0.2441 |        - |    43.69 KB |
| ICal_Net_Deserialize_And_Serialize_all_Calendars | 53,953.75 μs |   484.078 μs |   404.227 μs | 54,074.80 μs | 6250.0000 | 1750.0000 | 750.0000 | 48846.21 KB |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 56,897.45 μs | 1,135.218 μs | 2,240.810 μs | 55,792.09 μs | 6400.0000 | 2000.0000 | 800.0000 | 51236.02 KB |

Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob

## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.