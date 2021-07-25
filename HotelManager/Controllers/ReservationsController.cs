using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManager.Data;
using HotelManager.Models;
using HotelManager.Views.Reservations.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HotelManager.Controllers
{
    /// <summary>
    /// Manages reservations in the system.
    /// </summary>
    [Authorize(Roles = "User, Admin")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Connects with the databse and creates manager for users and for signing in.
        /// </summary>
        /// <param name="context">Instance of the database.</param>
        /// <param name="userManager">Manager for users.</param>
        /// <param name="signInManager">Manager for signing in.</param>
        public ReservationsController(ApplicationDbContext context, UserManager<UserEmployee> userManager,
           SignInManager<UserEmployee> signInManager )
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public readonly SignInManager<UserEmployee> signInManager; //
        private readonly UserManager<UserEmployee> userManager; //
        // GET: Reservations

        /// <summary>
        /// Shows all reservations that are contained in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reservations.Include(r => r.ReservedRoom);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        /// <summary>
        /// Shows the details of an existing reservation from the database by ID.
        /// </summary>
        /// <param name="id">Unique reservation ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.ReservedRoom)
                .FirstOrDefaultAsync(m => m.Id == id);
            var reservationclient = _context.ReservationClient.Where(m => m.ReservationID == id).ToList();
            List<Client> clients = new List<Client>();
            foreach (var item in reservationclient)
            {
                var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == item.ClientID);
                clients.Add(client);
            }

            if (reservation == null)
            {
                return NotFound();
            }
            var reservationromm = _context.Rooms.Where(m => m.Id == reservation.RoomId).First();
            var fullreservation = new FullReservation
            { //Id,RoomId,ClientId,UserEmployeeId,AccomodationDate,ReleaseDate,BreakfastIncluded,IsAllInclusive,FinalPrice
                RoomId = reservationromm.RoomNumber,
                AccomodationDate = reservation.AccomodationDate,
                ReleaseDate = reservation.ReleaseDate,
                BreakfastIncluded = reservation.BreakfastIncluded,
                IsAllInclusive = reservation.IsAllInclusive,
                FinalPrice = reservation.FinalPrice,
               UserEmployeeEmail = reservation.UserEmployeeEmail,
                Client = clients
            };
            return View(fullreservation);
        }

        // GET: Reservations/Create
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber");
            ViewData["UserEmployeeId"] = new SelectList(_context.UserEmployees, "Id", "Email");

            return View();
        }

        // POST: Reservations/Create
        /// <summary>
        /// Adds a new reservation to the database.
        /// </summary>
        /// <param name="fullreservation"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FullReservation fullreservation)
        {
            UserEmployee applicationUser = await userManager.GetUserAsync(User);
            var reservation = new Reservation
            { //Id,RoomId,ClientId,UserEmployeeId,AccomodationDate,ReleaseDate,BreakfastIncluded,IsAllInclusive,FinalPrice
                RoomId = fullreservation.RoomId,
                AccomodationDate = fullreservation.AccomodationDate,
                ReleaseDate = fullreservation.ReleaseDate,
                BreakfastIncluded = fullreservation.BreakfastIncluded,
                IsAllInclusive = fullreservation.IsAllInclusive,
               // UserEmployeeId = fullreservation.UserEmployeeId,
               UserEmployeeEmail = applicationUser.Email,
                FinalPrice = 0
            };

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == fullreservation.RoomId);

            List<string> listWithClientEmails = fullreservation.ClientString.Split(' ').ToList();
            List<int> listWithClientdID = new List<int>();
            var listWithClients = await _context.Clients.ToListAsync();

            for (int i = 0; i < listWithClientEmails.Count; i++)
            {
                for (int j = 0; j < listWithClients.Count; j++)
                {
                    if (listWithClientEmails[i] == listWithClients[j].Email)
                    {
                        listWithClientdID.Add(listWithClients[j].Id);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
               
            }
            else {
                ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", reservation.RoomId);
                ViewData["UserEmployeeId"] = new SelectList(_context.UserEmployees, "Id", "Email", reservation.RoomId);
                return View(fullreservation);
            }
            var roomforprice = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == fullreservation.RoomId);
            int people;
            if (room.Capacity < listWithClientdID.Count)
            {
               // throw new ArgumentException("Too much people");
                people = room.Capacity;
            }
            else
            {
                people = listWithClientdID.Count;
            }
            for (int i = 0; i < people; i++)
            {
                var reservationClient = new ReservationClient
                {
                    ReservationID = reservation.Id, //_context.Reservation.Last().Id,
                    ClientID = listWithClientdID[i]
                };
                var clientchildornot  = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == listWithClientdID[i]);
                if (clientchildornot.IsAdult == true)
                {
                    reservation.FinalPrice += roomforprice.AdultBedPrice;
                }
                else
                {
                    reservation.FinalPrice += roomforprice.ChildBedPrice;
                }
                // reservation.FinalPrice += 1; //Ще го заменим с иф

                _context.Add(reservationClient);
                await _context.SaveChangesAsync();

            }
            if (fullreservation.BreakfastIncluded == true)
            {
                reservation.FinalPrice += 50;
            }
            if (fullreservation.IsAllInclusive == true)
            {
                reservation.FinalPrice += 10;
            }
            double days = (fullreservation.ReleaseDate - fullreservation.AccomodationDate).TotalDays;

            reservation.FinalPrice = reservation.FinalPrice * Math.Round(days);
            reservation.FinalPrice = Math.Round(reservation.FinalPrice, 2);
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            //   ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Id", reservation.RoomId);
            //   ViewData["ClientID"] = new SelectList(_context.Client, "Id", "Email", fullreservation.ClientId;
           // return View(reservation);
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Edit/5
        /// <summary>
        /// Edits information about a reservation by ID.
        /// </summary>
        /// <param name="id">Unique reservation ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            var fullreservation = new FullReservation
            { //Id,RoomId,ClientId,UserEmployeeId,AccomodationDate,ReleaseDate,BreakfastIncluded,IsAllInclusive,FinalPrice
                RoomId = reservation.RoomId,
                AccomodationDate = reservation.AccomodationDate,
                ReleaseDate = reservation.ReleaseDate,
                BreakfastIncluded = reservation.BreakfastIncluded,
                IsAllInclusive = reservation.IsAllInclusive,
              //  UserEmployeeId = reservation.UserEmployeeId,
                FinalPrice = 0
            };

            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", reservation.RoomId);
            ViewData["UserEmployeeId"] = new SelectList(_context.UserEmployees, "Id", "Email", reservation.RoomId);
            return View(fullreservation);
        }

        // POST: Reservations/Edit/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullreservation"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FullReservation fullreservation)
        {
            var reservation = new Reservation
            { //Id,RoomId,ClientId,UserEmployeeId,AccomodationDate,ReleaseDate,BreakfastIncluded,IsAllInclusive,FinalPrice
                RoomId = fullreservation.RoomId,
                AccomodationDate = fullreservation.AccomodationDate,
                ReleaseDate = fullreservation.ReleaseDate,
                BreakfastIncluded = fullreservation.BreakfastIncluded,
                IsAllInclusive = fullreservation.IsAllInclusive,
             //   UserEmployeeId = fullreservation.UserEmployeeId,
                FinalPrice = 0
            };

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == fullreservation.RoomId);

            List<string> listWithClientEmails = fullreservation.ClientString.Split(' ').ToList();
            List<int> listWithClientdID = new List<int>();
            var listWithClients = await _context.Clients.ToListAsync();

            for (int i = 0; i < listWithClientEmails.Count; i++)
            {
                for (int j = 0; j < listWithClients.Count; j++)
                {
                    if (listWithClientEmails[i] == listWithClients[j].Email)
                    {
                        listWithClientdID.Add(listWithClients[j].Id);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();

            }
            else
            {
                ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", reservation.RoomId);
                ViewData["UserEmployeeId"] = new SelectList(_context.UserEmployees, "Id", "Email", reservation.RoomId);
                return View(fullreservation);
            }
            var roomforprice = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == fullreservation.RoomId);

            var listWithReservationClients = await _context.ReservationClient.Where(n => n.ReservationID == reservation.Id).ToListAsync();
            for (int i = 0; i < listWithReservationClients.Count; i++)
            {
                _context.ReservationClient.Remove(listWithReservationClients[i]);
                await _context.SaveChangesAsync();
            }
            int people;
            if (room.Capacity < listWithClientdID.Count)
            {
                // throw new ArgumentException("Too much people");
                people = room.Capacity;
            }
            else
            {
                people = listWithClientdID.Count;
            }
            for (int i = 0; i < people; i++)
            {
                var reservationClient = new ReservationClient
                {
                    ReservationID = reservation.Id, //_context.Reservation.Last().Id,
                    ClientID = listWithClientdID[i]
                };
                var clientchildornot = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == listWithClientdID[i]);
                if (clientchildornot.IsAdult == true)
                {
                    reservation.FinalPrice += roomforprice.AdultBedPrice;
                }
                else
                {
                    reservation.FinalPrice += roomforprice.ChildBedPrice;
                }
                // reservation.FinalPrice += 1; //Ще го заменим с иф

                _context.Add(reservationClient);
                await _context.SaveChangesAsync();

            }
            if (fullreservation.BreakfastIncluded == true)
            {
                reservation.FinalPrice += 50;
            }
            if (fullreservation.IsAllInclusive == true)
            {
                reservation.FinalPrice += 10;
            }
            double days = (fullreservation.ReleaseDate - fullreservation.AccomodationDate).TotalDays;

            reservation.FinalPrice = reservation.FinalPrice * Math.Round(days);
            reservation.FinalPrice = Math.Round(reservation.FinalPrice, 2);
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            //   ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Id", reservation.RoomId);
            //   ViewData["ClientID"] = new SelectList(_context.Client, "Id", "Email", fullreservation.ClientId;
            // return View(reservation);
            return RedirectToAction(nameof(Index));
        
        //if (ModelState.IsValid)
        //{
        //    try
        //    {
        //        _context.Update(reservation);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ReservationExists(reservation.Id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    return RedirectToAction(nameof(Index));
        //}
        //ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", reservation.RoomId);
        //return View(reservation);
    }

        // GET: Reservations/Delete/5
        /// <summary>
        /// Deletes a reservation by ID.
        /// </summary>
        /// <param name="id">Unique reservation ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.ReservedRoom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        /// <summary>
        /// Deletes a reservation by ID.
        /// </summary>
        /// <param name="id">Unique reservation ID.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a reservation exists in the database.
        /// </summary>
        /// <param name="id">Unique reservation ID.</param>
        /// <returns></returns>
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
