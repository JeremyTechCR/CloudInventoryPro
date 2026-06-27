using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudInventoryPro.ViewModels
{
    public class VentaCompletaVM
    {
        public int IdCliente { get; set; }

        public List<SelectListItem>? Clientes { get; set; }

        public List<SelectListItem>? Productos { get; set; }

        public List<DetalleVentaVM>? Detalles { get; set; } =
            new List<DetalleVentaVM>();

        public decimal Total { get; set; }
    }
}