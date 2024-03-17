using MD.CreadorDeReportes.Modelos;

namespace MD.CreadorDeReportes.Interfaces
{
    public interface IRabbitMQConsumerService
    {
        List<ReporteModel> ObtenerReportesPorTipo(string lineaNegocio, string nombreReporte);
    }
}
