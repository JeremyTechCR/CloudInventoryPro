using System.ComponentModel.DataAnnotations;

namespace CloudInventoryPro.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Correo { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } =
            DateTime.Now;
    }
}