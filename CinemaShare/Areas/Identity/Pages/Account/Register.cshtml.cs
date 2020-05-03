using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using CinemaShare.Common;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CinemaShare.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CinemaUser> _signInManager;
        private readonly UserManager<CinemaUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly Business.IEmailSender _emailSender;

        public RegisterModel(
            UserManager<CinemaUser> userManager,
            SignInManager<CinemaUser> signInManager,
            ILogger<RegisterModel> logger,
            Business.IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Username")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string Username { get; set; }

            [Required]
            [Display(Name = "Account type")]
            public UserTypeInput UserType { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new CinemaUser { UserName = Input.Username, 
                                            Email = Input.Email, 
                                            FirstName = Input.FirstName, 
                                            LastName = Input.LastName, 
                                            CreatedOn = DateTime.UtcNow
                                          };
                if (_userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult() == null)
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRoleAsync(user, Input.UserType.ToString());
                    }

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync("cinemashare222@gmail.com", "Admin", Input.Email, "Confirm your email",
                            $"<h3>Please confirm your account by</h3> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'><h3>clicking here</h3></a>." +
                            $"<br/> {HtmlEncoder.Default.Encode(callbackUrl)}");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
