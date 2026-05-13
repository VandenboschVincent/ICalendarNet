


## ICalendarNet
ICalendarNet is an iCalendar (RFC 5545) class library for .NET aimed at providing RFC 5545 compliance, while providing full compatibility with popular calendaring applications and libraries.
Credits go to [Rianjs](https://github.com/rianjs/ical.net) for providing a ton of info and the baseline of this project.


## Available for
* Net Standard 2.1
* Net 8.0/9.0/10.0

## Roadmap:

 - [x] Serialization
 - [x] Deserialization
 - [x] RFC 5545 compliancy
 - [x] Make it easier to create/edit occurency
 - [x] Make it easier to create/edit alarms
 - [ ] Timezones fully implemented

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
| DeserializeCalendar                     |     19.052 μs |   0.2674 μs |   0.2371 μs |    2.1362 |    0.0916 |        - |   17.55 KB |
| SerializeCalendar                       |      1.540 μs |   0.0304 μs |   0.0717 μs |    0.6104 |    0.0019 |        - |       5 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      1.971 μs |   0.0395 μs |   0.0387 μs |    0.4730 |         - |        - |    3.89 KB |
| Deserialize_And_Serialize_all_Calendars | 12,177.778 μs | 241.9347 μs | 454.4120 μs | 1218.7500 | 1140.6250 | 156.2500 | 9445.46 KB |
| Deserialize_And_Serialize_Event         |      3.488 μs |   0.0678 μs |   0.0634 μs |    0.5302 |    0.0038 |        - |    4.36 KB |
| Deserialize_And_Serialize_Big_Calendar  | 10,138.454 μs | 202.4134 μs | 343.7133 μs | 1140.6250 |  984.3750 | 406.2500 | 7790.86 KB |

When using ICal.Net

| Method                                           | Mean         | Error        | StdDev       | Gen0      | Gen1      | Gen2     | Allocated   |
|------------------------------------------------- |-------------:|-------------:|-------------:|----------:|----------:|---------:|------------:|
| ICal_Net_DeserializeCalendar                     |     84.03 μs |     1.679 μs |     1.797 μs |   19.6533 |    2.6855 |        - |   160.66 KB |
| ICal_Net_SerializeCalendar                       |     19.69 μs |     0.374 μs |     0.349 μs |    5.2490 |    0.2441 |        - |    43.69 KB |
| ICal_Net_Deserialize_And_Serialize_all_Calendars |           NA |           NA |           NA |        NA |        NA |       NA |          NA |
| ICal_Net_Deserialize_And_Serialize_Big_Calendar  | 57,775.08 μs | 1,135.447 μs | 1,435.977 μs | 6750.0000 | 2000.0000 | 750.0000 | 53087.93 KB |

Benchmarks with issues:
  OtherToolsTests.ICal_Net_Deserialize_And_Serialize_all_Calendars: DefaultJob

## How it works:

I replaced Regex functions, and started using [ReadonlySpan\<char>](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1?view=net-8.0) instead of strings when possible.
When using .net8 we can use the performant [SearchValues\<T>](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.searchvalues-1?view=net-8.0) to search for values.