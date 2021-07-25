using HotelManager.Data;
using HotelManager.Models;
using HotelManager.Views.User.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace HotelManager.Controllers
{
    /// <summary>
    /// Manages users in the system.
    /// </summary>
   [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserEmployee> userManager;        
        private readonly RoleManager<IdentityRole> roleManager;
        
        /// <summary>
        /// Creates a manager for roles, users and sign in.
        /// </summary>
        /// <param name="context">The database connection.</param>
        /// <param name="userManager">Manager for users.</param>
        /// <param name="signInManager">Manager for signing in.</param>
        /// <param name="roleManager">Manager for roles.</param>
        public UserController(ApplicationDbContext context, UserManager<UserEmployee> userManager,
           SignInManager<UserEmployee> signInManager,
           RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        // GET: Rooms
        /// <summary>
        /// Shows all users that are contained in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserEmployees.ToListAsync());
        }

        /// <summary>
        /// Shows the details of an existing user from the database by ID.
        /// </summary>
        /// <param name="id">Unique user ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userEmployees = await _context.UserEmployees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEmployees == null)
            {
                return NotFound();
            }

            return View(userEmployees);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="model">A model of an user.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model)
        {
            var userEmployee = new UserEmployee
            {
                UserName = model.Email,
                FisrtName = model.FisrtName,
                FatherName = model.FatherName,
                LastName = model.LastName,
                EGN = model.EGN,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                StartingDate = model.StartingDate,
                ActiveUser = model.ActiveUser,
            };
            if (ModelState.IsValid)
            {
                var result = await userManager.CreateAsync(userEmployee, model.Password);
                if (model.ActiveUser)
                {
                    await Promote(userEmployee.Id);
                }
                else
                {
                    await Demote(userEmployee.Id);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userEmployee);
        }

        // GET: /Edit/5
        /// <summary>
        /// Edits information about an user by ID.
        /// </summary>
        /// <param name="id">Unique user ID.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("Error");
            }

            var model = new User
            {
                UserName = user.Email,
                FisrtName = user.FisrtName,
                FatherName = user.FatherName,
                LastName = user.LastName,
                EGN = user.EGN,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                StartingDate = user.StartingDate,
                ActiveUser = user.ActiveUser,
                Password = user.PasswordHash
            };
            return View(model);
        }

        /// <summary>
        /// Edits information about a reservation by given model.
        /// </summary>
        /// <param name="model">A model of an user.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(User model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.Id} cannot be found";
                return View("Error");
            }
            else
            {
                user.UserName = model.Email;
                user.FisrtName = model.FisrtName;
                user.FatherName = model.FatherName;
                user.LastName = model.LastName;
                user.EGN = model.EGN;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.StartingDate = model.StartingDate;
                user.ActiveUser = model.ActiveUser;
                    
                var result = await userManager.UpdateAsync(user);

                if (model.ActiveUser)
                {
                    await Promote(user.Id);
                }
                else
                {
                    await Demote(user.Id);
                }

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        // GET: Clients/Delete/5
        /// <summary>
        /// Deletes an user by ID.
        /// </summary>
        /// <param name="id">Unique user ID.</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userEmployees = await _context.UserEmployees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEmployees == null)
            {
                return NotFound();
            }

            return View(userEmployees);
        }

        // POST: Clients/Delete/5
        /// <summary>
        /// Deletes an user by ID.
        /// </summary>
        /// <param name="id">Unique user ID.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userEmployees = await _context.UserEmployees.FindAsync(id);
            _context.UserEmployees.Remove(userEmployees);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Confirms that an user is an employee and gives access to information.
        /// </summary>
        /// <param name="userId">Unique hashed user ID.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Promote(string userId)
        {
            if (userId == null)
            {
                return this.RedirectToAction("Index");
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null || await this.userManager.IsInRoleAsync(user, "User"))
            {
                return this.RedirectToAction("Index");
            }

            await this.userManager.AddToRoleAsync(user, "User");
            user.ActiveUser = true;
            await _context.SaveChangesAsync();
            return this.RedirectToAction("Index");

        }

        /// <summary>
        /// Declines that an user is an employee and takes access from information.
        /// </summary>
        /// <param name="userId">Unique hashed user ID.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Demote(string userId)
        {
            if (userId == null)
            {
                return this.RedirectToAction("Index");
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null || !await this.userManager.IsInRoleAsync(user, "User"))
            {
                return this.RedirectToAction("Index");
            }

            await this.userManager.RemoveFromRoleAsync(user, "User");
            user.ActiveUser = false;
            await _context.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }
    }

}
