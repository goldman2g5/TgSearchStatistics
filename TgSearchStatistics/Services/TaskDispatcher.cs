using System.Collections.Concurrent;
using TgSearchStatistics.Models;

namespace TgSearchStatistics.Services
{
    public class TaskDispatcher
    {
        private readonly ConcurrentQueue<TaskRequest> _sharedQueue = new();
        private readonly ILogger<TaskDispatcher> _logger;
        private readonly object _lockObj = new object();

        public TaskDispatcher(ILogger<TaskDispatcher> logger)
        {
            _logger = logger;
            Task.Run(() => DispatchTasks());
        }

        public void EnqueueTask(TaskRequest taskRequest)
        {
            _sharedQueue.Enqueue(taskRequest);
            _logger.LogInformation("Task enqueued. Shared queue size: {QueueSize}", _sharedQueue.Count);
        }

        private async Task DispatchTasks()
        {
            while (true)
            {
                if (_sharedQueue.TryDequeue(out var taskRequest))
                {
                    lock (_lockObj)
                    {
                        var clientWrapper = TelegramClientService.Clients
                            .OrderBy(c => c.GetTotalBusyTime())
                            .FirstOrDefault();

                        if (clientWrapper != null)
                        {
                            taskRequest.Client = clientWrapper.Client; // Set the client in the task request
                            clientWrapper.EnqueueTask(taskRequest);
                            _logger.LogInformation("Task dispatched to client {ClientId}. Total busy time: {BusyTimeSeconds} seconds",
                                clientWrapper.DatabaseId, clientWrapper.GetTotalBusyTime().TotalSeconds);
                        }
                        else
                        {
                            _logger.LogWarning("No available client to dispatch the task.");
                            _sharedQueue.Enqueue(taskRequest); // Re-enqueue if no clients available
                        }
                    }
                }
                await Task.Delay(50); // Prevent tight loop
            }
        }
    }
}
