using MD.CreadorDeReportes.Modelos;
using System.Data;

namespace MD.CreadorDeReportes.Interfaces
{
    public interface IDBAcciones
    {
        List<CatTipoReporte> ObtenerTipoReportes();
        List<CtrlReporteColumnas> ObtenerColumnasReporte(int tipoReporteId);
        DataTable ResultadoQuery(string query);
    }
}
