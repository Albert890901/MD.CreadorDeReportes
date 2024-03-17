using MD.CreadorDeReportes.Interfaces;
using MD.CreadorDeReportes.Modelos;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace MD.CreadorDeReportes.Servicios
{
    public class DBAcciones : IDBAcciones
    {
        private readonly ReportesContext _db;
        private readonly IConfiguration _configuration;

        public DBAcciones(ReportesContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public List<CatTipoReporte> ObtenerTipoReportes()
        {
            return _db.catTipoReporte.ToList();
        }

        public DataTable ResultadoQuery(string query)
        {
            DataTable dtResultados = new DataTable();
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("FabConnection")))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    da.Fill(dtResultados);
                    connection.Close();
                }
                connection.Close();
            }

            return dtResultados;
        }

        public List<CtrlReporteColumnas> ObtenerColumnasReporte(int tipoReporteId)
        {
            return _db.ctrlReporteColumnas.Where(x => x.TipoReporteId == tipoReporteId).ToList();
        }
    }
}
