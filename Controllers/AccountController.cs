using Campus360.Data;
using Campus360.Services;
using Campus360.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Campus360.Controllers
{
    public class AccountController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        private readonly IAccountRepository _repo;
        
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IAccountRepository repo){
            _userManager = userManager;
            _signInManager = signInManager;
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login(){
            LoginVM loginVM = new LoginVM();
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM){
            if(!ModelState.IsValid){
                TempData["Error"] = "Wrong Credentials; Please Try Again";
                return View(loginVM);
            }
            var user = await _userManager.FindByNameAsync(loginVM.Username);

            if(user != null){
                var CheckPassword = await _userManager.CheckPasswordAsync(user,loginVM.Password.ToString());
                if(CheckPassword){
                    var login = await _signInManager.PasswordSignInAsync(user,loginVM.Password.ToString(),true,true);
                    if(login.Succeeded){
                        return RedirectToAction("Index","Home");
                    }
                }
            }
            TempData["Error"] = "Wrong Credentials; Please Try Again";
            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if(!ModelState.IsValid)return View(registerVM);
            User user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                TempData["Error"] = "The Email Address is Taken";
                return View(registerVM);
            }

                User newuser = new User()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FullName = registerVM.UserName
            };
            var added = await _userManager.CreateAsync(newuser,registerVM.Password);
            if (added.Succeeded)
            {
                await _userManager.AddToRoleAsync(newuser, UserRoles.User);
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = string.Join("; ", added.Errors.Select(e => e.Description));
            return View(registerVM);
        }
    }
}