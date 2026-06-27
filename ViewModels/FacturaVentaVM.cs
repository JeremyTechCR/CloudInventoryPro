using CloudInventoryPro.Models;

namespace CloudInventoryPro.ViewModels
{
    public class FacturaVentaVM
    {
        public Venta Venta { get; set; }

        public List<DetalleVenta> Detalles { get; set; }
    }
}