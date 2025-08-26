using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;

namespace WebApplication1.Services
{
    public class MessageSenderService : BackgroundService
    {
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<MessageSenderService> _logger;

        public MessageSenderService(IHubContext<MessageHub> hubContext, ILogger<MessageSenderService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var message = $"Aktualna data i czas: {currentDateTime}";
                
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", message, stoppingToken);
                
                _logger.LogInformation($"Sent message: {message}");
                
                await Task.Delay(3000, stoppingToken); // Wait 3 seconds
            }
        }
    }
}
