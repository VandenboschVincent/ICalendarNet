
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

| SERIALIZATION                                  | Mean          | Error       | StdDev        | Median        | Gen0      | Gen1      | Gen2     | Allocated   |
|---------------------------------------- |--------------:|------------:|--------------:|--------------:|----------:|----------:|---------:|------------:|
| DeserializeCalendar                     |     14.024 us |   0.5869 us |     1.6933 us |     13.325 us |    3.4180 |    0.1373 |        - |    20.95 KB |
| SerializeCalendar                       |      3.917 us |   0.2320 us |     0.6692 us |      3.664 us |    1.9684 |    0.0229 |        - |    12.06 KB |
| Deserialize_And_Serialize_Tiny_Calendar |      1.959 us |   0.1353 us |     0.3967 us |      1.804 us |    0.6084 |    0.0019 |        - |     3.74 KB |
| Deserialize_And_Serialize_all_Calendars (150 calendars) |  8,895.427 us | 463.8940 us | 1,367.8020 us |  9,324.708 us |  515.6250 |  421.8750 | 109.3750 |  2996.25 KB |
| Deserialize_And_Serialize_Event         |      2.245 us |   0.0447 us |     0.1053 us |      2.197 us |    0.6523 |    0.0038 |        - |     4.02 KB |
| Deserialize_And_Serialize_Big_Calendar (1,078 events)  | 28,687.914 us | 894.9811 us | 2,567.8685 us | 28,108.100 us | 3750.0000 | 1812.5000 | 625.0000 | 22656.47 KB |