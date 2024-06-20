using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        [HttpPost("run-benchmark")]
        public async Task<IActionResult> RunBenchmark()
        {
            var requests = new List<Task<BenchmarkLog>>();

            var now = DateTime.UtcNow;

            // Define different data sets for requests
            var requestDataList = new List<RequestData>
            {
                new RequestData { ChannelId = -1001075423523, StartDate = now.AddMonths(-1), EndDate = now }, // 1 month ago to now
                new RequestData { ChannelId = -1001075423523, StartDate = now.AddMonths(-2), EndDate = now }, // 2 months ago to now
                new RequestData { ChannelId = -1001075423523, StartDate = now.AddMonths(-3), EndDate = now }, // 3 months ago to now
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

        private async Task<BenchmarkLog> SendRequest(RequestData requestData)
        {
            var stopwatch = Stopwatch.StartNew();
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