using PokerStars.Accessibility.Models;

namespace PokerStars.Accessibility.Tests;

public class CardTests
{
    [Fact]
    public void Card_ToString_ReturnsCorrectFormat()
    {
        var card = new Card(CardRank.Ace, CardSuit.Spades);
        
        Assert.Equal("Ace of Spades", card.ToString());
    }

    [Fact]
    public void Card_ToShortString_ReturnsCorrectFormat()
    {
        var card = new Card(CardRank.Ace, CardSuit.Spades);
        
        Assert.Equal("As", card.ToShortString());
    }

    [Theory]
    [InlineData(CardRank.Two, "2")]
    [InlineData(CardRank.Ten, "T")]
    [InlineData(CardRank.Jack, "J")]
    [InlineData(CardRank.Queen, "Q")]
    [InlineData(CardRank.King, "K")]
    [InlineData(CardRank.Ace, "A")]
    public void Card_ToShortString_ReturnsCorrectRank(CardRank rank, string expectedRank)
    {
        var card = new Card(rank, CardSuit.Hearts);
        
        Assert.StartsWith(expectedRank, card.ToShortString());
    }

    [Theory]
    [InlineData(CardSuit.Clubs, "c")]
    [InlineData(CardSuit.Diamonds, "d")]
    [InlineData(CardSuit.Hearts, "h")]
    [InlineData(CardSuit.Spades, "s")]
    public void Card_ToShortString_ReturnsCorrectSuit(CardSuit suit, string expectedSuit)
    {
        var card = new Card(CardRank.Ace, suit);
        
        Assert.EndsWith(expectedSuit, card.ToShortString());
    }
}
