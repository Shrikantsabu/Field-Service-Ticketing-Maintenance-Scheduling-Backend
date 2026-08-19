namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services
{
    public class EscalationBackgroundService : Controller
    {
        private readonly TicketService _ticketService;
        private readonly ILogger<EscalationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);
        public EscalationBackgroundService(TicketService ticketService, ILogger<EscalationBackgroundService> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var escalated = await _ticketService.EscalateOverdueTicketsAsync();
                    if (escalated.Count > 0)
                        _logger.LogInformation($"{escalated.Count} tickets escalated by background job.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during escalation background job.");
                }
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
