using FortniteAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace FortniteAPI.Controllers;
[ApiController]
[Route("[controller]")]


public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly UsersContext _context;
    private readonly TokenService _tokenService;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AuthController(UserManager<IdentityUser> userManager, UsersContext context, TokenService tokenService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _roleManager = roleManager;
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create user using UserManager
            var user = new IdentityUser { UserName = request.Username, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            // Assign role to the user (default to 'regular' if role not specified)
            var role = request.Role ?? "regular"; // default to 'regular' if no role is specified

            // Ensure the user is assigned a valid role
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                // If role doesn't exist, return an error
                return BadRequest($"Role '{role}' does not exist.");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            // Retrieve user from context (similar to the login method)
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (userInDb == null)
            {
                return Unauthorized();
            }

            // Create token for the user
            var accessToken = _tokenService.CreateToken(userInDb);

            // Return the response (similar to login)
            return Ok(new
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Message: {ex.Message}\nStack: {ex.StackTrace}");
        }
    }




    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(request.Email);
            if (managedUser == null)
            {
                return BadRequest("Bad credentials");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (userInDb is null)
                return Unauthorized();
            var accessToken = _tokenService.CreateToken(userInDb);
            await _context.SaveChangesAsync();
            return Ok(new AuthResponse
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Message: {ex.Message}\nStack:{ex.StackTrace}");
           
        }        
    }
}
