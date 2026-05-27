using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudInventoryPro.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; }

        // Foreign Key
        public int IdCategoria { get; set; }

        [ForeignKey("IdCategoria")]
        [ValidateNever]
        public Categoria? Categoria { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string? Imagen { get; set; }
    }
}