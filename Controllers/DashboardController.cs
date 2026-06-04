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
            ViewBag.TotalProductos =
                _context.Productos.Count();

            ViewBag.TotalCategorias =
                _context.Categorias.Count();

            ViewBag.StockTotal =
                _context.Productos.Sum(p => p.Stock);

            ViewBag.ProductosBajoStock =
                _context.Productos
                .Count(p => p.Stock <= 5);

            var categorias = _context.Categorias
    .Include(c => c.Productos)
    .ToList();

            ViewBag.CategoriasGrafico =
                JsonSerializer.Serialize(
                    categorias.Select(c => c.Nombre));

            ViewBag.CantidadProductos =
                JsonSerializer.Serialize(
                    categorias.Select(c => c.Productos.Count));

            return View();

        }
    }
}