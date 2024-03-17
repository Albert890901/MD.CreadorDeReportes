namespace MD.CreadorDeReportes.Interfaces
{
    public interface IEmail
    {
        void EnviarCorreo(string correo, string asunto, string mensaje);
    }
}
