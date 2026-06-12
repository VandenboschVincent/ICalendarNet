


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
 - [] Improve speed and allocations of ExpandCalendar

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

Deserialize_And_Expand_Daily_Event = Generates 183 events with different rrule values for a year
```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5900HX with Radeon Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.204
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  DefaultJob : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3


```
| Method                                  | Mean         | Error       | StdDev      | Median       | Gen0      | Gen1      | Gen2     | Allocated  |
|---------------------------------------- |-------------:|------------:|------------:|-------------:|----------:|----------:|---------:|-----------:|
| DeserializeCalendar                     |     6.287 μs |   0.1251 μs |   0.2639 μs |     6.237 μs |    1.9302 |    0.0839 |        - |    15.8 KB |
| SerializeCalendar                       |     1.266 μs |   0.0381 μs |   0.1110 μs |     1.237 μs |    0.5226 |    0.0019 |        - |    4.27 KB |
| Deserialize_And_Serialize_all_Calendars | 9,954.334 μs | 198.5608 μs | 363.0797 μs | 9,900.213 μs | 1140.6250 | 1078.1250 | 140.6250 | 8761.63 KB |
| Deserialize_And_Serialize_Big_Calendar  | 4,221.036 μs | 109.8993 μs | 320.5815 μs | 4,113.208 μs |  898.4375 |  785.1563 | 332.0313 | 6005.29 KB |
| Deserialize_And_Expand_Daily_Event      | 2,218.328 μs |  39.8096 μs |  37.2379 μs | 2,219.503 μs |  359.3750 |  125.0000 |        - | 2938.13 KB |

When using ICal.Net

| Method                                           | Mean         | Error        | StdDev       | Median       | Gen0      | Gen1      | Gen2     | Allocated   |
|------------------------------------------------- |-------------:|-------------:|-------------:|-------------:|----------:|----------:|---------:|------------:|
| ICal_Net_DeserializeCalendar                     |     83.34 μs |     1.658 μs |     4.510 μs |     82.00 μs |   19.5313 |    2.4414 |        - |   160.66 KB |
| ICal_Net_SerializeCalendar                       |     20.32 μs |     0.418 μs |     1.226 μs |     19.78 μs |    5.2490 |    0.2441 |        - |    43.19 KB |
| ICal_Net_Deserialize_And_Serialize_all_Calendars | 58,101.93 μs | 1,630.405 μs | 4,807.287 μs | 57,320.41 μs | 6250.0000 | 1750.0000 | 750.0000 | 48846.24 KB |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 44,284.73 μs | 1,264.022 μs | 3,565.196 μs | 43,441.94 μs | 5400.0000 | 1600.0000 | 600.0000 | 42842.59 KB |
| ICal_Net_Deserialize_And_Expand_Daily_Event      |    625.47 μs |    12.427 μs |    30.483 μs |    621.88 μs |  117.1875 |   22.4609 |        - |   962.22 KB |

Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob

## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.