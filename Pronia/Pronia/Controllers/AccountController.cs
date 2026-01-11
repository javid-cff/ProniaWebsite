using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pronia.Interfaces;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = new AppUser
            {
                FullName = vm.FullName,
                UserName = vm.UserName,
                Email = vm.Email
            };

            var result = await userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(vm);
            }

            await SendConfirmationEmailAsync(user);

            return RedirectToAction("RegisterConfirmation");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await userManager.FindByNameAsync(vm.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Email və ya şifrə yalnışdır");
                return View(vm);
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Zəhmət olmasa emailinizi təsdiqləyin.");
                return View(vm);
            }

            var result = await signInManager.PasswordSignInAsync(
                user,
                vm.Password,
                vm.RememberMe,
                false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email və ya şifrə yalnışdır");
                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        private async Task SendConfirmationEmailAsync(AppUser user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebUtility.UrlEncode(token);

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, token },
                Request.Scheme
            );

            await emailService.SendEmailAsync(
                user.Email,
                "Email Təsdiqi",
                $"<h3>Hesabınızı təsdiqləyin</h3><a href='{confirmationLink}'>Təsdiqlə</a>"
            );
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Link düzgün deyil.");

            token = WebUtility.UrlDecode(token);

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User tapılmadı.");

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return BadRequest("Email təsdiqlənmədi.");

            return View("ConfirmEmail");
        }


    }
}
