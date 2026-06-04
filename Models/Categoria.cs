using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CloudInventoryPro.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        public string Nombre { get; set; }

        public bool Estado { get; set; }

        [ValidateNever]
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}