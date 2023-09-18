using StrategyInterface;

public static class Gods
{
    private static readonly int _numberOfCards = 36;
    private static readonly IDeckShuffler _deckShuffler = new DeckShuffler();

    public static void Play(Opponent elon, Opponent mark, int numberOfExperiments)
    {
        int numberOfSuccesses = 0;

        for (int i = 0; i < numberOfExperiments; ++i)
        {
            if (Experiment.Execute(elon, mark, _deckShuffler, _numberOfCards))
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