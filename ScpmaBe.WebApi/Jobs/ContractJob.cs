using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Services.Enums;
using ScpmaBe.Services.Helpers;

namespace ScpmaBe.WebApi.Jobs
{
    public class ContractJob : BackgroundService
    {
        private readonly ILogger<ContractJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly TimeSpan _interval = TimeSpan.FromDays(1); // Run once per day

        public ContractJob(
            ILogger<ContractJob> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Contract expiration job started");

            // Calculate time until next midnight
            var nextRunTime = CalculateNextMidnight();

            _logger.LogInformation("First contract expiration check scheduled for: {NextRunTime}", nextRunTime);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait until the next scheduled run time
                var delay = nextRunTime - DateTime.Now.ToVNTime();
                
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                try
                {
                    await UpdateExpiredContracts(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating expired contracts");
                }

                // Set next run time to be 24 hours from now
                nextRunTime = DateTime.Now.ToVNTime().Add(_interval);

                _logger.LogInformation("Next contract expiration check scheduled for: {NextRunTime}", nextRunTime);
            }
        }

        private DateTime CalculateNextMidnight()
        {
            var now = DateTime.Now.ToVNTime();
            return now.Date.AddDays(1); // Next midnight (00:00:00)
        }

        private async Task UpdateExpiredContracts(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Checking for expired contracts");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SCPMContext>();

            var today = DateTime.Now.ToVNTime().ToDateOnly();

            var expiredContracts = await dbContext.Contracts
                                            .Where(c => c.EndDate < today && c.Status != (int)ContractStatus.Expired)
                                            .ToListAsync(stoppingToken);

            if (expiredContracts.Any())
            {
                foreach (var contract in expiredContracts)
                {
                    contract.Status = (int)ContractStatus.Expired;
                    contract.UpdatedDate = DateTime.Now.ToVNTime();
                    contract.Note = "Contract expired";

                    dbContext.ParkingSpaces
                        .Where(p => p.ParkingSpaceId == contract.ParkingSpaceId)
                        .ToList()
                        .ForEach(p => p.Status = (int)ParkingSpaceStatus.Available);

                    _logger.LogInformation("Contract {ContractId} marked as expired", contract.ContractId);
                }

                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Updated {Count} contracts to expired status", expiredContracts.Count);
            }
            else
            {
                _logger.LogInformation("No expired contracts found");
            }
        }
    }
}
