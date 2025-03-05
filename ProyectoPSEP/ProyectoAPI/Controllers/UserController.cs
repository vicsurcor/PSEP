using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;
    private static int _nextUserId;

    public UserController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    // POST: Add a single User
    [HttpPost("add")]
    public async Task<IActionResult> Register([FromBody] User user){
        if (user == null)
            return BadRequest("Invalid user data");
        user.Id = _userService.GetNextUserId();
        user.Password =  _userService.HashPassword(user.Password);
        user.Email = Convert.ToBase64String(_userService.EncryptEmail(user.Email),0,_userService.EncryptEmail(user.Email).Length);
        await Task.Run(() => _userService.Users.Add(user)); // Simulate async work
        return Ok(new { message = "User added successfully!", user });
    }

    // POST: Verifies the user, shows their info and distributes a JWT Token
    [HttpPost]
    public async Task<IActionResult> Login([Bind("UserName", "Password")] User user){
        var userSaved = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == user.UserName)); // Simulate async work
        if (userSaved == null)
            return NotFound("User not found.");
        if (_userService.VerifyPassword(user.Password, userSaved.Password) == false)
            return BadRequest("Passwords don't match");

        if (userSaved.Role == "Admin") {
            var token = _jwtService.GenerateToken(user.UserName, "Admin");
            Console.WriteLine(token);
            return Ok(new { message = "User verified successfully!", user ,token});
        } else if (userSaved.Role == "Client") {
            var token = _jwtService.GenerateToken(user.UserName, "Client");
            return Ok(new { message = "User verified successfully!", user , token});
        } else {
            return BadRequest("Invalid Role");
        }
        
        
    }

    // GET: Retrieve all users
    [HttpGet("get")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await Task.Run(() => _userService.Users)); // Simulate async work
    }

    // PUT: Updates a single user by username
    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateUser(string username, [Bind("UserName", "Password")] User updatedUser)
    {
        if (updatedUser == null)
            return BadRequest("Invalid user data.");

        var user = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == username)); // Simulate async work
        if (user == null)
            return NotFound("User not found.");

        user.UserName = updatedUser.UserName;
        user.Password = _userService.HashPassword(updatedUser.Password);

        return Ok(new { message = "User updated successfully!", user });
    }

    // PUT: Updates a users role
    [HttpPut("role/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id)
    {
        var user = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.Id == id)); // Simulate async work
        if (user == null)
            return NotFound("User not found.");

        if (user.Role == "Admin") {
            user.Role = "Client";
        }
        else {
            user.Role = "Admin";
        }

        return Ok(new { message = "Role updated successfully!", user });
    }

    // DELETE: Deletes a single user
    [HttpDelete("delete/admin/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserAdmin(string username)
    {
        var user = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == username)); // Simulate async work
        Console.WriteLine(user.UserName);
        if (user == null)
            return NotFound("User not found.");

        await Task.Run(() => _userService.Users.Remove(user)); // Simulate async work
        return Ok(new { message = "User deleted successfully!" });
    }

    // DELETE: Deletes a single user
    // POST since it needs a body input 
    [HttpPost("delete/client/{username}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteUserClient(string username, [Bind("UserName", "Password")] User user)
    {
        var userSaved = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == username)); // Simulate async work
        if (userSaved == null)
            return NotFound("User not found.");
        if (_userService.VerifyPassword(user.Password, userSaved.Password) == false)
            return BadRequest("Passwords don't match");
        await Task.Run(() => _userService.Users.Remove(userSaved)); // Simulate async work
        return Ok(new { message = "User deleted successfully!" });
    }
}
