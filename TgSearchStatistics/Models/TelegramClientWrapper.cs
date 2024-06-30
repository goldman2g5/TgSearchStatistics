using WTelegram;
using System.Collections.Concurrent;
using TL;

namespace TgSearchStatistics.Models
{
    public class TelegramClientWrapper
    {
        private static ILogger<TelegramClientWrapper> _logger;

        public static void SetLogger(ILogger<TelegramClientWrapper> logger)
        {
            _logger = logger;
        }

        public Client Client { get; set; }
        public int DatabaseId { get; set; }
        public long TelegramId { get; set; }
        public bool IsBusy { get; set; }
        private ConcurrentQueue<(DateTime start, DateTime? end)> taskDurations;
        public TimeSpan TotalBusyTime { get; private set; }
        private Timer busyTimeUpdateTimer;
        private readonly TimeSpan busyTimeWindow;
        private readonly ConcurrentQueue<TaskRequest> taskQueue = new();
        private readonly object lockObj = new object();

        public TelegramClientWrapper(Client client, int databaseId, long telegramId, TimeSpan? busyTimeWindow = null)
        {
            Client = client;
            DatabaseId = databaseId;
            TelegramId = telegramId;
            IsBusy = false;
            taskDurations = new ConcurrentQueue<(DateTime start, DateTime? end)>();
            TotalBusyTime = TimeSpan.Zero;
            this.busyTimeWindow = busyTimeWindow ?? TimeSpan.FromSeconds(10);

            busyTimeUpdateTimer = new Timer(UpdateTotalBusyTime, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
            Task.Run(() => ProcessTasks());
        }

        public void AddTaskStart()
        {
            var startTime = DateTime.UtcNow;
            taskDurations.Enqueue((startTime, null));
            _logger?.LogInformation($"Client {DatabaseId}: Task started at {startTime}. Total busy time: {TotalBusyTime.TotalSeconds} seconds.");
        }

        public void AddTaskEnd()
        {
            var endTime = DateTime.UtcNow;
            bool updated = false;

            var tasks = taskDurations.ToArray();
            taskDurations = new ConcurrentQueue<(DateTime start, DateTime? end)>();

            foreach (var task in tasks)
            {
                if (!task.end.HasValue && !updated)
                {
                    taskDurations.Enqueue((task.start, endTime));
                    updated = true;
                }
                else
                {
                    taskDurations.Enqueue(task);
                }
            }

            if (updated)
            {
                _logger?.LogInformation($"Client {DatabaseId}: Task ended at {endTime}. Total busy time: {TotalBusyTime.TotalSeconds} seconds.");
            }
            else
            {
                _logger?.LogInformation($"Client {DatabaseId}: Attempted to end a task when no tasks were recorded or task already ended.");
            }
        }

        public TimeSpan GetTotalBusyTime()
        {
            return TotalBusyTime;
        }

        private void UpdateTotalBusyTime(object state)
        {
            var now = DateTime.UtcNow;
            var threshold = now - busyTimeWindow;
            var newBusyTime = TimeSpan.Zero;

            foreach (var task in taskDurations)
            {
                var endTime = task.end ?? now;

                if (task.start >= threshold)
                {
                    newBusyTime += endTime - task.start;
                }
                else if (endTime > threshold)
                {
                    newBusyTime += endTime - threshold;
                }
            }

            TotalBusyTime = newBusyTime;

            _logger?.LogInformation($"Client {DatabaseId}: Updated total busy time: {TotalBusyTime.TotalSeconds} seconds.");
        }

        public void EnqueueTask(TaskRequest taskRequest)
        {
            lock (lockObj)
            {
                taskQueue.Enqueue(taskRequest);
            }
        }

        private async Task ProcessTasks()
        {
            while (true)
            {
                TaskRequest taskRequest = null;

                lock (lockObj)
                {
                    if (taskQueue.TryDequeue(out var request))
                    {
                        taskRequest = request;
                    }
                }

                if (taskRequest != null)
                {
                    try
                    {
                        var result = await ExecuteWithClientAsync(taskRequest.TaskToExecute);
                        taskRequest.TaskCompletionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        taskRequest.TaskCompletionSource.SetException(ex);
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        private async Task<List<TL.Message>> ExecuteWithClientAsync(Func<Client, Task<List<TL.Message>>> task)
        {
            ArgumentNullException.ThrowIfNull(task);

            try
            {
                IsBusy = true;
                AddTaskStart();

                var result = await task(Client);

                return result;
            }
            finally
            {
                AddTaskEnd();
                IsBusy = false;
            }
        }
    }
}
