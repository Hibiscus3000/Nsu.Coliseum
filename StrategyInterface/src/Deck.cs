using System.Runtime.CompilerServices;

namespace StrategyInterface;

public class Deck
{
    private readonly int _numberOfCards;
    public readonly Card[] cards;

    public Deck(int numberOfCards)
    {
        _numberOfCards = numberOfCards;
        int numberOfCardsInSuit = numberOfCards / Enum.GetValues(typeof(CardType)).Length;

        cards = new Card[_numberOfCards];
        foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
        {
            for (int i = 0; i < numberOfCardsInSuit; ++i)
            {
                cards[(int)cardType * numberOfCardsInSuit + i] = new Card(cardType, i);
            }
        }
    }

    public Card[][] Split(int numberOfGroups)
    {
        var splited = new Card[numberOfGroups][];
        var numberOfCardsInGroup = _numberOfCards / numberOfGroups;
        for (int group = 0; group < numberOfGroups; ++group) {
            splited[group] = new Card[numberOfCardsInGroup];
        }
 
        for (int i = 0; i < _numberOfCards; ++i) {
            var groupIndex = i / numberOfCardsInGroup;
            var cardIndex = i % numberOfCardsInGroup;
 
            splited[groupIndex][cardIndex] = cards[i];
        }

        return splited;
    }

    public void PrintDeck()
    {
        foreach (Card card in cards)
        {
            Console.Write(card.ToString() + " ");
        }
        Console.WriteLine();
    }
}