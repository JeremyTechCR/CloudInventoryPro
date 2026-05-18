using CloudInventoryPro.Data;
using CloudInventoryPro.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudInventoryPro.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR CATEGORIAS
        public IActionResult Index()
        {
            var listaCategorias = _context.Categorias.ToList();

            return View(listaCategorias);
        }

        // MOSTRAR FORMULARIO CREATE
        public IActionResult Create()
        {
            return View();
        }

        // GUARDAR CATEGORIA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Add(categoria);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // MOSTRAR FORMULARIO EDITAR
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = _context.Categorias.Find(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GUARDAR EDICION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Update(categoria);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // MOSTRAR CONFIRMACION ELIMINAR
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = _context.Categorias.Find(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // ELIMINAR CATEGORIA
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var categoria = _context.Categorias.Find(id);

            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}