using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD.CreadorDeReportes.Modelos
{
    [Table("catTipoReporte", Schema = "dbo")]
    public class CatTipoReporte
    {
        public CatTipoReporte()
        {
            CtrlReporteColumnas = new HashSet<CtrlReporteColumnas>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? NombreReporte { get; set; }

        [Required]
        [StringLength(50)]
        public string? NombreArchivo { get; set; }

        [Required]
        [StringLength(200)]
        public string? QueryReporte { get; set; }

        [InverseProperty("TipoReporte")]
        public virtual ICollection<CtrlReporteColumnas> CtrlReporteColumnas { get; set; }
    }
}
