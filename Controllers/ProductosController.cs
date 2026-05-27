using CloudInventoryPro.Data;
using CloudInventoryPro.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloudInventoryPro.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductosController(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // LISTAR PRODUCTOS
        public IActionResult Index()
        {
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .ToList();

            ViewBag.Categorias =
    _context.Categorias.ToList();

            return View(productos);
        }

        // BUSCAR PRODUCTOS AJAX
        [HttpGet]
        public IActionResult Buscar(
    string termino,
    string categoria)
        {
            var productos = _context.Productos
      .Include(p => p.Categoria)

      .Where(p =>

          (string.IsNullOrEmpty(termino)
          || p.Nombre.Contains(termino)
          || p.Descripcion.Contains(termino)
          || p.Categoria.Nombre.Contains(termino))

          &&

          (string.IsNullOrEmpty(categoria)
          || p.Categoria.Nombre == categoria)
      )

      .Select(p => new
      {
          p.IdProducto,
          p.Nombre,
          p.Precio,
          p.Stock,
          Categoria = p.Categoria.Nombre,
          p.Imagen
      })

      .ToList();

            return Json(productos);
        }

        // GET CREATE
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(
                _context.Categorias.ToList(),
                "IdCategoria",
                "Nombre");

            return View();
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            Producto producto,
            IFormFile archivoImagen)
        {
            if (ModelState.IsValid)
            {
                if (archivoImagen != null &&
                    archivoImagen.Length > 0)
                {
                    string nombreArchivo =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(archivoImagen.FileName);

                    string ruta =
                        Path.Combine(
                            _environment.WebRootPath,
                            "img",
                            nombreArchivo);

                    using (var stream =
                           new FileStream(ruta, FileMode.Create))
                    {
                        archivoImagen.CopyTo(stream);
                    }

                    producto.Imagen =
                        "/img/" + nombreArchivo;
                }

                producto.FechaRegistro =
                    DateTime.Now;

                _context.Productos.Add(producto);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias =
                new SelectList(
                    _context.Categorias.ToList(),
                    "IdCategoria",
                    "Nombre");

            return View(producto);
        }
    }
}