using PokerStars.Accessibility.Core;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Interface for OCR service
/// </summary>
public interface IOcrService
{
    /// <summary>
    /// Performs OCR on an image
    /// </summary>
    Task<OcrResult> RecognizeTextAsync(byte[] imageData);

    /// <summary>
    /// Performs OCR on a screen capture
    /// </summary>
    Task<OcrResult> RecognizeTextAsync(ScreenCapture capture);

    /// <summary>
    /// Indicates if the OCR engine is initialized
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initializes the OCR engine
    /// </summary>
    Task InitializeAsync();
}
