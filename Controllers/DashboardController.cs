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

            ViewBag.ValorInventario =
    _context.Productos
        .Sum(p => p.Precio * p.Stock);

            ViewBag.ProductosBajoStock =
                _context.Productos
                .Count(p => p.Stock <= 5);

            var categorias = _context.Categorias
    .Include(c => c.Productos)
    .ToList();

            ViewBag.CategoriasGrafico =
    JsonSerializer.Serialize(
        categorias.Select(c => c.Nombre));

            ViewBag.ValorPorCategoria =
                JsonSerializer.Serialize(
                    categorias.Select(c =>
                        c.Productos.Sum(p =>
                            p.Precio * p.Stock)));

            ViewBag.CantidadProductos =
    JsonSerializer.Serialize(
        categorias.Select(c =>
            c.Productos.Count()));

            ViewBag.ListaBajoStock = _context.Productos
    .Where(p => p.Stock <= 5)
    .OrderBy(p => p.Stock)
    .ToList();

            ViewBag.TopProductosCaros = _context.Productos
    .OrderByDescending(p => p.Precio)
    .Take(5)
    .ToList();

            ViewBag.UltimosProductos = _context.Productos
    .OrderByDescending(p => p.FechaRegistro)
    .Take(5)
    .ToList();

            int totalProductos =
            _context.Productos.Count();

            int productosSaludables =
                _context.Productos.Count(p => p.Stock > 5);

            double saludInventario = 0;

            if (totalProductos > 0)
            {
                saludInventario =
                    ((double)productosSaludables / totalProductos) * 100;
            }

            ViewBag.SaludInventario =
                saludInventario;

            var productosPorFecha = _context.Productos
    .GroupBy(p => p.FechaRegistro.Date)
    .Select(g => new
    {
        Fecha = g.Key.ToString("dd/MM/yyyy"),
        Cantidad = g.Count()
    })
    .ToList();

            ViewBag.FechasProductos =
                JsonSerializer.Serialize(
                    productosPorFecha.Select(p => p.Fecha));

            ViewBag.CantidadProductosPorFecha =
                JsonSerializer.Serialize(
                    productosPorFecha.Select(p => p.Cantidad));

            ViewBag.TopCategoriasValor = _context.Categorias
    .Include(c => c.Productos)
    .Select(c => new
    {
        Nombre = c.Nombre,
        ValorTotal = c.Productos.Sum(p => p.Precio * p.Stock)
    })
    .OrderByDescending(c => c.ValorTotal)
    .Take(5)
    .ToList();

            return View();

        }
    }
}