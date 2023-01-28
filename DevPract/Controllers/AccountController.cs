using DevPract.AllData.ViewModels;
using DevPract.Models;
using DevPract.Models.Domain;
using DevPract.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace DevPract.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly MainContext _context;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,ILogger<AccountController> loger,MainContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loger;
            _context = context;
        }
       
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            var user =  _userManager.Users;
            
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Year = user.Year,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Year= model.Year;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var res = await _userManager.DeleteAsync(user);
            return View("Users");
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model,string returnUrl = null)
        {

             if (ModelState.IsValid)
             {
                 var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                 if (result.Succeeded)
                 {   if(!string.IsNullOrEmpty(returnUrl))
                     {
                         return LocalRedirect(returnUrl);
                     }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                        
                    }
                    
                 }
                 else
                 {
                     ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                 }
             }
             return View(model);
          /*  if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null &&
                await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(identity));
                return RedirectToLocal(returnUrl);
                //return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }*/
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        /*  public async Task<IActionResult> Login(LoginModel loginVM) /* --- NEW --- 
          {
              if (!ModelState.IsValid) return View(loginVM);

              var user = await _userManager.FindByEmailAsync(loginVM.Email);
              if (user != null)
              {
                  var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                  if (passwordCheck)
                  {
                      var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                      if (result.Succeeded)
                      {
                          return RedirectToAction("Index", "Home");
                      }
                  }
                  TempData["Error"] = "Wrong credentials. Please, try again!";
                  return View(loginVM);
              }

              TempData["Error"] = "Wrong credentials. Please, try again!";
              return View(loginVM);
          }*/

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (ModelState.IsValid) 
            {
                
                User user = new User { Email = model.Email, UserName = model.Email,FullName = model.FullName, Year = model.Year };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                // If the user is found AND Email is confirmed  && await _userManager.IsEmailConfirmedAsync(user)
                if (user != null)
                {
                    // Generate the reset password token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                  
                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    // Log the password reset link
                    _logger.Log(LogLevel.Warning, passwordResetLink);
                   
                    // Send the user to Forgot Password Confirmation view
                    return View("ResetPassword");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet()]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token,string email) 
        { 
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            ResetPasswordViewModel reset = new ResetPasswordViewModel { Email = email , Token = token};
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View(reset);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }


        // ----------------------------------------- Employee

        

    }
}
