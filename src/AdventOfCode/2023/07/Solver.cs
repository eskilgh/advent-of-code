namespace AdventOfCode.Y2023.D07;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var lines = input.Split('\n');
        var hands = lines
            .Select((line, i) => {
                var ss = line.Split(' ');
                return new HandPartOne(ss[0].Select(c => new CardPartOne(c)).ToArray(), int.Parse(ss[1]));
            })
            .ToList();

        hands.Sort();

        var winnings = hands.Select((hand, i) => hand.Bid * (i + 1));
        return winnings.Sum().ToString();
    }
    
    public string PartTwo(string input)
    {
        var lines = input.Split('\n');
        var hands = lines
            .Select((line, i) => {
                var ss = line.Split(' ');
                return new Hand(ss[0].Select(c => new Card(c)).ToArray(), int.Parse(ss[1]));
            })
            .ToList();

        hands.Sort();

        var winnings = hands.Select((hand, i) => hand.Bid * (i + 1));
        return winnings.Sum().ToString();
    }
}

public class Hand : IComparable
{
    public Card[] Cards { get; init; }
    public HandType HandType { get; init; }
    public int Bid { get; init; }

    public Hand(Card[] cards, int bid) : this(cards, bid, GetHandType(cards)) {}

    protected Hand(Card[] cards, int bid, HandType handType)
    {
        if (cards.Length != 5) throw new ArgumentOutOfRangeException(nameof(cards), "Must be length 5");
        Cards = cards;
        Bid = bid;
        HandType = handType;
    }

    public Card this[int index]
    {
        get { return Cards[index]; }
    }

    private static HandType GetHandType(Card[] cards)
    {
        var cardCount = new Dictionary<char, int>();
        var jokersCount = 0;
        foreach (var card in cards)
        {
            if (card.Value == 'J')
                jokersCount++;
            else if (cardCount.ContainsKey(card.Value))
                cardCount[card.Value]++;
            else
            {
                cardCount[card.Value] = 1;
            }
        }
        
        if (jokersCount == 0)
        {
            return cardCount.Keys.Count switch
            {
                1 => HandType.FiveOfAKind,
                2 when cardCount.Values.Any(count => count == 4) => HandType.FourOfAKind,
                2 when cardCount.Values.Any(count => count == 3) => HandType.FullHouse,
                3 when cardCount.Values.Any(count => count == 3) => HandType.ThreeOfAKind,
                3 when cardCount.Values.Any(count => count == 2) => HandType.TwoPair,
                4 => HandType.OnePair,
                5 => HandType.HighCard,
                _ => throw new ArgumentOutOfRangeException(nameof(cards)),
            };
        }

        var maxMatchingNonJokers = cardCount.Count > 0 ?  cardCount.Values.Max() : 0;
        return (maxMatchingNonJokers + jokersCount) switch
        {
            5 => HandType.FiveOfAKind,
            4 => HandType.FourOfAKind,
            3 when cardCount.Values.Count(x => x == 2) == 2 => HandType.FullHouse,
            3 => HandType.ThreeOfAKind,
            2 => HandType.OnePair,
            _ => throw new InvalidOperationException("Invalid state")
        };
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (obj is not Hand other)
            throw new ArgumentException($"Is not a {nameof(Hand)}", nameof(obj));

        if (HandType > other.HandType)
            return 1;
        if (HandType < other.HandType)
            return -1;

        for (var i = 0; i < Cards.Length; i++)
        {
            var comparison = Cards[i].CompareTo(other[i]);
            if (comparison != 0)
                return comparison;
        }
        return 0;
    }
}

public record Card(char Value) : IComparable
{
    private static readonly char[] CardOrder = { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };
    private int? _index;
    public virtual int Index()
    {
        if (_index.HasValue)
            return _index.Value;

        _index = Array.IndexOf(CardOrder, Value);
        return _index.Value;
    }
    
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (obj is not Card other)
            throw new ArgumentException($"Is not a {nameof(Card)}", nameof(obj));

        if (Index()> other.Index())
            return 1;
        if (Index() < other.Index())
            return -1;

        return 0;
    }
}

public record CardPartOne(char Value) : Card(Value)
{
    private static readonly char[] CardOrder = { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
    private int? _index;
    public override int Index()
    {
        if (_index.HasValue)
            return _index.Value;

        _index = Array.IndexOf(CardOrder, Value);
        return _index.Value;
    }
}

public class HandPartOne : Hand
{
    public HandPartOne(CardPartOne[] cards, int bid) : base(cards, bid, GetHandType(cards)) { }

    private static HandType GetHandType(CardPartOne[] cards)
    {
        var cardCount = new Dictionary<char, int>();
        foreach(var card in cards)
        {
            if (cardCount.ContainsKey(card.Value))
                cardCount[card.Value]++;
            else
            {
                cardCount[card.Value] = 1;
            }
        }
        return cardCount.Keys.Count switch
        {
            1 => HandType.FiveOfAKind,
            2 when cardCount.Values.Any(count => count == 4) => HandType.FourOfAKind,
            2 when cardCount.Values.Any(count => count == 3) => HandType.FullHouse,
            3 when cardCount.Values.Any(count => count == 3) => HandType.ThreeOfAKind,
            3 when cardCount.Values.Any(count => count == 2) => HandType.TwoPair,
            4 => HandType.OnePair,
            5 => HandType.HighCard,
            _ => throw new ArgumentOutOfRangeException(nameof(cards)),
        };
    }
}

public enum HandType
{
    HighCard,
    OnePair, 
    TwoPair, 
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
}

