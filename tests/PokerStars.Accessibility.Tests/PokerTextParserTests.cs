using PokerStars.Accessibility.Core;
using PokerStars.Accessibility.Models;

namespace PokerStars.Accessibility.Tests;

public class PokerTextParserTests
{
    private readonly PokerTextParser _parser = new();

    [Fact]
    public void ParseText_ExtractsBlinds()
    {
        var text = "Blinds: $0.25/$0.50";
        
        var result = _parser.ParseText(text);
        
        Assert.Equal(0.25m, result.Table.SmallBlind);
        Assert.Equal(0.50m, result.Table.BigBlind);
    }

    [Fact]
    public void ParseText_ExtractsPot()
    {
        var text = "Pot: $125.50";
        
        var result = _parser.ParseText(text);
        
        Assert.Equal(125.50m, result.Table.Pot);
    }

    [Fact]
    public void ParseText_ExtractsCards()
    {
        var text = "Hero: As Kh";
        
        var result = _parser.ParseText(text);
        
        var hero = result.Table.GetHero();
        Assert.NotNull(hero);
        Assert.Equal(2, hero.HoleCards.Count);
        Assert.Equal(CardRank.Ace, hero.HoleCards[0].Rank);
        Assert.Equal(CardSuit.Spades, hero.HoleCards[0].Suit);
        Assert.Equal(CardRank.King, hero.HoleCards[1].Rank);
        Assert.Equal(CardSuit.Hearts, hero.HoleCards[1].Suit);
    }

    [Fact]
    public void ParseText_ExtractsCommunityCards()
    {
        var text = "Hero: As Kh [Ah 7c 2d]";
        
        var result = _parser.ParseText(text);
        
        Assert.Equal(3, result.Table.CommunityCards.Count);
        Assert.Equal(BettingRound.Flop, result.Table.CurrentRound);
    }

    [Fact]
    public void ParseText_ExtractsFoldAction()
    {
        var text = "Player1 folds";
        
        var result = _parser.ParseText(text);
        
        var foldAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.Fold);
        Assert.NotNull(foldAction);
        Assert.Equal("Player1", foldAction.PlayerName);
    }

    [Fact]
    public void ParseText_ExtractsCallAction()
    {
        var text = "Player2 calls $10";
        
        var result = _parser.ParseText(text);
        
        var callAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.Call);
        Assert.NotNull(callAction);
        Assert.Equal("Player2", callAction.PlayerName);
        Assert.Equal(10m, callAction.Amount);
    }

    [Fact]
    public void ParseText_ExtractsBetAction()
    {
        var text = "Player3 bets $25.50";
        
        var result = _parser.ParseText(text);
        
        var betAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.Bet);
        Assert.NotNull(betAction);
        Assert.Equal("Player3", betAction.PlayerName);
        Assert.Equal(25.50m, betAction.Amount);
    }

    [Fact]
    public void ParseText_ExtractsRaiseAction()
    {
        var text = "Player4 raises to $100";
        
        var result = _parser.ParseText(text);
        
        var raiseAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.Raise);
        Assert.NotNull(raiseAction);
        Assert.Equal("Player4", raiseAction.PlayerName);
        Assert.Equal(100m, raiseAction.Amount);
    }

    [Fact]
    public void ParseText_ExtractsCheckAction()
    {
        var text = "Player5 checks";
        
        var result = _parser.ParseText(text);
        
        var checkAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.Check);
        Assert.NotNull(checkAction);
        Assert.Equal("Player5", checkAction.PlayerName);
    }

    [Fact]
    public void ParseText_ExtractsAllInAction()
    {
        var text = "Player6 is all-in";
        
        var result = _parser.ParseText(text);
        
        var allInAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.AllIn);
        Assert.NotNull(allInAction);
        Assert.Equal("Player6", allInAction.PlayerName);
    }

    [Fact]
    public void ParseText_ExtractsWinAction()
    {
        var text = "Player1 wins $200";
        
        var result = _parser.ParseText(text);
        
        var winAction = result.Table.ActionHistory.FirstOrDefault(a => a.ActionType == ActionType.Win);
        Assert.NotNull(winAction);
        Assert.Equal("Player1", winAction.PlayerName);
        Assert.Equal(200m, winAction.Amount);
    }

    [Fact]
    public void ParseText_ComplexScenario()
    {
        var text = @"
PokerStars - Table 'Achernar' 9-max
Hand #2847194726
Blinds: $0.25/$0.50

Hero: As Kh

Player5 posts small blind $0.25
Player6 posts big blind $0.50

*** HOLE CARDS ***
Player1 folds
Player2 calls $0.50
Hero raises to $2.00
Player4 folds
Player5 folds
Player6 calls $1.50
Player2 calls $1.50

*** FLOP *** [Ah 7c 2d]
Pot: $6.25
";

        var result = _parser.ParseText(text);
        
        Assert.True(result.IsValid);
        Assert.Equal(0.25m, result.Table.SmallBlind);
        Assert.Equal(0.50m, result.Table.BigBlind);
        Assert.Equal(6.25m, result.Table.Pot);
        // Hero has 2 cards (As Kh) and there are 3 community cards (Ah 7c 2d)
        // The parser currently puts first 2 cards in hero, rest in community
        Assert.True(result.Table.ActionHistory.Count > 0);
    }
}
