using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HotelManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace HotelManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserEmployee> signInManager;
        private readonly UserManager<UserEmployee> userManager;
        private readonly ILogger<RegisterModel> logger;

        public RegisterModel(
            UserManager<UserEmployee> userManager,
            SignInManager<UserEmployee> signInManager,
            ILogger<RegisterModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
           

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 5)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 3)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Fathers Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 3)]
            public string FathersName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 3)]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "EGN")]
            [StringLength(10, ErrorMessage = "The must 10 characters long.",
                MinimumLength = 10)]
            public string EGN { get; set; }

            [Required]
            [Display(Name = "Telephone number")]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid telephone number")]
            public string TelNumber { get; set; }

        }

        // GET
        public IActionResult OnGet(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        // POST
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                return this.LocalRedirect(returnUrl);
            }

            if (this.ModelState.IsValid)
            {
                var user = new UserEmployee
                {
                    UserName = this.Input.Email,
                    Email = this.Input.Email,
                    FisrtName = this.Input.FirstName,
                    FatherName = this.Input.FathersName,
                    LastName = this.Input.LastName,
                    PhoneNumber = this.Input.TelNumber,
                    EGN = this.Input.EGN

                };
                var result = await this.userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
//{
//    [AllowAnonymous]
//    public class RegisterModel : PageModel
//    {
//        private readonly SignInManager<IdentityUser> _signInManager;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly ILogger<RegisterModel> _logger;
//        private readonly IEmailSender _emailSender;

//        public RegisterModel(
//            UserManager<IdentityUser> userManager,
//            SignInManager<IdentityUser> signInManager,
//            ILogger<RegisterModel> logger,
//            IEmailSender emailSender)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _logger = logger;
//            _emailSender = emailSender;
//        }

//        [BindProperty]
//        public InputModel Input { get; set; }

//        public string ReturnUrl { get; set; }

//        public IList<AuthenticationScheme> ExternalLogins { get; set; }

//        public class InputModel
//        {
//            //az go dobavih
//            [Required]

//            [Display(Name = "FirstName")]
//            public string FirstName { get; set; }

//            [Required]
//            [EmailAddress]
//            [Display(Name = "Email")]
//            public string Email { get; set; }

//            [Required]
//            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
//            [DataType(DataType.Password)]
//            [Display(Name = "Password")]
//            public string Password { get; set; }

//            [DataType(DataType.Password)]
//            [Display(Name = "Confirm password")]
//            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//            public string ConfirmPassword { get; set; }
//        }

//        public async Task OnGetAsync(string returnUrl = null)
//        {
//            ReturnUrl = returnUrl;
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//        }

//        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//        {
//            returnUrl ??= Url.Content("~/");
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//            if (ModelState.IsValid)
//            {
//                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email,/*d*/ FisrtName = Input.FirstName /*d*/};
//                var result = await _userManager.CreateAsync(user, Input.Password);
//                if (result.Succeeded)
//                {
//                    _logger.LogInformation("User created a new account with password.");

//                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//                    var callbackUrl = Url.Page(
//                        "/Account/ConfirmEmail",
//                        pageHandler: null,
//                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
//                        protocol: Request.Scheme);

//                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
//                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

//                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
//                    {
//                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
//                    }
//                    else
//                    {
//                        await _signInManager.SignInAsync(user, isPersistent: false);
//                        return LocalRedirect(returnUrl);
//                    }
//                }
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError(string.Empty, error.Description);
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return Page();
//        }
//    }
//}
