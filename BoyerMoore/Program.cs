using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using BoyerMoore;
using ScottPlot;

public class Program
{
    // 1. Function to generate random text and patterns
    private static string GenerateRandomString(int length, Random random)
    {
        var builder = new StringBuilder(length);
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        // const string chars = "ab";
        for (int i = 0; i < length; i++)
        {
            builder.Append(chars[random.Next(chars.Length)]);
        }
        return builder.ToString();
    }

    // Function to test performance and step counts of different algorithms
    private static (List<double> times, List<long> steps) TestPerformance(
        ISubstringSearch substringSearchFunction,
        string pattern,
        int[] textLengths,
        int numTrials = 5)
    {
        var times = new List<double>();
        var steps = new List<long>();

        // Initialize a fixed random seed for reproducibility
        Random random = new Random(42);

        // Run the test for each text length
        foreach (var length in textLengths)
        {
            double totalTime = 0;
            long totalSteps = 0;

            for (int i = 0; i < numTrials; i++)
            {
                var text = GenerateRandomString(length, random);

                // Measure time and steps
                Stopwatch stopwatch = Stopwatch.StartNew();
                var matches = substringSearchFunction.Search(text, pattern, out long stepCount);
                stopwatch.Stop();

                totalTime += stopwatch.Elapsed.TotalSeconds;
                totalSteps += stepCount;
            }

            // Average time and steps over all trials
            times.Add(totalTime / numTrials);
            steps.Add(totalSteps / numTrials);
        }

        return (times, steps);
    }

    // 3. Function to plot the performance results
    private static void PlotPerformance(int[] textLengths, Dictionary<string, List<double>> performanceData, string yAxisLabel, string plotTitle, string fileName)
    {
        var plt = new Plot();

        foreach (var entry in performanceData)
        {
            string label = entry.Key;
            List<double> performanceValues = entry.Value;

            double[] x = Array.ConvertAll(textLengths, item => (double)item);
            double[] y = performanceValues.ToArray();

            var scatter = plt.Add.Scatter(x, y);
            scatter.LegendText = label;
        }

        // Customize the plot
        plt.Title(plotTitle);
        plt.XLabel("Text Length");
        plt.YLabel(yAxisLabel);
        plt.Legend.Alignment = Alignment.UpperCenter;
        plt.ShowLegend();
        plt.ShowGrid();

        // Save the plot as an image file and display it
        plt.SavePng(fileName, 600, 400);
    }
    
    public static void Main()
    {
        // Define text lengths and pattern length
        int[] textLengths = { 100, 1000, 5000, 10000, 50000, 100000  };
        int patternLength = 2;
        int numTrials = 5;

        // Generate a random pattern
        Random random = new Random();
        string pattern = GenerateRandomString(patternLength, random);

        // Define search algorithms
        var searchAlgorithms = new Dictionary<string, ISubstringSearch>
        {
            { "Naive Search", new NaiveSubstringSearch() },
            { "KMP Search", new Kmp() },
            { "Aho-Corasick Search", new AhoCorasick(pattern) },
            { "Boyer-Moore Search", new BoyerMoore.BoyerMoore(pattern) }
        };

        // Create two dictionaries for time and step results
        var timeResults = new Dictionary<string, List<double>>();
        var stepResults = new Dictionary<string, List<double>>();

        // Test each algorithm
        foreach (var algo in searchAlgorithms)
        {
            Console.WriteLine($"Testing {algo.Key}...");
            var (times, steps) = TestPerformance(algo.Value, pattern, textLengths, numTrials);

            // Store times and steps in their respective dictionaries
            timeResults.Add(algo.Key, times);
            stepResults.Add(algo.Key, steps.Select(step => (double)step).ToList());

            Console.WriteLine($"{algo.Key} completed.");
        }

        // Plot the results
        PlotPerformance(textLengths, timeResults, "Average Time (seconds)", "Performance Comparison of Substring Search Algorithms - Time", "time_performance.png");
        PlotPerformance(textLengths, stepResults, "Average Steps", "Performance Comparison of Substring Search Algorithms - Steps", "steps_performance.png");
    }
}
