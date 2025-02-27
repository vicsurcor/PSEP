using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private static int _nextUserId;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    // POST: Add a single User
    [HttpPost("add")]
    public async Task<IActionResult> Register([FromBody] User user){
        if (user == null)
            return BadRequest("Invalid user data");
        user.Id = _userService.GetNextUserId();
        // TODO: Add Email Encryption and Password Hash 
        await Task.Run(() => _userService.Users.Add(user)); // Simulate async work
        return Ok(new { message = "User added successfully!", user });
    }

    // TODO: VerifyUser AKA Login

    // GET: Retrieve a single user by username
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == username)); // Simulate async work
        if (user == null)
            return NotFound("User not found.");

        return Ok(user);
    }

    // PUT: Updates a single user by username
    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateUser(string username, [FromBody] User updatedUser)
    {
        if (updatedUser == null)
            return BadRequest("Invalid user data.");

        var user = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == username)); // Simulate async work
        if (user == null)
            return NotFound("User not found.");

        user.UserName = updatedUser.UserName;
        user.Password = updatedUser.Password;

        return Ok(new { message = "User updated successfully!", user });
    }

    // PUT: Updates a users role
    [HttpPut("/role/{id}")]
    //[Authorize(Roles = "Admin")]
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
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var user = await Task.Run(() => _userService.Users.FirstOrDefault(u => u.UserName == username)); // Simulate async work
        if (user == null)
            return NotFound("User not found.");

        await Task.Run(() => _userService.Users.Remove(user)); // Simulate async work
        return Ok(new { message = "User deleted successfully!" });
    }
}
