namespace PokerStars.Accessibility.Models;

/// <summary>
/// Represents a playing card
/// </summary>
public class Card
{
    /// <summary>
    /// The rank of the card (2-10, J, Q, K, A)
    /// </summary>
    public CardRank Rank { get; set; }

    /// <summary>
    /// The suit of the card
    /// </summary>
    public CardSuit Suit { get; set; }

    /// <summary>
    /// Creates a new card instance
    /// </summary>
    public Card(CardRank rank, CardSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }

    /// <summary>
    /// Creates an empty card instance
    /// </summary>
    public Card()
    {
    }

    /// <summary>
    /// Returns the card as a readable string
    /// </summary>
    public override string ToString()
    {
        return $"{GetRankDisplay()} of {Suit}";
    }

    /// <summary>
    /// Gets the display name of the rank
    /// </summary>
    private string GetRankDisplay()
    {
        return Rank switch
        {
            CardRank.Two => "2",
            CardRank.Three => "3",
            CardRank.Four => "4",
            CardRank.Five => "5",
            CardRank.Six => "6",
            CardRank.Seven => "7",
            CardRank.Eight => "8",
            CardRank.Nine => "9",
            CardRank.Ten => "10",
            CardRank.Jack => "Jack",
            CardRank.Queen => "Queen",
            CardRank.King => "King",
            CardRank.Ace => "Ace",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets a short notation for the card (e.g., "As" for Ace of spades)
    /// </summary>
    public string ToShortString()
    {
        var rankChar = Rank switch
        {
            CardRank.Two => "2",
            CardRank.Three => "3",
            CardRank.Four => "4",
            CardRank.Five => "5",
            CardRank.Six => "6",
            CardRank.Seven => "7",
            CardRank.Eight => "8",
            CardRank.Nine => "9",
            CardRank.Ten => "T",
            CardRank.Jack => "J",
            CardRank.Queen => "Q",
            CardRank.King => "K",
            CardRank.Ace => "A",
            _ => "?"
        };

        var suitChar = Suit switch
        {
            CardSuit.Clubs => "c",
            CardSuit.Diamonds => "d",
            CardSuit.Hearts => "h",
            CardSuit.Spades => "s",
            _ => "?"
        };

        return $"{rankChar}{suitChar}";
    }
}

/// <summary>
/// Card ranks from 2 to Ace
/// </summary>
public enum CardRank
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
}

/// <summary>
/// Card suits
/// </summary>
public enum CardSuit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}
