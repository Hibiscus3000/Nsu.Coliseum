using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Database;

//TODO add number of experiments limit
public class DBDeckProvider : IDeckProvider
{
    private const int ReadAtOnce = 20;

    private List<ExperimentEntity>? _experimentEntities = null;
    private int currentEntity = 0;

    private int? _lastId = null;

    private readonly int _numberOfExperimentLimit;

    public DBDeckProvider(int numberOfExperimentLimit = 100)
    {
        _numberOfExperimentLimit = numberOfExperimentLimit;
    }

    private void ReadEntities()
    {
        using (ApplicationContext appContext = new ApplicationContext())
        {
            if (null == _lastId)
            {
                ReadEntitiesFirstTime(appContext);
            }
            else
            {
                ReadEntitiesSubsequent(appContext);
            }

            if (_experimentEntities.Count > 0)
            {
                _lastId = _experimentEntities[^1].Id;
            }

            currentEntity = 0;
        }
    }

    private void ReadEntitiesFirstTime(ApplicationContext appContext)
    {
        _experimentEntities = appContext.Experiments
            .Take(ReadAtOnce)
            .ToList();
    }

    private void ReadEntitiesSubsequent(ApplicationContext appContext)
    {
        _experimentEntities = appContext.Experiments
            .Where(e => e.Id > _lastId)
            .Take(ReadAtOnce)
            .ToList();
    }

    public Deck.Deck? GetDeck()
    {
        if (null == _experimentEntities || currentEntity == _experimentEntities.Count)
        {
            ReadEntities();
            if (0 == _experimentEntities.Count)
            {
                return null;
            }
        }

        ExperimentEntity experimentEntity = _experimentEntities[currentEntity];

        Console.WriteLine(experimentEntity.Id + ": " + experimentEntity.Deck.ToString(" "));

        return _experimentEntities[currentEntity++].Deck;
    }
}