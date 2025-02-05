using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    // POST: Add a single game
    [HttpPost("add")]
    public async Task<IActionResult> AddGame([FromBody] Game game)
    {
        if (game == null)
            return BadRequest("Invalid game data.");

        await Task.Run(() => _gameService.Games.Add(game)); // Simulate async work
        return Ok(new { message = "Game added successfully!", game });
    }

    // POST: Add multiple games
    [HttpPost("add-multiple")]
    public async Task<IActionResult> AddGames([FromBody] List<Game> newGames)
    {
        if (newGames == null || newGames.Count == 0)
            return BadRequest("Invalid game list.");

        await Task.Run(() => _gameService.Games.AddRange(newGames)); // Simulate async work
        return Ok(new { message = "Games added successfully!", games = _gameService.Games });
    }

    // GET: Retrieve a single game by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGame(int id)
    {
        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");

        return Ok(game);
    }

    // GET: Retrieve all games
    [HttpGet]
    public async Task<IActionResult> GetAllGames()
    {
        return Ok(await Task.Run(() => _gameService.Games)); // Simulate async work
    }

    // PUT: Update a game by ID
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game updatedGame)
    {
        if (updatedGame == null)
            return BadRequest("Invalid game data.");

        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");

        game.Name = updatedGame.Name;
        game.Genre = updatedGame.Genre;
        game.Price = updatedGame.Price;
        game.Dlcs = updatedGame.Dlcs;

        return Ok(new { message = "Game updated successfully!", game });
    }

    // PUT: Update multiple games
    [HttpPut("update-multiple")]
    public async Task<IActionResult> UpdateMultipleGames([FromBody] List<Game> updatedGames)
    {
        if (updatedGames == null || updatedGames.Count == 0)
            return BadRequest("Invalid games data.");

        await Task.Run(() =>
        {
            foreach (var updatedGame in updatedGames)
            {
                var game = _gameService.Games.FirstOrDefault(g => g.Id == updatedGame.Id);
                if (game != null)
                {
                    game.Name = updatedGame.Name;
                    game.Genre = updatedGame.Genre;
                    game.Price = updatedGame.Price;
                    game.Dlcs = updatedGame.Dlcs;
                }
            }
        });

        return Ok(new { message = "Multiple games updated successfully!" });
    }

    // DELETE: Delete a game by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await Task.Run(() => _gameService.Games.FirstOrDefault(g => g.Id == id)); // Simulate async work
        if (game == null)
            return NotFound("Game not found.");

        await Task.Run(() => _gameService.Games.Remove(game)); // Simulate async work
        return Ok(new { message = "Game deleted successfully!" });
    }

    // DELETE: Delete multiple games by IDs
    [HttpDelete("delete-multiple")]
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

        return Ok(new { message = "Multiple games deleted successfully!" });
    }
}
