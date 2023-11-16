using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Database;

public class DbDeckProvider : IDeckProvider
{
    private const int ReadAtOnce = 100;

    private List<ExperimentEntity>? _experimentEntities = null;

    // index of next to provide entity from _experimentsEntities
    private int _currentEntity = 0;

    // id of the last read entity
    private int _lastId = 0;

    private readonly int _numberOfExperimentLimit;
    private int _numberOfExperimentsRead = 0;

    public DbDeckProvider(DeckSizeAndNumOfDecks deckSizeAndNumOfDecks)
    {
        _numberOfExperimentLimit = deckSizeAndNumOfDecks.NumberOfDecks;
    }

    private void ReadEntities()
    {
        // check number of experiments limit
        if (_numberOfExperimentsRead >= _numberOfExperimentLimit)
        {
            _experimentEntities = null;
            return;
        }
        
        using var appContext = new ExperimentsContext();
        var experimentRepository = new ExperimentRepository(appContext);

        //read entities from db
        _experimentEntities = experimentRepository.GetExperimentsIdGreaterThen(_lastId, ReadAtOnce);

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

        _currentEntity = 0;
    }

    public Deck.Deck? GetDeck()
    {
        if (null == _experimentEntities || _currentEntity == _experimentEntities.Count)
        {
            ReadEntities();
            if (null == _experimentEntities || 0 == _experimentEntities.Count) return null;
        }

        return _experimentEntities[_currentEntity++].Deck;
    }
}