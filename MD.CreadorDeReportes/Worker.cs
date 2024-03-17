using MD.CreadorDeReportes.Interfaces;

namespace MD.CreadorDeReportes
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Inicio de proceso de crear reportes: {time}", DateTimeOffset.Now);

                using (var scopeObtenerLecturas = _serviceProvider?.CreateScope())
                {
                    scopeObtenerLecturas.ServiceProvider.GetRequiredService<IReporte>().ProcesoReportes(_configuration["RabbitMQOptions:LineaNegocio"]);
                }

                await Task.Delay(Convert.ToInt32(_configuration["TiempoEjecucion"]), stoppingToken);
            }
        }
    }
}