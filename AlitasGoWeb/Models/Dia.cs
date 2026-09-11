using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlitasGoWeb.Models
{
    public class Dia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDia { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
    }
}
