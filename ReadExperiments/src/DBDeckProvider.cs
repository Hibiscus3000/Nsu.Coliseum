using ExperimentDB;
using StrategyInterface;

namespace ReadExperiments;

public class DBDeckProvider : IDeckProvider
{
    //private static readonly int ReadAtOnce = 1000;
    //private int currentPage;

    private List<ExperimentEntity> _experimentEntities;
    private int currentEntity = 0;

    public DBDeckProvider()
    {
        using (ApplicationContext appContext = new ApplicationContext())
        {
            _experimentEntities = appContext.Experiments.ToList();
        }
    }

    // private void ReadEntities()
    // {
    //     using (ApplicationContext appContext = new ApplicationContext())
    //     {
    //         _experimentEntities = appContext.Experiments
    //             .Where(e => e.Id / ReadAtOnce == currentPage)
    //             .Take(ReadAtOnce)
    //             .ToList();
    //         ++currentPage;
    //         currentEntity = 0;
    //     }
    // }

    public Deck? GetDeck()
    {
        ExperimentEntity? entity =
            currentEntity < _experimentEntities.Count ? _experimentEntities[currentEntity++] : null;
        if (null != entity)
        {
            Console.WriteLine(entity.Id + ": " + entity.Deck.ToString(" "));
        }

        return entity?.Deck;
    }
    //{
    // if (currentEntity == _experimentEntities.Count)
    // {
    //     ReadEntities();
    //     if (0 == _experimentEntities.Count)
    //     {
    //         return null;
    //     }
    // }
    //
    // return _experimentEntities[currentEntity++].Deck;
    //}
}