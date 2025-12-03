namespace PokerStars.Accessibility.Models;

/// <summary>
/// Represents a poker table
/// </summary>
public class Table
{
    /// <summary>
    /// The table name/ID from PokerStars
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of game (Hold'em, Omaha, etc.)
    /// </summary>
    public GameType GameType { get; set; } = GameType.TexasHoldem;

    /// <summary>
    /// The small blind amount
    /// </summary>
    public decimal SmallBlind { get; set; }

    /// <summary>
    /// The big blind amount
    /// </summary>
    public decimal BigBlind { get; set; }

    /// <summary>
    /// The ante amount (if any)
    /// </summary>
    public decimal Ante { get; set; }

    /// <summary>
    /// Maximum number of players at the table
    /// </summary>
    public int MaxPlayers { get; set; } = 9;

    /// <summary>
    /// Players currently at the table
    /// </summary>
    public List<Player> Players { get; set; } = new();

    /// <summary>
    /// The community cards on the board
    /// </summary>
    public List<Card> CommunityCards { get; set; } = new();

    /// <summary>
    /// The current pot amount
    /// </summary>
    public decimal Pot { get; set; }

    /// <summary>
    /// Side pots if any
    /// </summary>
    public List<decimal> SidePots { get; set; } = new();

    /// <summary>
    /// The current betting round
    /// </summary>
    public BettingRound CurrentRound { get; set; } = BettingRound.Preflop;

    /// <summary>
    /// History of actions in the current hand
    /// </summary>
    public List<PlayerAction> ActionHistory { get; set; } = new();

    /// <summary>
    /// Gets the blinds as a formatted string
    /// </summary>
    public string GetBlindsDescription()
    {
        var blinds = $"Blinds: {SmallBlind:C}/{BigBlind:C}";
        if (Ante > 0)
            blinds += $" with {Ante:C} ante";
        return blinds;
    }

    /// <summary>
    /// Gets a description of the community cards
    /// </summary>
    public string GetCommunityCardsDescription()
    {
        if (CommunityCards.Count == 0)
            return "No community cards dealt yet";

        var roundName = CurrentRound switch
        {
            BettingRound.Flop => "Flop",
            BettingRound.Turn => "Turn",
            BettingRound.River => "River",
            BettingRound.Showdown => "Showdown",
            _ => "Board"
        };

        return $"{roundName}: {string.Join(", ", CommunityCards.Select(c => c.ToString()))}";
    }

    /// <summary>
    /// Gets the hero player if present
    /// </summary>
    public Player? GetHero()
    {
        return Players.FirstOrDefault(p => p.IsHero);
    }

    /// <summary>
    /// Gets a description of all active players
    /// </summary>
    public string GetPlayersDescription()
    {
        var activePlayers = Players.Where(p => !p.HasFolded).ToList();
        return $"{activePlayers.Count} players in hand:\n" + 
               string.Join("\n", activePlayers.Select(p => $"  - {p}"));
    }
}

/// <summary>
/// Types of poker games
/// </summary>
public enum GameType
{
    TexasHoldem,
    Omaha,
    OmahaHiLo,
    SevenCardStud,
    Razz,
    Other
}
