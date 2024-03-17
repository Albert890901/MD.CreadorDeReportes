using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD.CreadorDeReportes.Modelos
{
    [Table("ctrlReporteColumnas", Schema = "dbo")]
    public class CtrlReporteColumnas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public int TipoReporteId { get; set; }

        [Required]
        [StringLength(150)]
        public string? NombreColumna { get; set; }

        [Required]
        [StringLength(150)]
        public string? NombreColumnaReporte { get; set; }

        [ForeignKey(nameof(TipoReporteId))]
        [InverseProperty(nameof(CatTipoReporte.CtrlReporteColumnas))]
        public virtual CatTipoReporte TipoReporte { get; set; }
    }
}
