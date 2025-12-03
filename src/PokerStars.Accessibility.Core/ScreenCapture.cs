namespace PokerStars.Accessibility.Core;

/// <summary>
/// Represents a captured screenshot
/// </summary>
public class ScreenCapture
{
    /// <summary>
    /// The image data as a byte array
    /// </summary>
    public byte[] ImageData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Width of the captured image
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Height of the captured image
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// When the screenshot was captured
    /// </summary>
    public DateTime CapturedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// The title of the window that was captured
    /// </summary>
    public string WindowTitle { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the capture was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the capture failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful screen capture
    /// </summary>
    public static ScreenCapture CreateSuccess(byte[] imageData, int width, int height, string windowTitle = "")
    {
        return new ScreenCapture
        {
            ImageData = imageData,
            Width = width,
            Height = height,
            WindowTitle = windowTitle,
            Success = true
        };
    }

    /// <summary>
    /// Creates a failed screen capture
    /// </summary>
    public static ScreenCapture CreateFailure(string errorMessage)
    {
        return new ScreenCapture
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
