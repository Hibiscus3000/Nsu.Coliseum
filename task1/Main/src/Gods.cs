using StrategyInterface;

public class Gods
{
    private static int _numberOfCards = 36;

    public static void Play(IStrategy elonStrategy, IStrategy markStrategy,
        int numberOfExperiments)
    {
        int numberOfSuccesses = 0;

        for (int i = 0; i < numberOfExperiments; ++i)
        {
            var deck = new Deck(_numberOfCards);

            Card[][] splitedDeck = deck.Split(2);

            Card elonCard = elonStrategy.PickCard(splitedDeck[0]);
            Card markCard = markStrategy.PickCard(splitedDeck[1]);

            if (elonCard.CardColor == markCard.CardColor)
            {
                ++numberOfSuccesses;
            }
        }

        PrintResults(numberOfExperiments, numberOfSuccesses);
    }

    private static void PrintResults(int numberOfExperiments, int numberOfSuccesses)
    {
        Console.WriteLine("Number of experiments: " + numberOfExperiments);
        Console.WriteLine("Number of successes: " + numberOfSuccesses);
        Console.WriteLine("Statistics: "
            + ((double)numberOfSuccesses * 100 / numberOfExperiments).ToString("N2") + "%");
    }
}