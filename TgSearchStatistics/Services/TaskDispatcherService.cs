//using System.Collections.Concurrent;
//using TgSearchStatistics.Models;

//namespace TgSearchStatistics.Services
//{
//    public class TaskDispatcherService
//    {
//        private readonly ConcurrentQueue<TaskRequest> sharedTaskQueue = new();
//        private readonly List<ClientWorker> clientWorkers;
//        private readonly Timer dispatchTimer;
//        private readonly ILogger<TaskDispatcherService> _logger;

//        public TaskDispatcherService(List<ClientWorker> clientWorkers, ILogger<TaskDispatcherService> logger)
//        {
//            this.clientWorkers = clientWorkers;
//            _logger = logger;
//            dispatchTimer = new Timer(DispatchTasks, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
//        }

//        public void EnqueueTask(TaskRequest task)
//        {
//            sharedTaskQueue.Enqueue(task);
//        }

//        private void DispatchTasks(object state)
//        {
//            while (sharedTaskQueue.TryDequeue(out var task))
//            {
//                var leastBusyClient = clientWorkers.OrderBy(c => c.GetQueueLength()).FirstOrDefault();
//                if (leastBusyClient != null)
//                {
//                    leastBusyClient.EnqueueTask(task);
//                }
//                else
//                {
//                    _logger.LogWarning("No available clients to handle task.");
//                }
//            }
//        }
//    }
//}
