using Nsu.Coliseum.Database;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.ExperimentsCreation;

public class Program
{
    private const int NumberOfCards = 36;

    public static void Main(string[] args)
    {
        int numberOfExperiments = args.Length >= 1 && int.TryParse(args[0], out int result) ? result : 100;

        var deckShuffler = new DeckShuffler();

        using (var appContext = new ApplicationContext())
        {
            for (int i = 0; i < numberOfExperiments; ++i)
            {
                var deck = new Deck.Deck(NumberOfCards);
                deckShuffler.ShuffleDeck(deck);
                var experimentEntity = new ExperimentEntity
                {
                    Deck = deck
                };
                appContext.Experiments.Add(experimentEntity);
            }

            appContext.SaveChanges();
        }
    }
}