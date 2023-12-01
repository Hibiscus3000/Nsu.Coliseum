using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Database;

/// <summary>
/// Objects of this class are meant for providing predefined decks for experiment.  
/// </summary>
public class DbDeckProvider : IDeckProvider
{
    /// <summary>
    /// Number of db experiment entries to read at once (Page volume).
    /// </summary>
    private const int ReadAtOnce = 100;
    
    /// <summary>
    /// Current list of experiment entities.
    /// </summary>
    private List<ExperimentEntity>? _experimentEntities = null;

    /// <summary>
    /// Index of next to provide entity from <see cref="_experimentEntities"/>.
    /// </summary>
    private int _currentEntity = 0;

    /// <summary>
    /// Id of the last read entity. Used to query new page of experiment entries from db.
    /// Defaults to zero since id is non-negative number. 
    /// </summary>
    private int _lastId = 0;

    /// <summary>
    /// Maximum amount of decks to provide.
    /// </summary>
    private readonly int _numberOfExperimentLimit;
    
    /// <summary>
    /// Used for checking if <see cref="_numberOfExperimentLimit"/>> is reached.
    /// </summary>
    private int _numberOfExperimentsRead = 0;

    /// <param name="deckSizeAndNumOfDecks">only number of decks is used</param>
    public DbDeckProvider(DeckSizeAndNumOfDecks deckSizeAndNumOfDecks) => _numberOfExperimentLimit = deckSizeAndNumOfDecks.NumberOfDecks;

    /// <summary>
    /// Method is called then all entries from <see cref="_experimentEntities"/>> were used for deck providing.
    /// </summary>
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

        // read entities from db
        _experimentEntities = experimentRepository.GetExperimentsIdGreaterThen(_lastId, ReadAtOnce);

        // check that Experiments table isn't over
        if (_experimentEntities.Count > 0)
        {
            // update last id
            _lastId = _experimentEntities[^1].Id;

            // check number of experiments limit
            if (_experimentEntities.Count + _numberOfExperimentsRead > _numberOfExperimentLimit)
            {
                // remove list tail which exceeds number of experiments limit
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
            // if condition is true then either Experiments table is over or experiments limit is reached
            if (null == _experimentEntities || 0 == _experimentEntities.Count) return null;
        }

        return _experimentEntities[_currentEntity++].Deck;
    }
}