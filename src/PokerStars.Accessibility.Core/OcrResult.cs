namespace PokerStars.Accessibility.Core;

/// <summary>
/// Represents the result of an OCR operation
/// </summary>
public class OcrResult
{
    /// <summary>
    /// The extracted text from the image
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the OCR operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Confidence level of the OCR result (0-100)
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// Time taken to perform OCR in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Creates a successful OCR result
    /// </summary>
    public static OcrResult CreateSuccess(string text, float confidence = 100, long processingTimeMs = 0)
    {
        return new OcrResult
        {
            Text = text,
            Success = true,
            Confidence = confidence,
            ProcessingTimeMs = processingTimeMs
        };
    }

    /// <summary>
    /// Creates a failed OCR result
    /// </summary>
    public static OcrResult CreateFailure(string errorMessage)
    {
        return new OcrResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
