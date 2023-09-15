using Strategy;

class MainClass
{
    public static void Main(string[] args)
    {
        var elonStrategy = new ZeroStrategy(); 
        var markStrategy = new ZeroStrategy();

        Gods.Play(elonStrategy, markStrategy, Int32.Parse(args[0]));
    }
}