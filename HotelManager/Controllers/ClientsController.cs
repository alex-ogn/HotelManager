using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManager.Data;
using HotelManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelManager.Controllers
{
    /// <summary>
    /// Manages clients in the system.
    /// </summary>
    [Authorize(Roles = "Admin, User")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Connects with the databse.
        /// </summary>
        /// <param name="context">Instance of the database.</param>
        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        /// <summary>
        /// Shows all clients that are contained in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: Clients/Details/5
        /// <summary>
        /// Shows the details of an existing client from the database by ID.
        /// </summary>
        /// <param name="id">Unique client ID.</param>
        /// <returns>An instance containing information about a client.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        /// <summary>
        /// Adds a new client to the database.
        /// </summary>
        /// <param name="client">A model of a client.</param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FisrtName,LastName,PhoneNumber,Email,IsAdult")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        /// <summary>
        /// Edits information about a client by ID.
        /// </summary>
        /// <param name="id">Unique client ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        /// <summary>
        /// Edits information about a client by ID and given model of one.
        /// </summary>
        /// <param name="id">Unique client ID.</param>
        /// <param name="client">The model of an existing client.</param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FisrtName,LastName,PhoneNumber,Email,IsAdult")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        /// <summary>
        /// Deletes a client by ID.
        /// </summary>
        /// <param name="id">Unique client ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        /// <summary>
        /// Deletes a client by ID.
        /// </summary>
        /// <param name="id">Unique client ID.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
