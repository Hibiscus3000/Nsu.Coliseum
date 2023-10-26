using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Database;

//TODO add number of experiments limit
public class DbDeckProvider : IDeckProvider
{
    private const int ReadAtOnce = 20;

    private List<ExperimentEntity>? _experimentEntities = null;
    private int currentEntity = 0;

    private int _lastId = 0;

    private readonly int _numberOfExperimentLimit;
    private int _numberOfExperimentsRead = 0;

    public DbDeckProvider(int numberOfExperimentLimit = 100)
    {
        _numberOfExperimentLimit = numberOfExperimentLimit;
    }

    private void ReadEntities()
    {
        // check number of experiments limit
        if (_numberOfExperimentsRead >= _numberOfExperimentLimit)
        {
            _experimentEntities = null;
            return;
        }

        using (ExperimentsContext appContext = new ExperimentsContext())
        {
            //read entities from db
            _experimentEntities = appContext.Experiments
                .Where(e => e.Id > _lastId)
                .Take(ReadAtOnce)
                .ToList();

            if (_experimentEntities.Count > 0)
            {
                _lastId = _experimentEntities[^1].Id;

                // check number of experiments limit
                if (_experimentEntities.Count + _numberOfExperimentsRead > _numberOfExperimentLimit)
                {
                    _experimentEntities =
                        _experimentEntities.GetRange(0, _numberOfExperimentLimit - _numberOfExperimentsRead);
                }

                _numberOfExperimentsRead += _experimentEntities.Count;
            }

            currentEntity = 0;
        }
    }

    public Deck.Deck? GetDeck()
    {
        if (null == _experimentEntities || currentEntity == _experimentEntities.Count)
        {
            ReadEntities();
            if (null == _experimentEntities || 0 == _experimentEntities.Count) return null;
        }

        return _experimentEntities[currentEntity++].Deck;
    }
}