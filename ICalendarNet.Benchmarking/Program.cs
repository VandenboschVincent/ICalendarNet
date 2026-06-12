// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Ical.Net;
using ICalendarNet.Benchmarking;
using ICalendarNet.Benchmarking.Data;
using ICalendarNet.Models.Components;
using ICalendarNet.Models.Enum;

//var dateTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
//Calendar? calendar = Calendar.LoadCalendar(SampleData.SampleRecurring);
//var expanded = calendar!.ExpandCalendar(dateTime, dateTime.AddYears(1)).ToList();

//var start = new Ical.Net.DataTypes.CalDateTime(2020, 1, 1, 0, 0, 0, _aTzid);
//var end = new Ical.Net.DataTypes.CalDateTime(2021, 1, 1, 0, 0, 0, _aTzid);
//Ical.Net.Calendar? Othercalendar = Ical.Net.Calendar.Load(SampleData.SampleRecurring);
//return [.. Othercalendar!.GetOccurrences(start).TakeWhileBefore(end)];

//ICalSerializationTests calSerializationTests = new();
//await calSerializationTests.Setup();
//var calendarEvent = calSerializationTests.Deserialize_And_Expand_Daily_Event();
//var eventDates = calendarEvent.Select(t => t.DateTimeStart.Value.UtcDateTime).ToList();

//OtherToolsTests otherToolsTests = new();
//await otherToolsTests.Setup();
//var items = otherToolsTests.ICal_Net_Deserialize_And_Expand_Daily_Event();
//var otherEvretDates = items.Select(t => t.Period.StartTime.AsUtc).ToList();

var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
foreach (var item in summary)
{
    Console.WriteLine("LogFilePath: {0}", item.LogFilePath);
    Console.WriteLine("ResultsDirectoryPath: {0}", item.ResultsDirectoryPath);
}