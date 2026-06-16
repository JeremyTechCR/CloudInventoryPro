
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudInventoryPro.Models;
using CloudInventoryPro.Data;

public class ClientesController : Controller
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: CLIENTES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Clientes.ToListAsync());
    }

    // GET: CLIENTES/Details/5
    public async Task<IActionResult> Details(int? idcliente)
    {
        if (idcliente == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(m => m.IdCliente == idcliente);
        if (cliente == null)
        {
            return NotFound();
        }

        return View(cliente);
    }

    // GET: CLIENTES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CLIENTES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdCliente,Nombre,Correo,Telefono,Direccion,Estado,FechaRegistro")] Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            _context.Add(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    // GET: CLIENTES/Edit/5
    public async Task<IActionResult> Edit(int? idcliente)
    {
        if (idcliente == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FindAsync(idcliente);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    // POST: CLIENTES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idcliente, [Bind("IdCliente,Nombre,Correo,Telefono,Direccion,Estado,FechaRegistro")] Cliente cliente)
    {
        if (idcliente != cliente.IdCliente)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(cliente.IdCliente))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    // GET: CLIENTES/Delete/5
    public async Task<IActionResult> Delete(int? idcliente)
    {
        if (idcliente == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(m => m.IdCliente == idcliente);
        if (cliente == null)
        {
            return NotFound();
        }

        return View(cliente);
    }

    // POST: CLIENTES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idcliente)
    {
        var cliente = await _context.Clientes.FindAsync(idcliente);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClienteExists(int? idcliente)
    {
        return _context.Clientes.Any(e => e.IdCliente == idcliente);
    }
}
