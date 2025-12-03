using PokerStars.Accessibility.Models;
using System.Text.RegularExpressions;

namespace PokerStars.Accessibility.Core;

/// <summary>
/// Parses OCR text to extract poker game data
/// </summary>
public class PokerTextParser
{
    private static readonly Regex ChipStackPattern = new(@"\$?([\d,]+\.?\d*)", RegexOptions.Compiled);
    private static readonly Regex BlindsPattern = new(@"Blinds?\s*:?\s*\$?([\d,]+\.?\d*)\s*/\s*\$?([\d,]+\.?\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex PotPattern = new(@"Pot\s*:?\s*\$?([\d,]+\.?\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex HandNumberPattern = new(@"#?\s*(\d{10,})", RegexOptions.Compiled);
    private static readonly Regex CardPattern = new(@"([2-9TJQKA])([cdhs])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private static readonly Regex FoldPattern = new(@"(\w+)\s+folds?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex CheckPattern = new(@"(\w+)\s+checks?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex CallPattern = new(@"(\w+)\s+calls?\s+\$?([\d,]+\.?\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex BetPattern = new(@"(\w+)\s+bets?\s+\$?([\d,]+\.?\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex RaisePattern = new(@"(\w+)\s+raises?\s+(?:to\s+)?\$?([\d,]+\.?\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex AllInPattern = new(@"(\w+)\s+(?:is\s+)?all-?in", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex WinPattern = new(@"(\w+)\s+wins?\s+\$?([\d,]+\.?\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Parses raw OCR text and extracts game state information
    /// </summary>
    public GameState ParseText(string ocrText)
    {
        var gameState = new GameState
        {
            RawOcrText = ocrText,
            CapturedAt = DateTime.Now
        };

        try
        {
            ParseBlinds(ocrText, gameState);
            ParsePot(ocrText, gameState);
            ParseHandNumber(ocrText, gameState);
            ParseCards(ocrText, gameState);
            ParseActions(ocrText, gameState);
            
            gameState.IsValid = true;
        }
        catch (Exception ex)
        {
            gameState.IsValid = false;
            gameState.ErrorMessage = $"Error parsing game state: {ex.Message}";
        }

        return gameState;
    }

    /// <summary>
    /// Parses blinds information from text
    /// </summary>
    private void ParseBlinds(string text, GameState state)
    {
        var match = BlindsPattern.Match(text);
        if (match.Success)
        {
            state.Table.SmallBlind = ParseDecimal(match.Groups[1].Value);
            state.Table.BigBlind = ParseDecimal(match.Groups[2].Value);
        }
    }

    /// <summary>
    /// Parses pot amount from text
    /// </summary>
    private void ParsePot(string text, GameState state)
    {
        var match = PotPattern.Match(text);
        if (match.Success)
        {
            state.Table.Pot = ParseDecimal(match.Groups[1].Value);
        }
    }

    /// <summary>
    /// Parses hand number from text
    /// </summary>
    private void ParseHandNumber(string text, GameState state)
    {
        var match = HandNumberPattern.Match(text);
        if (match.Success)
        {
            state.HandNumber = match.Groups[1].Value;
        }
    }

    /// <summary>
    /// Parses card information from text
    /// </summary>
    private void ParseCards(string text, GameState state)
    {
        var cardMatches = CardPattern.Matches(text);
        var cards = cardMatches
            .Select(m => ParseCard(m.Groups[1].Value, m.Groups[2].Value))
            .Where(c => c != null)
            .Cast<Card>()
            .ToList();

        // First two cards are typically hero's cards
        if (cards.Count >= 2)
        {
            var hero = state.Table.GetHero() ?? new Player { IsHero = true, Name = "Hero" };
            hero.HoleCards = cards.Take(2).ToList();
            
            if (!state.Table.Players.Any(p => p.IsHero))
            {
                state.Table.Players.Add(hero);
            }
        }

        // Additional cards are community cards
        if (cards.Count > 2)
        {
            state.Table.CommunityCards = cards.Skip(2).Take(5).ToList();
            
            state.Table.CurrentRound = state.Table.CommunityCards.Count switch
            {
                3 => BettingRound.Flop,
                4 => BettingRound.Turn,
                5 => BettingRound.River,
                _ => BettingRound.Preflop
            };
        }
    }

    /// <summary>
    /// Parses player actions from text
    /// </summary>
    private void ParseActions(string text, GameState state)
    {
        var actions = new List<PlayerAction>();

        // Parse folds
        foreach (Match match in FoldPattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.Fold, 0, state.Table.CurrentRound));
        }

        // Parse checks
        foreach (Match match in CheckPattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.Check, 0, state.Table.CurrentRound));
        }

        // Parse calls
        foreach (Match match in CallPattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.Call, 
                ParseDecimal(match.Groups[2].Value), state.Table.CurrentRound));
        }

        // Parse bets
        foreach (Match match in BetPattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.Bet, 
                ParseDecimal(match.Groups[2].Value), state.Table.CurrentRound));
        }

        // Parse raises
        foreach (Match match in RaisePattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.Raise, 
                ParseDecimal(match.Groups[2].Value), state.Table.CurrentRound));
        }

        // Parse all-ins
        foreach (Match match in AllInPattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.AllIn, 0, state.Table.CurrentRound));
        }

        // Parse wins
        foreach (Match match in WinPattern.Matches(text))
        {
            actions.Add(new PlayerAction(match.Groups[1].Value, ActionType.Win, 
                ParseDecimal(match.Groups[2].Value), state.Table.CurrentRound));
        }

        state.Table.ActionHistory.AddRange(actions);
    }

    /// <summary>
    /// Parses a card from rank and suit characters
    /// </summary>
    private Card? ParseCard(string rank, string suit)
    {
        var cardRank = rank.ToUpper() switch
        {
            "2" => CardRank.Two,
            "3" => CardRank.Three,
            "4" => CardRank.Four,
            "5" => CardRank.Five,
            "6" => CardRank.Six,
            "7" => CardRank.Seven,
            "8" => CardRank.Eight,
            "9" => CardRank.Nine,
            "T" => CardRank.Ten,
            "J" => CardRank.Jack,
            "Q" => CardRank.Queen,
            "K" => CardRank.King,
            "A" => CardRank.Ace,
            _ => (CardRank?)null
        };

        var cardSuit = suit.ToLower() switch
        {
            "c" => CardSuit.Clubs,
            "d" => CardSuit.Diamonds,
            "h" => CardSuit.Hearts,
            "s" => CardSuit.Spades,
            _ => (CardSuit?)null
        };

        if (cardRank.HasValue && cardSuit.HasValue)
        {
            return new Card(cardRank.Value, cardSuit.Value);
        }

        return null;
    }

    /// <summary>
    /// Parses a decimal value from a string
    /// </summary>
    private decimal ParseDecimal(string value)
    {
        var cleaned = value.Replace(",", "").Replace("$", "").Trim();
        return decimal.TryParse(cleaned, out var result) ? result : 0;
    }
}
