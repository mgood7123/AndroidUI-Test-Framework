using MeasureMap;
using Spectre.Console;
using System;

namespace AndroidUITestFramework
{
    public abstract class BenchmarkTest : Test
    {
        public abstract void prepareBenchmark(BenchmarkRunner runner);
        public sealed override void Run(TestGroup nullableInstance)
        {
            BenchmarkRunner benchmarkRunner = new();
            prepareBenchmark(benchmarkRunner);
            WriteToConsole(benchmarkRunner.RunSessions());
        }

        private static void WriteToConsole(IProfilerResultCollection results)
        {
            Console.WriteLine("Iterations: " + results.Iterations);

            var table = new Table();

            table.AddColumns(
                new TableColumn("Name").LeftAligned(),
                new TableColumn("Avg Time").RightAligned(),
                new TableColumn("Avg Ticks").RightAligned(),
                new TableColumn("Total").RightAligned(),
                new TableColumn("Fastest").RightAligned(),
                new TableColumn("Slowest").RightAligned(),
                new TableColumn("Memory Increase").RightAligned()
            );

            foreach (string key in results.Keys)
            {
                IProfilerResult result = results[key];
                table.AddRow(
                    key,
                    result.AverageTime.ToString(),
                    result.AverageTicks.ToString(),
                    result.TotalTime.ToString(),
                    result.Fastest.Ticks.ToString(),
                    result.Slowest.Ticks.ToString(),
                    result.Increase.ToString()
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
