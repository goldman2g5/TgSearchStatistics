﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace TgSearchStatistics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenchmarkController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _logFilePath = "benchmark_log.txt";

        public BenchmarkController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7143"); // Ensure this matches the address your API is running on

            // Ensure the log file is created or cleared at the start
            if (System.IO.File.Exists(_logFilePath))
            {
                System.IO.File.Delete(_logFilePath);
            }
            System.IO.File.Create(_logFilePath).Dispose();
        }

        [HttpPost("run-benchmark-async")]
        public async Task<IActionResult> RunBenchmarkAsync()
        {
            var requests = new List<Task<BenchmarkLog>>();

            var now = DateTime.UtcNow;

            // Define different data sets for requests
            var requestDataList = new List<RequestData>
            {
                                //new RequestData { ChannelId = -1001135818819, StartDate = now.AddDays(-10), EndDate = now }, // 3 months ago to nownew RequestData { ChannelId = -1001135818819, StartDate = now.AddDays(-10), EndDate = now }, // 3 months ago to now
                new RequestData { ChannelId = -1001075423523, StartDate = now.AddDays(-120), EndDate = now },

                new RequestData { ChannelId = -1001135818819, StartDate = now.AddDays(-80), EndDate = now },
                new RequestData { ChannelId = -1001403805810, StartDate = now.AddDays(-30), EndDate = now },


                                                                                                             // Add more data sets as needed
            };

            // Send requests concurrently
            foreach (var requestData in requestDataList)
            {
                requests.Add(SendRequest(requestData));
            }

            var results = await Task.WhenAll(requests);

            foreach (var log in results)
            {
                LogBenchmarkResult(log);
            }

            return Ok(results);
        }

        [HttpPost("run-benchmark")]
        public async Task<IActionResult> RunBenchmark()
        {
            var now = DateTime.UtcNow;

            // Define different data sets for requests
            var requestDataList = new List<RequestData>
    {
        new RequestData { ChannelId = -1001075423523, StartDate = now.AddDays(-90), EndDate = now },

                new RequestData { ChannelId = -1001135818819, StartDate = now.AddDays(-60), EndDate = now },
                new RequestData { ChannelId = -1001703721750, StartDate = now.AddDays(-30), EndDate = now },
        // Add more data sets as needed
    };

            var results = new List<BenchmarkLog>();

            // Send requests sequentially
            foreach (var requestData in requestDataList)
            {
                var log = await SendRequest(requestData);
                await Task.Delay(1000);
                results.Add(log);
                LogBenchmarkResult(log);
            }

            return Ok(results);
        }

        private async Task<BenchmarkLog> SendRequest(RequestData requestData)
        {
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(new Random().Next(1000, 5000));
            var response = await _client.GetAsync($"/api/Telegram/GetMessagesByPeriod?channelId={requestData.ChannelId}&startDate={requestData.StartDate:yyyy-MM-ddTHH:mm:ssZ}&endDate={requestData.EndDate:yyyy-MM-ddTHH:mm:ssZ}");
            stopwatch.Stop();

            double executionTimeSeconds = stopwatch.Elapsed.TotalSeconds;

            return new BenchmarkLog
            {
                Url = $"/api/Telegram/GetMessagesByPeriod?channelId={requestData.ChannelId}&startDate={requestData.StartDate:yyyy-MM-ddTHH:mm:ssZ}&endDate={requestData.EndDate:yyyy-MM-ddTHH:mm:ssZ}",
                StatusCode = response.StatusCode,
                ExecutionTime = $"{executionTimeSeconds:F2} seconds",
                Timestamp = DateTime.UtcNow
            };
        }

        private void LogBenchmarkResult(BenchmarkLog log)
        {
            var logEntry = $"URL: {log.Url}, StatusCode: {log.StatusCode}, ExecutionTime: {log.ExecutionTime}, Timestamp: {log.Timestamp}\n";

            // Log to console
            Console.WriteLine(logEntry);

            // Log to file
            System.IO.File.AppendAllText(_logFilePath, logEntry);
        }

        private class BenchmarkLog
        {
            public string Url { get; set; }
            public System.Net.HttpStatusCode StatusCode { get; set; }
            public string ExecutionTime { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private class RequestData
        {
            public long ChannelId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}