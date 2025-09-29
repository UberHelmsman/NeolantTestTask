using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

using NeolantTestTask.Models;
using NeolantTestTask.Repositories;

namespace NeolantTestTask.Controllers;

public class AccountController : Controller
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IUserRepository _userRepository;
    private readonly IPetsRepository _petsRepository;


    public AccountController(IUserRepository userRepository, IWebHostEnvironment env, IPetsRepository petsRepository, IStringLocalizer<SharedResource> localizer)
    {
        _userRepository = userRepository;
        _petsRepository = petsRepository;
        _localizer = localizer;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = _localizer["InvalidLoginOrPassword"];
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password, string role = "User")
    {
        if (await _userRepository.GetByUsernameAsync(username) != null)
        {
            ViewBag.Error = _localizer["UserAlreadyExists"];
            return View();
        }

        var user = new User
        {
            Username = username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };

        await _userRepository.AddAsync(user);

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> UserPanel()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return RedirectToAction("login", "account");
        }
        var userId = int.Parse(userIdClaim.Value);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null) return NotFound();

        return View(user);
    }
    
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddPet(string PetType, string PetName)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return RedirectToAction("login", "account");
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return RedirectToAction("login", "account");

        Animal newPet;

        if (PetType == "Cat")
            newPet = new Cat { Name = PetName, OwnerId = user.Id };
        else
            newPet = new Dog { Name = PetName, OwnerId = user.Id };

        user.Pets.Add(newPet);

        await _userRepository.UpdateAsync(user);

        return RedirectToAction("UserPanel");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeletePet(int petId)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return RedirectToAction("login", "account");
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return RedirectToAction("login", "account");

        
        var pet = user.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet != null)
        {
            user.Pets.Remove(pet);
            await _userRepository.UpdateAsync(user);
            await _petsRepository.DeleteAsync(pet.Id);
        }

        return RedirectToAction("UserPanel");
    }


    
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(User updatedUser, IFormFile? AvatarFile, string? deleteAvatar)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return NotFound();
        
        user.Username = updatedUser.Username;
        user.Name = updatedUser.Name;
        user.Surname = updatedUser.Surname;
        user.Email = updatedUser.Email;

        string uploadsFolder = Path.Combine("wwwroot", "images", "users");
        
        if (!string.IsNullOrEmpty(deleteAvatar) && deleteAvatar == "true")
        {
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var filePath = Path.Combine("wwwroot", user.AvatarUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            user.AvatarUrl = null;
        }
        
        if (AvatarFile != null && AvatarFile.Length > 0)
        {
            Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(AvatarFile.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await AvatarFile.CopyToAsync(fileStream);

            user.AvatarUrl = $"images/users/{fileName}";
        }

        await _userRepository.UpdateAsync(user);

        return RedirectToAction("UserPanel");
    }

    

}