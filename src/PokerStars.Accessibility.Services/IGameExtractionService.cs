using PokerStars.Accessibility.Models;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Interface for game extraction service
/// </summary>
public interface IGameExtractionService
{
    /// <summary>
    /// Extracts game state from the current screen
    /// </summary>
    Task<GameState> ExtractGameStateAsync();

    /// <summary>
    /// Starts continuous monitoring of the game
    /// </summary>
    Task StartMonitoringAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops monitoring
    /// </summary>
    void StopMonitoring();

    /// <summary>
    /// Gets the latest game state
    /// </summary>
    GameState? GetLatestGameState();

    /// <summary>
    /// Event fired when game state changes
    /// </summary>
    event EventHandler<GameState>? GameStateChanged;

    /// <summary>
    /// Event fired when a new action is detected
    /// </summary>
    event EventHandler<PlayerAction>? ActionDetected;
}
