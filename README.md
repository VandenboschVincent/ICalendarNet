
## ICalendarNet
ICalendarNet is an iCalendar (RFC 5545) class library for .NET aimed at providing RFC 5545 compliance, while providing full compatibility with popular calendaring applications and libraries.
Credits goes to https://github.com/rianjs/ical.net for providing a ton of info and the baseline of this project.


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
        new CalendarAlarm()
        {
            Trigger = "-PT1080M",
            Action = "DISPLAY",
            Description = "Reminder"
        });
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

| SERIALIZATION                                  | Mean          | Error       | StdDev      | Median        | Gen0      | Gen1      | Gen2     | Allocated   |
|---------------------------------------- |--------------:|------------:|------------:|--------------:|----------:|----------:|---------:|------------:|
| DeserializeCalendar                     |     12.656 us |   0.1432 us |   0.1195 us |     12.675 us |    3.4180 |    0.1221 |        - |    20.94 KB |
| SerializeCalendar                       |      3.129 us |   0.0623 us |   0.0612 us |      3.125 us |    1.9684 |    0.0267 |        - |    12.06 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      1.406 us |   0.0280 us |   0.0444 us |      1.382 us |    0.6065 |    0.0019 |        - |     3.73 KB |
| Deserialize_And_Serialize_all_Calendars (150 calendars) |  6,945.098 us | 136.4880 us | 195.7470 us |  6,970.355 us |  539.0625 |  437.5000 | 125.0000 |  2997.37 KB |
| Deserialize_And_Serialize_Event         |      2.014 us |   0.0187 us |   0.0165 us |      2.008 us |    0.6523 |         - |        - |     4.01 KB |
| Deserialize_And_Serialize_Big_Calendar (1,078 events) | 30,689.179 us | 274.9266 us | 243.7153 us | 30,682.292 us | 4062.5000 | 2156.2500 | 906.2500 | 22656.61 KB |