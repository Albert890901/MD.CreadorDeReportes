using Microsoft.EntityFrameworkCore;

namespace MD.CreadorDeReportes.Modelos
{
    public class ReportesContext : DbContext
    {
        public ReportesContext() { }

        public ReportesContext(DbContextOptions<ReportesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CatTipoReporte> catTipoReporte { get; set; }
        public virtual DbSet<CtrlReporteColumnas> ctrlReporteColumnas { get; set; }
    }
}
