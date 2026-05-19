using CloudInventoryPro.Data;
using CloudInventoryPro.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudInventoryPro.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // MOSTRAR LOGIN
        public IActionResult Index()
        {
            return View();
        }

        // VALIDAR LOGIN
        [HttpPost]
        public IActionResult Index(string correo, string clave)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u =>
                    u.Correo == correo &&
                    u.Clave == clave &&
                    u.Estado == true);

            if (usuario != null)
            {
                return RedirectToAction("Index", "Categorias");
            }

            ViewBag.Error = "Correo o contraseña incorrectos";

            return View();
        }
    }
}