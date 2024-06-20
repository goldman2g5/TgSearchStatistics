using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TgSearchStatistics.Services;
using Xunit;

namespace TgSearchStatistics.Benchmarks
{
    public class ApiBenchmarkTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiBenchmarkTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
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
            // Log to console or a file or a database
            Console.WriteLine($"URL: {log.Url}, StatusCode: {log.StatusCode}, ExecutionTime: {log.ExecutionTime}ms, Timestamp: {log.Timestamp}, Response: {log.ResponseContent}");
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
