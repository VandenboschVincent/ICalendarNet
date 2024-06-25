// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using ICalendarNet.Benchmarking;

Console.WriteLine("Hello, World!");

var summary = BenchmarkRunner.Run<ICalBenchmarkingTests>();
Console.WriteLine("LogFilePath: {0}", summary.LogFilePath);
Console.WriteLine("ResultsDirectoryPath: {0}", summary.ResultsDirectoryPath);