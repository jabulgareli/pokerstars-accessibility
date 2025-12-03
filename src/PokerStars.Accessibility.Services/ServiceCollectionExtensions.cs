using Microsoft.Extensions.DependencyInjection;
using PokerStars.Accessibility.Core;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Extension methods for registering PokerStars Accessibility services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds PokerStars Accessibility services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddPokerStarsAccessibilityServices(this IServiceCollection services)
    {
        // Register core services
        services.AddSingleton<PokerTextParser>();

        // Register services
        services.AddSingleton<IScreenCaptureService, WindowsScreenCaptureService>();
        services.AddSingleton<IOcrService, OcrService>();
        services.AddSingleton<IGameExtractionService, GameExtractionService>();
        services.AddSingleton<ITextToSpeechService, MauiTextToSpeechService>();

        return services;
    }
}
