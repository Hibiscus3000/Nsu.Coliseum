using Strategy;

class MainClass
{
    public static void Main(string[] args)
    {
        var elonStrategy = new ZeroStrategy();
        var markStrategy = new ZeroStrategy();

        var numberOfExperiments = 1_000_000;
        if (args.Length > 0)
        {
            int.TryParse(args[0], out numberOfExperiments);
        }

        Gods.Play(elonStrategy, markStrategy, numberOfExperiments);
    }
}