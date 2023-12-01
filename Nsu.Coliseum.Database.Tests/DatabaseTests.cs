using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Database.Tests;

public class DatabaseTests : IDisposable
{
    private bool _disposed = false;

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions _contextOptions;

    public DatabaseTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ExperimentsContext>()
            .UseSqlite(_connection)
            .Options;
    }

    ~DatabaseTests()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) _connection.Dispose();
        _disposed = true;
    }

    private ExperimentsContext CreateContext() => new(_contextOptions);

    private IDeckProvider CreateDeckProvider(int numberOfDecks) => new RandomDeckProvider(numberOfDecks: numberOfDecks,
        numberOfCards: 36,
        deckShuffler: new DeckShuffler());


    [Fact]
    public void AddThreeExperiments_ThreeExperimentsRead()
    {
        // Arrange
        using var appContext = CreateContext();
        var experimentRepository = new ExperimentRepository(appContext);

        int numberOfExperiments = 3;

        IDeckProvider deckProvider = CreateDeckProvider(numberOfExperiments);

        // Act
        for (int i = 0; i < numberOfExperiments; ++i)
        {
            var experimentEntity = new ExperimentEntity
            {
                Deck = deckProvider.GetDeck()!
            };
            experimentRepository.AddExperiment(experimentEntity);
        }

        appContext.SaveChanges();
        
        // Assert
        Assert.Equal(numberOfExperiments, experimentRepository.GetAllExperiments().Count());
    }

    [Fact]
    public void AddExperiment_ReadWrittenDeck()
    {
        // Arrange
        using var appContext = CreateContext();
        var experimentRepository = new ExperimentRepository(appContext);

        IDeckProvider deckProvider = CreateDeckProvider(1);
        Deck.Deck deck = deckProvider.GetDeck()!;

        Deck.Deck deckCopy = new Deck.Deck(deck);
            
        // Act
        experimentRepository.AddExperiment(new ExperimentEntity
        {
            Deck = deck
        });

        appContext.SaveChanges();
        
        // Assert
        Assert.Equal(experimentRepository.GetAllExperiments().First().Deck, deckCopy);
    }
}