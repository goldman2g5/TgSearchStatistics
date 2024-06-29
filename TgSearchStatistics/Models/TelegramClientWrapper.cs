using WTelegram;
using System.Collections.Concurrent;

namespace TgSearchStatistics.Models
{
    public class TelegramClientWrapper
    {
        public static ILogger<TelegramClientWrapper> Logger { private get; set; }

        public Client Client { get; set; }
        public int DatabaseId { get; set; }
        public long TelegramId { get; set; }
        public bool IsBusy { get; set; }
        private ConcurrentQueue<(DateTime start, DateTime? end)> taskDurations;
        public TimeSpan TotalBusyTime { get; private set; }
        private Timer busyTimeUpdateTimer;

        public TelegramClientWrapper(Client client, int databaseId, long telegramId, TimeSpan? busyTimeWindow = null)
        {
            Client = client;
            DatabaseId = databaseId;
            TelegramId = telegramId;
            IsBusy = false;
            taskDurations = new ConcurrentQueue<(DateTime start, DateTime? end)>();
            TotalBusyTime = TimeSpan.Zero;

            busyTimeUpdateTimer = new Timer(UpdateTotalBusyTime, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
        }

        public void AddTaskStart()
        {
            var startTime = DateTime.UtcNow;
            taskDurations.Enqueue((startTime, null));
            Console.WriteLine($"Client {DatabaseId}: Task started at {startTime}. Total busy time: {TotalBusyTime.TotalSeconds} seconds.");
        }

        public void AddTaskEnd()
        {
            var endTime = DateTime.UtcNow;
            bool updated = false;

            var updatedQueue = new ConcurrentQueue<(DateTime start, DateTime? end)>();
            while (taskDurations.TryDequeue(out var task))
            {
                if (!task.end.HasValue && !updated)
                {
                    updatedQueue.Enqueue((task.start, endTime));
                    updated = true;
                }
                else
                {
                    updatedQueue.Enqueue(task);
                }
            }
            taskDurations = updatedQueue;

            if (updated)
            {
                Console.WriteLine($"Client {DatabaseId}: Task ended at {endTime}. Total busy time: {TotalBusyTime.TotalSeconds} seconds.");
            }
            else
            {
                Console.WriteLine($"Client {DatabaseId}: Attempted to end a task when no tasks were recorded or task already ended.");
            }
        }

        public TimeSpan GetTotalBusyTime()
        {
            return TotalBusyTime;
        }

        private void UpdateTotalBusyTime(object state)
        {
            var now = DateTime.UtcNow;
            var period = TimeSpan.FromSeconds(10);
            var threshold = now - period;
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

            Console.WriteLine($"Client {DatabaseId}: Updated total busy time: {TotalBusyTime.TotalSeconds} seconds.");
        }
    }
}
