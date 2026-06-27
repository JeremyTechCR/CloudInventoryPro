using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudInventoryPro.Models
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; } =
            DateTime.Now;

        [Required]
        public decimal Total { get; set; }

        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        public ICollection<DetalleVenta>? DetalleVentas { get; set; }

    }
}