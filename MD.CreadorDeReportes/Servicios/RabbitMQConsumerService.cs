using MD.CreadorDeReportes.Interfaces;
using MD.CreadorDeReportes.Modelos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MD.CreadorDeReportes.Servicios
{
    public class RabbitMQConsumerService : IRabbitMQConsumerService
    {
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IConfiguration _configuration;

        public RabbitMQConsumerService(ILogger<RabbitMQConsumerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Obsolete]
        public List<ReporteModel> ObtenerReportesPorTipo(string lineaNegocio, string nombreReporte)
        {
            try
            {
                List<ReporteModel> lstMessage = new List<ReporteModel>();
                var factory = new ConnectionFactory()
                {
                    HostName = _configuration["RabbitMQOptions:HostName"],
                    Port = AmqpTcpEndpoint.UseDefaultPort,
                    UserName = _configuration["RabbitMQOptions:UserName"],
                    Password = _configuration["RabbitMQOptions:Password"],
                    VirtualHost = _configuration["RabbitMQOptions:VirtualHost"]
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var queue = $"{lineaNegocio}_{nombreReporte}";
                        var queueDeclareResponse = channel.QueueDeclare(queue, true, false, false, null);

                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(queue, true, consumer);

                        for (int i = 0; i < queueDeclareResponse.MessageCount; i++)
                        {
                            var ea = consumer.Queue.Dequeue();
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);

                            if(!string.IsNullOrEmpty(message))
                                lstMessage.Add(JsonConvert.DeserializeObject<ReporteModel>(message));
                        }

                        //channel.QueueDelete(queue);
                    }
                }

                return lstMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener los mensajes para el reporte {nombreReporte}, el error fue {ex.Message}");
                return new List<ReporteModel>();
            }
        }
    }
}