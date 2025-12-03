namespace PokerStars.Accessibility.Models;

/// <summary>
/// Represents the complete state of the poker game at a given moment
/// </summary>
public class GameState
{
    /// <summary>
    /// The current table information
    /// </summary>
    public Table Table { get; set; } = new();

    /// <summary>
    /// When this state was captured
    /// </summary>
    public DateTime CapturedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// The hand number from PokerStars
    /// </summary>
    public string HandNumber { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if it's the hero's turn to act
    /// </summary>
    public bool IsHeroTurn { get; set; }

    /// <summary>
    /// The current player to act (if detectable)
    /// </summary>
    public string? CurrentPlayerToAct { get; set; }

    /// <summary>
    /// The minimum bet/raise amount
    /// </summary>
    public decimal MinBet { get; set; }

    /// <summary>
    /// The maximum bet amount (usually all-in)
    /// </summary>
    public decimal MaxBet { get; set; }

    /// <summary>
    /// Time remaining for hero to act (if visible)
    /// </summary>
    public int? TimeRemaining { get; set; }

    /// <summary>
    /// Raw OCR text from the screen
    /// </summary>
    public string RawOcrText { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the game state is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Error message if state is invalid
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets a complete summary of the game state for accessibility
    /// </summary>
    public string GetAccessibleSummary()
    {
        if (!IsValid)
            return ErrorMessage ?? "Unable to read game state";

        var summary = new List<string>
        {
            $"Table: {Table.Name}",
            Table.GetBlindsDescription(),
            $"Pot: {Table.Pot:C}"
        };

        // Add community cards if any
        if (Table.CommunityCards.Count > 0)
        {
            summary.Add(Table.GetCommunityCardsDescription());
        }

        // Add hero cards
        var hero = Table.GetHero();
        if (hero != null)
        {
            summary.Add($"Your cards: {hero.GetCardsDescription()}");
            summary.Add($"Your stack: {hero.ChipStack:C}");
        }

        // Add turn info
        if (IsHeroTurn)
        {
            summary.Add("It's your turn to act!");
            if (TimeRemaining.HasValue)
            {
                summary.Add($"Time remaining: {TimeRemaining} seconds");
            }
        }
        else if (!string.IsNullOrEmpty(CurrentPlayerToAct))
        {
            summary.Add($"Waiting for {CurrentPlayerToAct} to act");
        }

        return string.Join("\n", summary);
    }

    /// <summary>
    /// Gets a summary of recent actions for accessibility
    /// </summary>
    public string GetRecentActionsDescription(int count = 5)
    {
        var recentActions = Table.ActionHistory
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .Reverse()
            .ToList();

        if (recentActions.Count == 0)
            return "No actions yet";

        return "Recent actions:\n" + string.Join("\n", recentActions.Select(a => $"  - {a}"));
    }
}
