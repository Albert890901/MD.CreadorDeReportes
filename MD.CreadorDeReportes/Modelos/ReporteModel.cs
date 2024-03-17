namespace MD.CreadorDeReportes.Modelos
{
    public class ReporteModel
    {
        public int IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public string? Correo { get; set; }
        public string? Negocio { get; set; }
        public int Ejercicio { get; set; }
        public string? NombreArchivo { get; set; }
        public string? Extencion { get; set; }
        public Query? Query { get; set; }
    }

    public class Query
    {
        public List<Consulta>? Consulta { get; set; }
    }

    public class Consulta
    {
        public string? Query { get; set; }
        public string? Hoja { get; set; }
    }
}
