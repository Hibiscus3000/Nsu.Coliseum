using StrategyInterface;

public static class Gods
{
    private static readonly int _numberOfCards = 36;

    public static void Play(Opponent elon, Opponent mark, int numberOfExperiments)
    {
        int numberOfSuccesses = 0;

        for (int i = 0; i < numberOfExperiments; ++i)
        {
            if (OneExperiment(elon, mark))
            {
                ++numberOfSuccesses;
            }
        }

        PrintResults(numberOfExperiments, numberOfSuccesses);
    }

    private static bool OneExperiment(Opponent elon, Opponent mark)
    {
        var deck = new Deck(_numberOfCards);

        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        int elonCardNum = elon.UseStrategy(elonDeck);
        int markCardNum = mark.UseStrategy(markDeck);

        return elonDeck[markCardNum].CardColor == markDeck[elonCardNum].CardColor;
    }

    private static void PrintResults(int numberOfExperiments, int numberOfSuccesses)
    {
        Console.WriteLine("Number of experiments: " + numberOfExperiments);
        Console.WriteLine("Number of successes: " + numberOfSuccesses);
        Console.WriteLine("Statistics: "
            + ((double)numberOfSuccesses * 100 / numberOfExperiments).ToString("N2") + "%");
    }
}