using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobUtils
{
    public abstract class BaseWorker(ILogger logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (IsActive())
                {
                    try
                    {
                        await ExecuteWithTimeoutAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"[{GetType().Name}][ExecuteAsync] an error occurred: {ex.Message}");
                        await Task.Delay(GetErrorDelayTime(), stoppingToken);
                        continue;
                    }
                }
                else
                {
                    logger.LogWarning($"[{GetType().Name}][ExecuteAsync] worker is deactivated!");

                }

                await Task.Delay(GetDelayTime(), stoppingToken);
            }
        }

        private async Task ExecuteWithTimeoutAsync(CancellationToken stoppingToken)
        {
            var timeout = GetIterationTimeout();
            if (timeout.HasValue)
            {
                using var cts = new CancellationTokenSource();
                var task = ProcessAsync();
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout.Value, stoppingToken));

                if (completedTask != task)
                {
                    logger.LogWarning($"[{GetType().Name}] Iteration timed out after {timeout.Value}");
                }
                else
                {
                    await task; // Observe any exceptions
                }
            }
            else
            {
                await ProcessAsync();
            }
        }

        protected abstract Task ProcessAsync();
        protected abstract TimeSpan GetDelayTime();
        protected abstract TimeSpan GetErrorDelayTime();
        protected abstract bool IsActive();
        protected abstract TimeSpan? GetIterationTimeout();
    }
}