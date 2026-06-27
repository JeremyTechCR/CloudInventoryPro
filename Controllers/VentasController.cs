using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudInventoryPro.Models;
using CloudInventoryPro.Data;
using CloudInventoryPro.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CloudInventoryPro.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = _context.Ventas
                .Include(v => v.Cliente);

            return View(await ventas.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["IdCliente"] = new SelectList(
                _context.Clientes,
                "IdCliente",
                "Nombre"
            );

            return View();
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVenta,FechaVenta,Total,IdCliente")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes,
                "IdCliente",
                "Nombre",
                venta.IdCliente
            );

            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);

            if (venta == null)
            {
                return NotFound();
            }

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes,
                "IdCliente",
                "Nombre",
                venta.IdCliente
            );

            return View(venta);
        }

        // POST: Ventas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVenta,FechaVenta,Total,IdCliente")] Venta venta)
        {
            if (id != venta.IdVenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.IdVenta))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCliente"] = new SelectList(
                _context.Clientes,
                "IdCliente",
                "Nombre",
                venta.IdCliente
            );

            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetalleVentas)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta != null)
            {
                if (venta.DetalleVentas != null)
                {
                    foreach (var detalle in venta.DetalleVentas)
                    {
                        var producto = await _context.Productos
                            .FirstOrDefaultAsync(p => p.IdProducto == detalle.IdProducto);

                        if (producto != null)
                        {
                            producto.Stock += detalle.Cantidad;
                        }
                    }

                    _context.DetalleVentas.RemoveRange(venta.DetalleVentas);
                }

                _context.Ventas.Remove(venta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Ventas/NuevaVentaCompleta
        public IActionResult NuevaVentaCompleta()
        {
            var vm = new VentaCompletaVM
            {
                Clientes = _context.Clientes
                    .Where(c => c.Estado)
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCliente.ToString(),
                        Text = c.Nombre
                    }).ToList(),

                Productos = _context.Productos
                    .Where(p => p.Stock > 0)
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdProducto.ToString(),
                        Text = p.Nombre
                    }).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> GenerarFactura(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
                return NotFound();

            var detalles = await _context.DetalleVentas
                .Include(d => d.Producto)
                .Where(d => d.IdVenta == id)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("CloudInventoryPro")
                        .FontSize(24)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Factura #: {venta.IdVenta}");
                        col.Item().Text($"Fecha: {venta.FechaVenta:dd/MM/yyyy HH:mm}");
                        col.Item().Text($"Cliente: {venta.Cliente.Nombre}");
                        col.Item().Text($"Correo: {venta.Cliente.Correo}");

                        col.Item().PaddingVertical(10);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Producto").Bold();
                                header.Cell().Text("Cantidad").Bold();
                                header.Cell().Text("Precio").Bold();
                                header.Cell().Text("Subtotal").Bold();
                            });

                            foreach (var item in detalles)
                            {
                                table.Cell().Text(item.Producto.Nombre);
                                table.Cell().Text(item.Cantidad.ToString());
                                table.Cell().Text($"₡ {item.PrecioUnitario:N2}");
                                table.Cell().Text($"₡ {item.Subtotal:N2}");
                            }
                        });

                        col.Item()
                            .AlignRight()
                            .Text($"TOTAL: ₡ {venta.Total:N2}")
                            .Bold()
                            .FontSize(18);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Gracias por su compra");
                });
            });

            var stream = new MemoryStream();

            pdf.GeneratePdf(stream);

            return File(
                stream.ToArray(),
                "application/pdf",
                $"Factura_{venta.IdVenta}.pdf");
        }


        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.IdVenta == id);
        }

        public IActionResult CrearVentaDetalle()
        {
            var viewModel = new VentaDetalleViewModel
            {
                Clientes = _context.Clientes
                    .Where(c => c.Estado)
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCliente.ToString(),
                        Text = c.Nombre
                    })
                    .ToList(),

                Productos = _context.Productos
                    .Where(p => p.Stock > 0)
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdProducto.ToString(),
                        Text = p.Nombre + " - Stock: " + p.Stock
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVentaDetalle(VentaDetalleViewModel viewModel)
        {
            if (viewModel.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a cero.");
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == viewModel.IdProducto);

            if (producto == null)
            {
                ModelState.AddModelError("IdProducto", "Producto no encontrado.");
            }
            else if (viewModel.Cantidad > producto.Stock)
            {
                ModelState.AddModelError("Cantidad", "No hay suficiente stock disponible.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Clientes = _context.Clientes
                    .Where(c => c.Estado)
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdCliente.ToString(),
                        Text = c.Nombre
                    })
                    .ToList();

                viewModel.Productos = _context.Productos
                    .Where(p => p.Stock > 0)
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdProducto.ToString(),
                        Text = p.Nombre + " - Stock: " + p.Stock
                    })
                    .ToList();

                return View(viewModel);
            }

            decimal subtotal = producto.Precio * viewModel.Cantidad;

            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                IdCliente = viewModel.IdCliente,
                Total = subtotal
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            var detalle = new DetalleVenta
            {
                IdVenta = venta.IdVenta,
                IdProducto = producto.IdProducto,
                Cantidad = viewModel.Cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = subtotal
            };

            _context.DetalleVentas.Add(detalle);

            producto.Stock -= viewModel.Cantidad;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}