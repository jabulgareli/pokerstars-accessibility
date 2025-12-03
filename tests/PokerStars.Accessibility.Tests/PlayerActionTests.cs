using PokerStars.Accessibility.Models;

namespace PokerStars.Accessibility.Tests;

public class PlayerActionTests
{
    [Fact]
    public void PlayerAction_ToString_Fold()
    {
        var action = new PlayerAction("Player1", ActionType.Fold);
        
        Assert.Equal("Player1 folds", action.ToString());
    }

    [Fact]
    public void PlayerAction_ToString_Check()
    {
        var action = new PlayerAction("Player1", ActionType.Check);
        
        Assert.Equal("Player1 checks", action.ToString());
    }

    [Fact]
    public void PlayerAction_ToString_Call()
    {
        var action = new PlayerAction("Player1", ActionType.Call, 10m);
        
        Assert.Contains("Player1 calls", action.ToString());
        Assert.Contains("10", action.ToString());
    }

    [Fact]
    public void PlayerAction_ToString_Bet()
    {
        var action = new PlayerAction("Player1", ActionType.Bet, 25m);
        
        Assert.Contains("Player1 bets", action.ToString());
        Assert.Contains("25", action.ToString());
    }

    [Fact]
    public void PlayerAction_ToString_Raise()
    {
        var action = new PlayerAction("Player1", ActionType.Raise, 50m);
        
        Assert.Contains("Player1 raises", action.ToString());
        Assert.Contains("50", action.ToString());
    }

    [Fact]
    public void PlayerAction_ToString_AllIn()
    {
        var action = new PlayerAction("Player1", ActionType.AllIn, 100m);
        
        Assert.Contains("Player1 goes all-in", action.ToString());
    }

    [Fact]
    public void PlayerAction_ToString_Win()
    {
        var action = new PlayerAction("Player1", ActionType.Win, 200m);
        
        Assert.Contains("Player1 wins", action.ToString());
        Assert.Contains("200", action.ToString());
    }
}
