using System.ComponentModel.DataAnnotations;

namespace CloudInventoryPro.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        public string Nombre { get; set; }

        public bool Estado { get; set; }
    }
}