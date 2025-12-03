using Microsoft.Extensions.Logging;
using PokerStars.Accessibility.Core;
using PokerStars.Accessibility.Models;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Service for extracting game state from screen captures
/// </summary>
public class GameExtractionService : IGameExtractionService
{
    private readonly IScreenCaptureService _screenCaptureService;
    private readonly IOcrService _ocrService;
    private readonly PokerTextParser _textParser;
    private readonly ILogger<GameExtractionService> _logger;
    
    private GameState? _latestGameState;
    private CancellationTokenSource? _monitoringCts;
    private bool _isMonitoring;

    public event EventHandler<GameState>? GameStateChanged;
    public event EventHandler<PlayerAction>? ActionDetected;

    public GameExtractionService(
        IScreenCaptureService screenCaptureService,
        IOcrService ocrService,
        PokerTextParser textParser,
        ILogger<GameExtractionService> logger)
    {
        _screenCaptureService = screenCaptureService;
        _ocrService = ocrService;
        _textParser = textParser;
        _logger = logger;
    }

    public async Task<GameState> ExtractGameStateAsync()
    {
        try
        {
            _logger.LogInformation("Extracting game state...");

            // Capture the PokerStars table
            var capture = await _screenCaptureService.CapturePokerStarsTableAsync();
            if (!capture.Success)
            {
                return new GameState
                {
                    IsValid = false,
                    ErrorMessage = capture.ErrorMessage
                };
            }

            // Perform OCR
            var ocrResult = await _ocrService.RecognizeTextAsync(capture);
            if (!ocrResult.Success)
            {
                return new GameState
                {
                    IsValid = false,
                    ErrorMessage = ocrResult.ErrorMessage
                };
            }

            // Parse the text
            var gameState = _textParser.ParseText(ocrResult.Text);
            gameState.Table.Name = capture.WindowTitle;

            _logger.LogInformation("Game state extracted: {HandNumber}, Pot: {Pot}", 
                gameState.HandNumber, gameState.Table.Pot);

            return gameState;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting game state");
            return new GameState
            {
                IsValid = false,
                ErrorMessage = $"Error extracting game state: {ex.Message}"
            };
        }
    }

    public async Task StartMonitoringAsync(CancellationToken cancellationToken = default)
    {
        if (_isMonitoring)
        {
            _logger.LogWarning("Monitoring is already running");
            return;
        }

        _monitoringCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _isMonitoring = true;

        _logger.LogInformation("Starting game monitoring...");

        try
        {
            while (!_monitoringCts.Token.IsCancellationRequested)
            {
                var newState = await ExtractGameStateAsync();

                if (newState.IsValid)
                {
                    // Check for changes
                    if (HasStateChanged(_latestGameState, newState))
                    {
                        // Detect new actions
                        DetectNewActions(_latestGameState, newState);

                        _latestGameState = newState;
                        GameStateChanged?.Invoke(this, newState);
                    }
                }

                // Wait before next capture (adjust based on performance needs)
                await Task.Delay(500, _monitoringCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Monitoring cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during monitoring");
        }
        finally
        {
            _isMonitoring = false;
        }
    }

    public void StopMonitoring()
    {
        _logger.LogInformation("Stopping game monitoring...");
        _monitoringCts?.Cancel();
    }

    public GameState? GetLatestGameState()
    {
        return _latestGameState;
    }

    private bool HasStateChanged(GameState? oldState, GameState newState)
    {
        if (oldState == null) return true;

        // Check for significant changes
        if (oldState.HandNumber != newState.HandNumber) return true;
        if (oldState.Table.Pot != newState.Table.Pot) return true;
        if (oldState.Table.CurrentRound != newState.Table.CurrentRound) return true;
        if (oldState.Table.CommunityCards.Count != newState.Table.CommunityCards.Count) return true;
        if (oldState.Table.ActionHistory.Count != newState.Table.ActionHistory.Count) return true;

        return false;
    }

    private void DetectNewActions(GameState? oldState, GameState newState)
    {
        if (oldState == null) return;

        var oldActionCount = oldState.Table.ActionHistory.Count;
        var newActions = newState.Table.ActionHistory.Skip(oldActionCount);

        foreach (var action in newActions)
        {
            _logger.LogInformation("New action detected: {Action}", action);
            ActionDetected?.Invoke(this, action);
        }
    }
}
