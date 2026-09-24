using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    // Bitácora: quién, cuándo y qué cambió (anular, precios, stock, usuarios, accesos denegados).
    public class Auditoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAuditoria { get; set; }

        public DateTime Fecha { get; set; }

        [Required, StringLength(256)]
        public string Usuario { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Accion { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Entidad { get; set; } = string.Empty;

        [StringLength(50)]
        public string? IdEntidad { get; set; }

        [StringLength(500)]
        public string? Detalle { get; set; }
    }
}
