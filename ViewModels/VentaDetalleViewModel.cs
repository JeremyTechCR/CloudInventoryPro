using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudInventoryPro.ViewModels
{
    public class VentaDetalleViewModel
    {
        public int IdCliente { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        public List<SelectListItem>? Clientes { get; set; }

        public List<SelectListItem>? Productos { get; set; }
    }
}