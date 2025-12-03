namespace PokerStars.Accessibility.Models;

/// <summary>
/// Represents a player at the poker table
/// </summary>
public class Player
{
    /// <summary>
    /// The player's name as shown in PokerStars
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The player's seat position (1-10)
    /// </summary>
    public int SeatNumber { get; set; }

    /// <summary>
    /// The player's current chip stack
    /// </summary>
    public decimal ChipStack { get; set; }

    /// <summary>
    /// The player's bet in the current round
    /// </summary>
    public decimal CurrentBet { get; set; }

    /// <summary>
    /// Indicates if this is the user's player (hero)
    /// </summary>
    public bool IsHero { get; set; }

    /// <summary>
    /// Indicates if the player is currently in the hand
    /// </summary>
    public bool IsInHand { get; set; } = true;

    /// <summary>
    /// Indicates if the player has folded
    /// </summary>
    public bool HasFolded { get; set; }

    /// <summary>
    /// Indicates if the player is the dealer
    /// </summary>
    public bool IsDealer { get; set; }

    /// <summary>
    /// Indicates if the player is the small blind
    /// </summary>
    public bool IsSmallBlind { get; set; }

    /// <summary>
    /// Indicates if the player is the big blind
    /// </summary>
    public bool IsBigBlind { get; set; }

    /// <summary>
    /// Indicates if the player is all-in
    /// </summary>
    public bool IsAllIn { get; set; }

    /// <summary>
    /// The player's hole cards (only visible for hero or at showdown)
    /// </summary>
    public List<Card> HoleCards { get; set; } = new();

    /// <summary>
    /// The last action taken by the player
    /// </summary>
    public PlayerAction? LastAction { get; set; }

    /// <summary>
    /// Returns a formatted string with the player's status
    /// </summary>
    public override string ToString()
    {
        var position = IsDealer ? " (Dealer)" : IsSmallBlind ? " (SB)" : IsBigBlind ? " (BB)" : "";
        var status = HasFolded ? " - Folded" : IsAllIn ? " - All-In" : "";
        return $"{Name}{position}: {ChipStack:C}{status}";
    }

    /// <summary>
    /// Gets a description of the player's hole cards for accessibility
    /// </summary>
    public string GetCardsDescription()
    {
        if (HoleCards.Count == 0)
            return "No cards visible";

        return string.Join(" and ", HoleCards.Select(c => c.ToString()));
    }
}
