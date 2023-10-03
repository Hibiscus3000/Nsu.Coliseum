using Nsu.Coliseum.Database;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.ExperimentsCreation;

public class Program
{
    private const int NumberOfCards = 36;

    public static void Main(string[] args)
    {
        int numberOfExperiments = args.Length >= 1 && int.TryParse(args[0], out int result) ? result : 100;

        var deckProvider = new RandomDeckProvider(numberOfExperiments, NumberOfCards, new DeckShuffler());

        using var appContext = new ApplicationContext();
        for (int i = 0; i < numberOfExperiments; ++i)
        {
            var experimentEntity = new ExperimentEntity
            {
                Deck = deckProvider.GetDeck()
            };
            appContext.Experiments.Add(experimentEntity);
        }

        appContext.SaveChanges();
    }
}