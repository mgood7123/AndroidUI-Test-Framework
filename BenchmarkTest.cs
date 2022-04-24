using MeasureMap;

namespace AndroidUITestFramework
{
    public abstract class BenchmarkTest : Test
    {
        public abstract void prepareBenchmark(BenchmarkRunner runner);
        public sealed override void Run(TestGroup nullableInstance)
        {
            BenchmarkRunner benchmarkRunner = new();
            prepareBenchmark(benchmarkRunner);
            benchmarkRunner.RunSessions().Trace();
        }
    }
}
