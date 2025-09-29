using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NeolantTestTask.Models;
using NeolantTestTask.Repositories;

namespace NeolantTestTask.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserRepository _userRepository;

    public AdminController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    [Route("Admin")]
    public async Task<IActionResult> AdminPanel()
    {
        var users = await _userRepository.GetAllAsync();
        return View(users);
    }


    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(User editedUser)
    {
        var user = await _userRepository.GetByIdAsync(editedUser.Id);

        if (user == null) return NotFound();
        
        if (!string.IsNullOrEmpty(editedUser.PasswordHash))
            editedUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(editedUser.PasswordHash);

        await _userRepository.UpdateAsync(editedUser);

        return RedirectToAction(nameof(AdminPanel));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null){ return RedirectToAction("login", "account"); }
        var userId = int.Parse(userIdClaim.Value);
        if (id == userId)
        {
            return RedirectPermanent("https://www.ya-roditel.ru/parents/helpline/");
        }
        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if  (user == null) return NotFound();

        await _userRepository.DeleteAsync(id);
        return RedirectToAction(nameof(AdminPanel));
    }

    public IActionResult Create()
    {
        return View();
    }

    
    [HttpPost]
    public async Task<IActionResult> Create(string username, string password, string role)
    {
        User user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };
        await _userRepository.AddAsync(user);
        return RedirectToAction(nameof(AdminPanel));
    }
}