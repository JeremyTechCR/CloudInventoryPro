using CloudInventoryPro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CloudInventoryPro.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.Productos.ToList();

            var categorias = _context.Categorias
                .Include(c => c.Productos)
                .ToList();

            ViewBag.TotalProductos = productos.Count;

            ViewBag.TotalCategorias = categorias.Count;

            ViewBag.StockTotal = productos.Sum(p => p.Stock);

            ViewBag.ValorInventario = productos.Sum(p => p.Precio * p.Stock);

            ViewBag.ProductosBajoStock = productos.Count(p => p.Stock <= 5);

            int productosSaludables = productos.Count(p => p.Stock > 5);

            double saludInventario = 0;

            if (productos.Count > 0)
            {
                saludInventario =
                    ((double)productosSaludables / productos.Count) * 100;
            }

            ViewBag.SaludInventario = saludInventario;

            ViewBag.CategoriasGrafico =
                JsonSerializer.Serialize(
                    categorias.Select(c => c.Nombre));

            ViewBag.ValorPorCategoria =
                JsonSerializer.Serialize(
                    categorias.Select(c =>
                        c.Productos.Sum(p => p.Precio * p.Stock)));

            ViewBag.CantidadProductos =
                JsonSerializer.Serialize(
                    categorias.Select(c => c.Productos.Count()));

            ViewBag.ListaBajoStock = productos
                .Where(p => p.Stock <= 5)
                .OrderBy(p => p.Stock)
                .ToList();

            ViewBag.TopProductosCaros = productos
                .OrderByDescending(p => p.Precio)
                .Take(5)
                .ToList();

            ViewBag.UltimosProductos = productos
                .OrderByDescending(p => p.FechaRegistro)
                .Take(5)
                .ToList();

            var productosPorFecha = productos
                .GroupBy(p => p.FechaRegistro.Date)
                .Select(g => new
                {
                    Fecha = g.Key.ToString("dd/MM/yyyy"),
                    Cantidad = g.Count()
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            ViewBag.FechasProductos =
                JsonSerializer.Serialize(
                    productosPorFecha.Select(p => p.Fecha));

            ViewBag.CantidadProductosPorFecha =
                JsonSerializer.Serialize(
                    productosPorFecha.Select(p => p.Cantidad));

     
            var productoTop = productos
    .OrderByDescending(p => p.Precio * p.Stock)
    .FirstOrDefault();

            ViewBag.ProductoTop =
                productoTop?.Nombre ?? "N/A";

            ViewBag.ProductoTopValor =
                productoTop != null
                    ? productoTop.Precio * productoTop.Stock
                    : 0;

            var categoriaTop = categorias
     .Select(c => new
    {
        Nombre = c.Nombre,
        ValorTotal = c.Productos.Sum(p => p.Precio * p.Stock)
    })
    .OrderByDescending(c => c.ValorTotal)
    .FirstOrDefault();

            ViewBag.CategoriaTop =
                categoriaTop?.Nombre ?? "N/A";

            ViewBag.CategoriaTopValor =
                categoriaTop?.ValorTotal ?? 0;

            ViewBag.RankingProductos = productos
    .OrderByDescending(p => p.Precio * p.Stock)
    .Take(10)
    .ToList();

            var topCategoriasValor = categorias
     .Select(c => new
     {
         Nombre = c.Nombre,
         ValorTotal = c.Productos.Sum(p => p.Precio * p.Stock)
     })
     .OrderByDescending(c => c.ValorTotal)
     .Take(5)
     .ToList();

            ViewBag.TopCategoriasValor = topCategoriasValor;

            ViewBag.NombresCategoriasTop =
                JsonSerializer.Serialize(
                    topCategoriasValor.Select(c => c.Nombre));

            ViewBag.ValoresCategoriasTop =
                JsonSerializer.Serialize(
                    topCategoriasValor.Select(c => c.ValorTotal));


            // CLIENTES

            var clientes = _context.Clientes.ToList();

            ViewBag.TotalClientes =
                clientes.Count();

            ViewBag.ClientesActivos =
                clientes.Count(c => c.Estado);

            ViewBag.ClientesInactivos =
                clientes.Count(c => !c.Estado);

            ViewBag.ClientesNuevosMes =
                clientes.Count(c =>
                    c.FechaRegistro.Month == DateTime.Now.Month &&
                    c.FechaRegistro.Year == DateTime.Now.Year);

            return View();
        }
    }
}