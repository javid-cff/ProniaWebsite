using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : Controller
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
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if(!ModelState.IsValid)
                return View(vm);

            var user = new AppUser
            {
                FullName = vm.FullName,
                UserName = vm.UserName,
                Email = vm.Email
            };

            var result = await userManager.CreateAsync(user, vm.Password);

            if(!result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if(!ModelState.IsValid)
                return View(vm);

            var user = await userManager.FindByNameAsync(vm.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "UserName ve ya Sifre Yanlisdir!");
                return View(vm);
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, vm.Password, lockoutOnFailure: false);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName ve ya Sifre Yanlisdir!");
                return View(vm);
            }

            await signInManager.SignInAsync(user, vm.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
