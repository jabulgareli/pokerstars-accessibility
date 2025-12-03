namespace PokerStars.Accessibility.Models;

/// <summary>
/// Represents an action taken by a player
/// </summary>
public class PlayerAction
{
    /// <summary>
    /// The name of the player who performed the action
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// The type of action
    /// </summary>
    public ActionType ActionType { get; set; }

    /// <summary>
    /// The amount involved (for bets, raises, calls)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// When the action occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// The betting round when the action occurred
    /// </summary>
    public BettingRound Round { get; set; }

    /// <summary>
    /// Creates a new player action
    /// </summary>
    public PlayerAction()
    {
    }

    /// <summary>
    /// Creates a new player action with specified parameters
    /// </summary>
    public PlayerAction(string playerName, ActionType actionType, decimal amount = 0, BettingRound round = BettingRound.Preflop)
    {
        PlayerName = playerName;
        ActionType = actionType;
        Amount = amount;
        Round = round;
    }

    /// <summary>
    /// Returns a readable description of the action
    /// </summary>
    public override string ToString()
    {
        return ActionType switch
        {
            ActionType.Fold => $"{PlayerName} folds",
            ActionType.Check => $"{PlayerName} checks",
            ActionType.Call => $"{PlayerName} calls {Amount:C}",
            ActionType.Bet => $"{PlayerName} bets {Amount:C}",
            ActionType.Raise => $"{PlayerName} raises to {Amount:C}",
            ActionType.AllIn => $"{PlayerName} goes all-in with {Amount:C}",
            ActionType.PostSmallBlind => $"{PlayerName} posts small blind {Amount:C}",
            ActionType.PostBigBlind => $"{PlayerName} posts big blind {Amount:C}",
            ActionType.ShowCards => $"{PlayerName} shows cards",
            ActionType.Win => $"{PlayerName} wins {Amount:C}",
            _ => $"{PlayerName} performs unknown action"
        };
    }
}

/// <summary>
/// Types of actions a player can take
/// </summary>
public enum ActionType
{
    Fold,
    Check,
    Call,
    Bet,
    Raise,
    AllIn,
    PostSmallBlind,
    PostBigBlind,
    ShowCards,
    Win
}

/// <summary>
/// Betting rounds in a poker hand
/// </summary>
public enum BettingRound
{
    Preflop,
    Flop,
    Turn,
    River,
    Showdown
}
