namespace CloudInventoryPro.ViewModels
{
    public class DetalleVentaVM
    {
        public int IdProducto { get; set; }

        public string NombreProducto { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

        public decimal Subtotal
        {
            get { return Cantidad * Precio; }
        }
    }
}