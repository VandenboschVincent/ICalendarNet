// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using ICalendarNet.Benchmarking;

Console.WriteLine("Hello, World!");

var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
foreach (var item in summary)
{
    Console.WriteLine("LogFilePath: {0}", item.LogFilePath);
    Console.WriteLine("ResultsDirectoryPath: {0}", item.ResultsDirectoryPath);
}