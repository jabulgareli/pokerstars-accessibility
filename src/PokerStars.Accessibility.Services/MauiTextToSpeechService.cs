using Microsoft.Extensions.Logging;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Text-to-Speech service using MAUI Essentials
/// </summary>
public class MauiTextToSpeechService : ITextToSpeechService
{
    private readonly ILogger<MauiTextToSpeechService> _logger;
    private CancellationTokenSource? _currentSpeechCts;
    
    public float SpeechRate { get; set; } = 1.0f;
    public float Volume { get; set; } = 1.0f;
    public bool IsSpeaking { get; private set; }

    private string? _selectedVoice;

    public MauiTextToSpeechService(ILogger<MauiTextToSpeechService> logger)
    {
        _logger = logger;
    }

    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        try
        {
            // Cancel any current speech
            Stop();

            _currentSpeechCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            IsSpeaking = true;

            _logger.LogDebug("Speaking: {Text}", text);

            var options = new Microsoft.Maui.Media.SpeechOptions
            {
                Volume = Volume,
                Pitch = 1.0f
            };

            await Microsoft.Maui.Media.TextToSpeech.Default.SpeakAsync(text, options, _currentSpeechCts.Token);

            _logger.LogDebug("Speech completed");
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Speech cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during speech");
        }
        finally
        {
            IsSpeaking = false;
        }
    }

    public void Stop()
    {
        if (_currentSpeechCts != null && !_currentSpeechCts.IsCancellationRequested)
        {
            _logger.LogDebug("Stopping current speech");
            _currentSpeechCts.Cancel();
            _currentSpeechCts.Dispose();
            _currentSpeechCts = null;
        }
        IsSpeaking = false;
    }

    public async Task<IEnumerable<string>> GetAvailableVoicesAsync()
    {
        try
        {
            var locales = await Microsoft.Maui.Media.TextToSpeech.Default.GetLocalesAsync();
            return locales.Select(l => $"{l.Name} ({l.Language})").ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available voices");
            return Enumerable.Empty<string>();
        }
    }

    public Task SetVoiceAsync(string voiceName)
    {
        _selectedVoice = voiceName;
        _logger.LogInformation("Voice set to: {VoiceName}", voiceName);
        return Task.CompletedTask;
    }
}
