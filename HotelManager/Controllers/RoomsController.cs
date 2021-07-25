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
    /// Manages rooms in the system.
    /// </summary>
    [Authorize(Roles = "User, Admin")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Connects with the databse.
        /// </summary>
        /// <param name="context">Instance of the database.</param>
        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        /// <summary>
        /// Shows all rooms that are contained inthe database.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rooms.ToListAsync());
        }

        // GET: Rooms/Details/5
        /// <summary>
        /// Shows the details of an existing room from the database by ID.
        /// </summary>
        /// <param name="id">Unique room ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [Authorize(Roles = "Admin")]
        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Adds a new room to the database.
        /// </summary>
        /// <param name="room">A model of a room.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Capacity,RoomType,IsFree,AdultBedPrice,ChildBedPrice,RoomNumber")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        /// <summary>
        /// Edits information about a room by ID.
        /// </summary>
        /// <param name="id">Unique room ID.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        /// <summary>
        /// Edits information about a room by ID and given model.
        /// </summary>
        /// <param name="id">Unique room ID.</param>
        /// <param name="room">The model of an existing room.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Capacity,RoomType,IsFree,AdultBedPrice,ChildBedPrice,RoomNumber")] Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
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
            return View(room);
        }

        /// <summary>
        /// Deletes a room by ID.
        /// </summary>
        /// <param name="id">Unique room ID.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        /// <summary>
        /// Deletes a room by ID.
        /// </summary>
        /// <param name="id">Unique room ID.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a room exists in the database.
        /// </summary>
        /// <param name="id">Unique room ID.</param>
        /// <returns></returns>
        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
