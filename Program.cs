using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System.Linq;

namespace RsaRepro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var defaultJob = Job
                .RyuJitX64
                .WithGcServer(true);

            if (args.Any(args => args.Contains("testrun") || args.Contains("testRun")))
            {
                defaultJob = defaultJob.WithInvocationCount(16)
                .WithIterationCount(1)
                .WithLaunchCount(1)
                .WithWarmupCount(1);
            }



            // separate to other method!
            var configOptions = DefaultConfig
                .Instance
                .AddExporter(JsonExporter.Default)
                //.AddExporter(CsvExporter.Default)
                //.AddExporter(MarkdownExporter.Default)
                //.AddExporter(HtmlExporter.Default)
                .AddExporter(PlainExporter.Default)
                .AddExporter(RPlotExporter.Default)
                .AddDiagnoser(MemoryDiagnoser.Default)
                //.AddDiagnoser(new InliningDiagnoser())
                //.AddDiagnoser(new EtwProfiler())
                // .AddDiagnoser(ThreadingDiagnoser.Default)
                //      .AddDiagnoser(new ConcurrencyVisualizerProfiler())
                //.AddHardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.CacheMisses, HardwareCounter.TotalCycles)
                .AddDiagnoser(ExceptionDiagnoser.Default)
                .WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Default, MethodOrderPolicy.Declared))
                .AddColumn(new BaselineColumn())
                .AddColumn(StatisticColumn.Min)
                .AddColumn(new CategoriesColumn())
                .AddColumn(new RankColumn(BenchmarkDotNet.Mathematics.NumeralSystem.Arabic))
#if NETFRAMEWORK
            .AddJob(defaultJob.WithRuntime(ClrRuntime.Net48))
#else
                .AddJob(defaultJob.WithRuntime(CoreRuntime.Core70))
#endif
                .AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            var summary = BenchmarkRunner.Run<RsaCryptoDegradations>(configOptions);
        }
    }
}