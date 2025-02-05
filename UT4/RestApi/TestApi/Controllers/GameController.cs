using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
    public IActionResult AddGame([FromBody] Game game)
    {
        if (game == null)
            return BadRequest("Invalid game data.");

        _gameService.Games.Add(game);
        return Ok(new { message = "Game added successfully!", game });
    }

    // POST: Add multiple games
    [HttpPost("add-multiple")]
    public IActionResult AddGames([FromBody] List<Game> newGames)
    {
        if (newGames == null || newGames.Count == 0)
            return BadRequest("Invalid game list.");

        _gameService.Games.AddRange(newGames);
        return Ok(new { message = "Games added successfully!", games = _gameService.Games });
    }
    [HttpGet("{id}")]
    public IActionResult GetGame(int id)
    {
        var game = _gameService.Games.FirstOrDefault(g => g.Id == id);
        if (game == null)
            return NotFound("Game not found.");

        return Ok(game);
    }

    // GET: Retrieve all games
    [HttpGet]
    public IActionResult GetAllGames()
    {
        return Ok(_gameService.Games);
    }

    // PUT: Update a game by ID
    [HttpPut("{id}")]
    public IActionResult UpdateGame(int id, [FromBody] Game updatedGame)
    {
        if (updatedGame == null)
            return BadRequest("Invalid game data.");

        var game = _gameService.Games.FirstOrDefault(g => g.Id == id);
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
    public IActionResult UpdateMultipleGames([FromBody] List<Game> updatedGames)
    {
        if (updatedGames == null || updatedGames.Count == 0)
            return BadRequest("Invalid games data.");

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

        return Ok(new { message = "Multiple games updated successfully!" });
    }

    // DELETE: Delete a game by ID
    [HttpDelete("{id}")]
    public IActionResult DeleteGame(int id)
    {
        var game = _gameService.Games.FirstOrDefault(g => g.Id == id);
        if (game == null)
            return NotFound("Game not found.");

        _gameService.Games.Remove(game);
        return Ok(new { message = "Game deleted successfully!" });
    }

    // DELETE: Delete multiple games by IDs
    [HttpDelete("delete-multiple")]
    public IActionResult DeleteMultipleGames([FromBody] List<int> gameIds)
    {
        if (gameIds == null || gameIds.Count == 0)
            return BadRequest("Invalid game IDs.");

        var gamesToDelete = _gameService.Games.Where(g => gameIds.Contains(g.Id)).ToList();
        if (gamesToDelete.Count == 0)
            return NotFound("No games found to delete.");

        foreach (var game in gamesToDelete)
        {
            _gameService.Games.Remove(game);
        }

        return Ok(new { message = "Multiple games deleted successfully!" });
    }
}
