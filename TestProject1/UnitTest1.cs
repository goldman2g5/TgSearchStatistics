using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace TgSearchStatistics.Benchmarks
{
    public class ApiBenchmarkTests
    {
        private readonly HttpClient _client;
        private readonly string _logFilePath = "benchmark_log.txt";

        public ApiBenchmarkTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:7143") // Ensure this matches the address your API is running on
            };

            // Ensure the log file is created or cleared at the start of the tests
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
            File.Create(_logFilePath).Dispose();
        }

        [Fact]
        public async Task BenchmarkConcurrentGetMessagesByPeriodRequests()
        {
            var requests = new List<Task<BenchmarkLog>>();

            // Define different data sets for requests
            var requestDataList = new List<RequestData>
            {
                new RequestData { ChannelId = 1234567890L, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 12, 31) },
                new RequestData { ChannelId = 1234567891L, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 6, 30) },
                new RequestData { ChannelId = 1234567892L, StartDate = new DateTime(2023, 7, 1), EndDate = new DateTime(2023, 12, 31) },
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
        }

        private async Task<BenchmarkLog> SendRequest(RequestData requestData)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync($"/api/GetMessagesByPeriod?channelId={requestData.ChannelId}&startDate={requestData.StartDate:yyyy-MM-ddTHH:mm:ssZ}&endDate={requestData.EndDate:yyyy-MM-ddTHH:mm:ssZ}");
            stopwatch.Stop();

            var result = await response.Content.ReadAsStringAsync();

            return new BenchmarkLog
            {
                Url = $"/api/GetMessagesByPeriod?channelId={requestData.ChannelId}&startDate={requestData.StartDate:yyyy-MM-ddTHH:mm:ssZ}&endDate={requestData.EndDate:yyyy-MM-ddTHH:mm:ssZ}",
                StatusCode = response.StatusCode,
                ExecutionTime = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow,
                ResponseContent = result
            };
        }

        private void LogBenchmarkResult(BenchmarkLog log)
        {
            var logEntry = $"URL: {log.Url}, StatusCode: {log.StatusCode}, ExecutionTime: {log.ExecutionTime}ms, Timestamp: {log.Timestamp}, Response: {log.ResponseContent}\n";

            // Log to console
            Console.WriteLine(logEntry);

            // Log to file
            File.AppendAllText(_logFilePath, logEntry);
        }

        private class BenchmarkLog
        {
            public string Url { get; set; }
            public System.Net.HttpStatusCode StatusCode { get; set; }
            public long ExecutionTime { get; set; }
            public DateTime Timestamp { get; set; }
            public string ResponseContent { get; set; }
        }

        private class RequestData
        {
            public long ChannelId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}