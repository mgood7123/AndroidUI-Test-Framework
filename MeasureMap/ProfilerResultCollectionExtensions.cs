namespace MeasureMap
{
    /// <summary>
    /// Extension methods for the IProfilerResultCollection
    /// </summary>
    public static class ProfilerResultCollectionExtensions
    {
        /// <summary>
        /// Outputs a Benchmark Test to the console
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static void Trace(this IProfilerResultCollection results)
        {
            var table = new Spectre.Console.Table();

            Spectre.Console.TableExtensions.AddColumns(
                table,
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.LeftAligned(
                        new Spectre.Console.TableColumn("Name")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Threads")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Avg\nTime")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Avg\nTicks")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Total")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Elapsed")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Iterations")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Fastest")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Slowest")
                    )
                ),
                Spectre.Console.ColumnExtensions.NoWrap(
                    Spectre.Console.AlignableExtensions.RightAligned(
                        new Spectre.Console.TableColumn("Total\nUsed Memory")
                    )
                )
            );

            foreach (string key in results.Keys)
            {
                IProfilerResult result = results[key];
                Spectre.Console.TableExtensions.AddRow(
                    table,
                    key,
                    result.ThreadCount.ToString(),
                    result.AverageTime.ToString(),
                    result.AverageTicks.ToString(),
                    result.TotalTime.ToString(),
                    result.Elapsed().ToString(),
                    result.TotalIterations.ToString(),
                    result.Fastest.Ticks.ToString(),
                    result.Slowest.Ticks.ToString(),
                    result.Increase.ToString()
                );
            }

            Spectre.Console.AnsiConsoleSettings s = new Spectre.Console.AnsiConsoleSettings();
            var console = Spectre.Console.AnsiConsole.Create(s);
            console.Write(table);
        }
    }
}
