using PokerStars.Accessibility.Core;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Interface for screen capture service
/// </summary>
public interface IScreenCaptureService
{
    /// <summary>
    /// Captures the entire screen
    /// </summary>
    Task<ScreenCapture> CaptureScreenAsync();

    /// <summary>
    /// Captures a specific window by title
    /// </summary>
    Task<ScreenCapture> CaptureWindowAsync(string windowTitle);

    /// <summary>
    /// Finds PokerStars windows
    /// </summary>
    Task<IEnumerable<string>> FindPokerStarsWindowsAsync();

    /// <summary>
    /// Gets the primary PokerStars table window
    /// </summary>
    Task<ScreenCapture> CapturePokerStarsTableAsync();
}
