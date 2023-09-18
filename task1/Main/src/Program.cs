using Strategy;

class Program
{
    public static void Main(string[] args)
    {
        var elonStrategy = new ZeroStrategy();
        var markStrategy = new ZeroStrategy();

        Opponent elon = new(elonStrategy);
        Opponent mark = new(markStrategy);

        var numberOfExperiments = 1_000_000;
        if (args.Length > 0)
        {
            int.TryParse(args[0], out numberOfExperiments);
        }

        Gods.Play(elon, mark, numberOfExperiments);
    }
}