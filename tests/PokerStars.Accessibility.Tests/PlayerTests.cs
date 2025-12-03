using PokerStars.Accessibility.Models;

namespace PokerStars.Accessibility.Tests;

public class PlayerTests
{
    [Fact]
    public void Player_ToString_ReturnsCorrectFormat()
    {
        var player = new Player
        {
            Name = "TestPlayer",
            ChipStack = 100.50m
        };
        
        Assert.Contains("TestPlayer", player.ToString());
        Assert.Contains("100.50", player.ToString());
    }

    [Fact]
    public void Player_ToString_IncludesDealer()
    {
        var player = new Player
        {
            Name = "TestPlayer",
            ChipStack = 100m,
            IsDealer = true
        };
        
        Assert.Contains("(Dealer)", player.ToString());
    }

    [Fact]
    public void Player_ToString_IncludesSmallBlind()
    {
        var player = new Player
        {
            Name = "TestPlayer",
            ChipStack = 100m,
            IsSmallBlind = true
        };
        
        Assert.Contains("(SB)", player.ToString());
    }

    [Fact]
    public void Player_ToString_IncludesBigBlind()
    {
        var player = new Player
        {
            Name = "TestPlayer",
            ChipStack = 100m,
            IsBigBlind = true
        };
        
        Assert.Contains("(BB)", player.ToString());
    }

    [Fact]
    public void Player_ToString_IncludesFolded()
    {
        var player = new Player
        {
            Name = "TestPlayer",
            ChipStack = 100m,
            HasFolded = true
        };
        
        Assert.Contains("Folded", player.ToString());
    }

    [Fact]
    public void Player_GetCardsDescription_ReturnsNoCards_WhenEmpty()
    {
        var player = new Player();
        
        Assert.Equal("No cards visible", player.GetCardsDescription());
    }

    [Fact]
    public void Player_GetCardsDescription_ReturnsCards()
    {
        var player = new Player
        {
            HoleCards = new List<Card>
            {
                new Card(CardRank.Ace, CardSuit.Spades),
                new Card(CardRank.King, CardSuit.Hearts)
            }
        };
        
        var description = player.GetCardsDescription();
        
        Assert.Contains("Ace", description);
        Assert.Contains("King", description);
    }
}
