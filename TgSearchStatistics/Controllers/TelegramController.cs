using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TgSearchStatistics.Models.BaseModels;
using WTelegram;
using TL;
using System.Linq;
using TgSearchStatistics.Utility;
using TgSearchStatistics.Services;
using System.Text.Json;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TgSearchStatistics.Interfaces;
using TgSearchStatistics.Queues;
//using Nest;

namespace TgCheckerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly TgDbContext _context;
        private readonly ILogger<TelegramController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly TelegramClientService _tgclientService;
        private readonly IMessageUpdateQueue _messageUpdateQueue;

        //private readonly IElasticClient _elasticClient;


        public TelegramController(TgDbContext context, ILogger<TelegramController> logger, IMessageUpdateQueue messageUpdateQueue, IWebHostEnvironment env, TelegramClientService telegramClientService/*, IElasticClient elasticClient*/)
        {
            _context = context;
            _logger = logger;
            _env = env;
            _tgclientService = telegramClientService;
            _messageUpdateQueue = messageUpdateQueue;
            //_elasticClient = elasticClient;
        }

        [HttpGet("SendMessage")]
        public async Task<IActionResult> SendMessageAsync()
        {
            var _client = await _tgclientService.GetClient();
            var chats = await _client.Messages_GetAllChats();
            Console.WriteLine("This user has joined the following:");
            foreach (var (id, chat) in chats.chats)
                if (chat.IsActive)
                    Console.WriteLine($"{id,10}: {chat}");
            return Ok();
        }

        [HttpGet("GetMessagesByPeriod")]
        public async Task<IActionResult> GetMessagesByPeriodAsync(long channelId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var _client = await _tgclientService.GetClientByTelegramId(channelId);
                if (_client == null)
                {
                    return NotFound("No active client found for that channel");
                }

                var channelInfo = await _tgclientService.GetChannelAccessHash(channelId, _client);
                if (!channelInfo.HasValue)
                {
                    return NotFound("Channel not found or access hash unavailable.");
                }

                var (resolvedChannelId, accessHash) = channelInfo.Value;
                var inputPeer = new InputPeerChannel(resolvedChannelId, accessHash);

                var allMessages = new List<TL.Message>();
                int limit = 100;
                DateTime offsetDate = DateTime.UtcNow;
                int lastMessageId = 0;

                // Fetch messages from Telegram
                while (offsetDate > startDate)
                {
                    var messagesBatch = await _client.Messages_GetHistory(inputPeer, offset_id: lastMessageId, limit: limit, offset_date: offsetDate);
                    var messages = messagesBatch.Messages.OfType<TL.Message>()
                        .Where(m => m.Date >= startDate && m.Date <= endDate)
                        .ToList();

                    if (messages.Count == 0) break;
                    allMessages.AddRange(messages);
                    var earliestMessageInBatch = messages.OrderBy(m => m.Date).FirstOrDefault();

                    if (earliestMessageInBatch == null || earliestMessageInBatch.Date <= startDate) break;

                    offsetDate = earliestMessageInBatch.Date;
                    lastMessageId = earliestMessageInBatch.id;
                }
                try
                {
                    await _messageUpdateQueue.QueueMessagesForUpdateAsync(allMessages);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                // Queue messages for background update
               

                // Return fetched messages
                return Ok(allMessages.Select(m => new { m.id, m.Date, m.views, m.reactions }).OrderByDescending(m => m.Date));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve messages by period: {ex.Message}");
                return StatusCode(500, "An error occurred while attempting to fetch messages by period.");
            }
        }

        [HttpGet("GetMessagesByIds")]
        public async Task<IActionResult> GetMessagesByIdsAsync(long channelId, [FromQuery] List<int> messageIds)
        {
            if (messageIds == null || !messageIds.Any())
            {
                return BadRequest("Message IDs are required.");
            }

            try
            {
               
                var _client = await _tgclientService.GetClientByTelegramId(channelId);
                channelId = TelegramIdConverter.ToWTelegramClient(channelId);
                if (_client == null)
                {
                    return NotFound("No active client found for that channel");
                }

                // WTelegramClient's method to get messages by IDs
                var channelInfo = await _tgclientService.GetChannelAccessHash(channelId, _client);
                var (resolvedChannelId, accessHash) = channelInfo.Value;
                var result = await _client.Channels_GetMessages(new InputChannel(resolvedChannelId, accessHash), 
                    messageIds.Select(x => new InputMessageID() {id = x}).ToArray());
                if (result is not TL.Messages_MessagesBase messagesBase)
                {
                    return NotFound("Messages not found.");
                }

                var messages = messagesBase.Messages;
                //var views = await _client.Messages_GetMessagesViews(new InputChannel(resolvedChannelId, accessHash), messageIds.ToArray(), true);
                //var response = views.views;

                //Depending on your needs, you might return the messages directly,
                //or transform them into a DTO to hide certain properties or to better fit your response model.
                var response = messages.Select(m => new
                {
                    m.ID,
                    Date = m.Date,
                    FromId = m.From,
                    // Add other properties as needed
                });


                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpGet("{query}")]
        //public async Task<IActionResult> Search(string query)
        //{
        //    var response = await _elasticClient.SearchAsync<Models.BaseModels.Channel>(s => s
        //            .Query(q => q
        //                .Bool(b => b
        //                    .Should(sh => sh
        //                        .Match(m => m
        //                            .Field(f => f.Description.Suffix("default_stemmed"))
        //                            .Query(query)
        //                            .Analyzer("default_russian")
        //                            .Boost(3.0)
        //                        ),
        //                        sh => sh
        //                        .Match(m => m
        //                            .Field(f => f.Description.Suffix("snowball_stemmed"))
        //                            .Query(query)
        //                            .Analyzer("snowball_russian")
        //                            .Boost(2.0)
        //                        ),
        //                        sh => sh
        //                        .Match(m => m
        //                            .Field(f => f.Description.Suffix("ngram"))
        //                            .Query(query)
        //                            .Analyzer("ngram_russian")
        //                            .Boost(1.0)
        //                        )
        //                    )
        //                )
        //            )
        //            .Sort(srt => srt
        //                .Descending(SortSpecialField.Score)
        //            )
        //        );

        //    if (!response.IsValid)
        //    {
        //        return BadRequest(response.OriginalException.Message);
        //    }

        //    return Ok(response.Documents);
        //}

        //[HttpPost("GetRoot")]
        //public ActionResult<string> GetRoot(string word)
        //{
        //    if (word == null || string.IsNullOrWhiteSpace(word))
        //    {
        //        return BadRequest("Please provide a valid word.");
        //    }

        //    var root = Porter.Stemm(word);

        //    return Ok(root);
        //}
    }
}
