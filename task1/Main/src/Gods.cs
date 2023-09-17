using StrategyInterface;

public static class Gods
{
    private static readonly int _numberOfCards = 36;

    public static void Play(IStrategy elonStrategy, IStrategy markStrategy,
        int numberOfExperiments)
    {
        int numberOfSuccesses = 0;

        for (int i = 0; i < numberOfExperiments; ++i)
        {
            var deck = new Deck(_numberOfCards);

            Card[][] splitedDeck = deck.Split(2);

            Card[] elonDeck = splitedDeck[0];
            Card[] markDeck = splitedDeck[1];

            int elonCardNum = elonStrategy.PickCard(elonDeck);
            int markCardNum = markStrategy.PickCard(markDeck);

            if (elonDeck[markCardNum].CardColor == markDeck[elonCardNum].CardColor)
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