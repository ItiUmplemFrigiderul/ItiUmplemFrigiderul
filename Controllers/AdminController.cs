using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        db = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    
    public async Task<IActionResult> UserRoles()
    {
        var users = _userManager.Users.ToList();
        ViewBag.UserManager = _userManager;
        return View(users);
    }

    
    [HttpPost]
    public async Task<IActionResult> UpdateUserRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        
        if (!await _roleManager.RoleExistsAsync(role))
        {
            return BadRequest("Rolul nu există.");
        }

        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction("UserRoles");
    }
}
