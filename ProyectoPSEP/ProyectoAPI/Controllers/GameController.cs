using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Partial Updates to an Object in a request should be handled with Patch instead#.
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly FireBaseService _firebaseService;
    private static int _nextGameId;

    public GameController(GameService gameService, FireBaseService firebaseService)
    {
        _gameService = gameService;
        _firebaseService = firebaseService;
    }

    // POST: Add a single game
    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddGame([FromBody] Game game)
    {
        if (game == null)
            return BadRequest("Invalid game data.");
        game.Id = _gameService.GetNextGameId();
        await _firebaseService.AddGame(game);
        await Task.Run(() => _gameService.Games.Add(game)); // Simulate async work
        return Ok(new { message = "Game added successfully!", game });
    }

    // POST: Add multiple games
    [HttpPost("add-multiple")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddGames([FromBody] List<Game> newGames)
    {
        if (newGames == null || newGames.Count == 0)
            return BadRequest("Invalid game list.");
        foreach (var game in newGames)
        {
            // Assign a unique Game ID
            game.Id = _gameService.GetNextGameId();

        }
        await _firebaseService.AddMultipleGames(newGames);
        await Task.Run(() => _gameService.Games.AddRange(newGames)); // Simulate async work
        return Ok(new { message = "Games added successfully!", games = _gameService.Games });
    }

    // GET: Retrieve a single game by ID
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetGame(int id)
    {
        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");
        await _firebaseService.GetGame(game.Id.ToString());
        return Ok(game);
    }

    // GET: Retrieve all games
    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        await _firebaseService.GetGames();
        return Ok(await Task.Run(() => _gameService.Games)); // Simulate async work
    }

    // PUT: Update a game by ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game updatedGame)
    {
        if (updatedGame == null)
            return BadRequest("Invalid game data.");

        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");

        var updates = new Dictionary<string, object>();

        updates["Name"] = updatedGame.Name;
        updates["Genre"] = updatedGame.Genre;
        updates["Price"] = updatedGame.Price;
        updates["Dlcs"] = updatedGame.Dlcs;

        await _firebaseService.UpdateGame(game.Id.ToString(), updates);

        game.Name = updatedGame.Name;
        game.Genre = updatedGame.Genre;
        game.Price = updatedGame.Price;
        game.Dlcs = updatedGame.Dlcs;
        
        return Ok(new { message = "Game updated successfully!", game });
    }

    // PUT: Update multiple games
    [HttpPut("update-multiple")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMultipleGames([FromBody] List<Game> updatedGames)
    {
        if (updatedGames == null || updatedGames.Count == 0)
            return BadRequest("Invalid games data.");

        var tasks = new List<Task>();

        foreach (var updatedGame in updatedGames)
        {
            var game = _gameService.Games.FirstOrDefault(g => g.Id == updatedGame.Id);
            if (game != null)
            {
                var updates = new Dictionary<string, object>();

                updates["Name"] = updatedGame.Name;
                updates["Genre"] = updatedGame.Genre;
                updates["Price"] = updatedGame.Price;
                updates["Dlcs"] = updatedGame.Dlcs;

                // Add the asynchronous task for Firebase update to the tasks list
                tasks.Add(_firebaseService.UpdateGame(game.Id.ToString(), updates));

                // Update the local game object
                game.Name = updatedGame.Name;
                game.Genre = updatedGame.Genre;
                game.Price = updatedGame.Price;
                game.Dlcs = updatedGame.Dlcs;
            }
        }

        // Wait for all Firebase update tasks to complete
        await Task.WhenAll(tasks);

        return Ok(new { message = "Multiple games updated successfully!" });
    }


    // DELETE: Delete a game by ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");

        await _firebaseService.DeleteGame(game.Id.ToString());
        await Task.Run(() => _gameService.Games.Remove(game)); // Simulate async work
        return Ok(new { message = "Game deleted successfully!" });
    }

    // DELETE: Delete multiple games by IDs
    [HttpDelete("delete-multiple")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMultipleGames([FromBody] List<int> gameIds)
    {
        if (gameIds == null || gameIds.Count == 0)
            return BadRequest("Invalid game IDs.");

        var gamesToDelete = await Task.Run(() => _gameService.Games.Where(g => gameIds.Contains(g.Id)).ToList()); // Simulate async work
        if (gamesToDelete.Count == 0)
            return NotFound("No games found to delete.");

        await Task.Run(() =>
        {
            foreach (var game in gamesToDelete)
            {
                _gameService.Games.Remove(game);
            }
        });
        await _firebaseService.DeleteMultipleGames(gameIds);
        return Ok(new { message = "Multiple games deleted successfully!" });
    }
    
    // PUT: Buy a game by ID
    [HttpPut("/buy/{id}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> BuyGame(int id) {
        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");
        if (game.Stock <= 0)
            return BadRequest("Not enough stock\n Operation Cancelled");

        var updates = new Dictionary<string, object>();

        game.Stock--;
        updates["Stock"] = game.Stock;
        await _firebaseService.UpdateGame(id.ToString(), updates);

        return Ok(new { message = "Purchase completed successfully!", game });
    }
}
