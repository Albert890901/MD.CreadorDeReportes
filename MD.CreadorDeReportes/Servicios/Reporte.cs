using ClosedXML.Excel;
using log4net;
using MD.CreadorDeReportes.Interfaces;
using MD.CreadorDeReportes.Modelos;
using System.Data;
using System.IO.Compression;

namespace MD.CreadorDeReportes.Servicios
{
    public class Reporte : IReporte
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Reporte));
        private IDBAcciones _dbAcciones;
        private IRabbitMQConsumerService _consumerService;
        private IConfiguration _configuration;
        private IEmail _email;

        public Reporte(IDBAcciones dBAcciones, IRabbitMQConsumerService consumerService, IConfiguration configuration, IEmail email)
        {
            _dbAcciones = dBAcciones;
            _consumerService = consumerService;
            _configuration = configuration;
            _email = email;
        }

        public void ProcesoReportes(string lineaNegocio)
        {
            var lstTipoReportes = _dbAcciones.ObtenerTipoReportes();

            foreach (var t in lstTipoReportes)
            {
                var lstReporteColumnas = _dbAcciones.ObtenerColumnasReporte(t.Id);
                GenerarReportes(lineaNegocio, t.NombreReporte, lstReporteColumnas);
            }
        }

        public void GenerarReportes(string lineaNegocio, string nombreReporte, List<CtrlReporteColumnas> lstReporteColumnas)
        {
            var lstReportes = _consumerService.ObtenerReportesPorTipo(lineaNegocio, nombreReporte);

            Dictionary<string, DataTable> dcResultadosConsultas = new Dictionary<string, DataTable>();
            DataTable dtResultadosTotales;

            foreach (var reporte in lstReportes)
            {
                dcResultadosConsultas = new Dictionary<string, DataTable>();
                foreach (var consultas in reporte.Query.Consulta.GroupBy(x => x.Hoja))
                {
                    dtResultadosTotales = new DataTable();
                    foreach (var consulta in consultas)
                        dtResultadosTotales.Merge(_dbAcciones.ResultadoQuery(consulta.Query));

                    foreach (var columna in lstReporteColumnas)
                        dtResultadosTotales.Columns[columna.NombreColumna].ColumnName = columna.NombreColumnaReporte;

                    dcResultadosConsultas.Add(consultas.Key, dtResultadosTotales);
                }

                var resultadoArchivo = false;

                switch (reporte.Extencion)
                {
                    case "xlsx":
                        resultadoArchivo = CrearArchivoExcel(reporte, dcResultadosConsultas);
                        break;
                    case "csv":
                        resultadoArchivo = CrearArchivoCSV(reporte, dcResultadosConsultas);
                        break;
                }

                _email.EnviarCorreo(reporte.Correo, $"Reporte {reporte.NombreArchivo}", "Reporte generado exitosamente");
            }
        }

        public bool CrearArchivoExcel(ReporteModel reporte, Dictionary<string, DataTable> dcResultadosConsultas)
        {
            try
            {
                XLWorkbook wb = new XLWorkbook();

                foreach (var hoja in dcResultadosConsultas)
                    wb.Worksheets.Add(hoja.Value, hoja.Key);

                wb.SaveAs($"C:\\Publicado\\{reporte.NombreArchivo} {reporte.Ejercicio}.{reporte.Extencion}");

                // Esperar instrucciones de cloud
                //using (var ms = new MemoryStream())
                //{
                //    wb.SaveAs(ms);
                //    byte[] workbookBytes = ms.ToArray();
                //}
                

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }

        public bool CrearArchivoCSV(ReporteModel reporte, Dictionary<string, DataTable> dcResultadosConsultas)
        {
            try
            {
                DirectoryInfo di = null;
                string rutaCarpeta = $"{_configuration["RutaArchivosCSV"]}{reporte.Negocio}_{reporte.NombreArchivo
                    .Replace(" ", "")}_{reporte.IdUsuario}\\";

                if (!Directory.Exists(rutaCarpeta))
                    di = Directory.CreateDirectory(rutaCarpeta);

                foreach (var resultado in dcResultadosConsultas)
                {
                    string ruta = $"{rutaCarpeta}{reporte.NombreArchivo} {resultado.Key} {reporte.Ejercicio}.{reporte.Extencion}";

                    StreamWriter sw = new StreamWriter(ruta, false);

                    for (int i = 0; i < resultado.Value.Columns.Count; i++)
                    {
                        sw.Write(resultado.Value.Columns[i]);
                        if (i < resultado.Value.Columns.Count - 1)
                            sw.Write(",");
                    }
                    sw.Write(sw.NewLine);

                    foreach (DataRow dr in resultado.Value.Rows)
                    {
                        for (int i = 0; i < resultado.Value.Columns.Count; i++)
                        {
                            if (!Convert.IsDBNull(dr[i]))
                            {
                                string value = dr[i].ToString();
                                if (value.Contains(','))
                                {
                                    value = String.Format("\"{0}\"", value);
                                    sw.Write(value);
                                }
                                else
                                    sw.Write(dr[i].ToString());
                            }

                            if (i < resultado.Value.Columns.Count - 1)
                                sw.Write(",");

                        }
                        sw.Write(sw.NewLine);
                    }
                    sw.Close();
                }

                var rutaArchivoZip = $"{_configuration["RutaArchivosCSV"]}";
                var nombreArchivoZip = $"{reporte.Negocio} {reporte.NombreArchivo.Replace(" ", "")} {reporte.IdUsuario}.zip";
                ZipFile.CreateFromDirectory(rutaCarpeta, $"{rutaArchivoZip}{nombreArchivoZip}");

                //Proceso para subir el archivo al bucket.
                var byteArchivoZip = File.ReadAllBytes($"{rutaArchivoZip}{nombreArchivoZip}");

                Directory.Delete(rutaCarpeta, true);
                File.Delete($"{rutaArchivoZip}{nombreArchivoZip}");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }
    }
}
