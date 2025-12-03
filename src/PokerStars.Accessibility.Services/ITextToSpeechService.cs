namespace PokerStars.Accessibility.Services;

/// <summary>
/// Interface for text-to-speech service
/// </summary>
public interface ITextToSpeechService
{
    /// <summary>
    /// Speaks the given text
    /// </summary>
    Task SpeakAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops any current speech
    /// </summary>
    void Stop();

    /// <summary>
    /// Gets or sets the speech rate (0.0 to 2.0, default 1.0)
    /// </summary>
    float SpeechRate { get; set; }

    /// <summary>
    /// Gets or sets the speech volume (0.0 to 1.0, default 1.0)
    /// </summary>
    float Volume { get; set; }

    /// <summary>
    /// Gets available voices
    /// </summary>
    Task<IEnumerable<string>> GetAvailableVoicesAsync();

    /// <summary>
    /// Sets the voice to use
    /// </summary>
    Task SetVoiceAsync(string voiceName);

    /// <summary>
    /// Indicates if TTS is currently speaking
    /// </summary>
    bool IsSpeaking { get; }
}
