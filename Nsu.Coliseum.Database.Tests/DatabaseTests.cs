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

    private ExperimentsContext CreateContext() => new(_contextOptions);

    private IDeckProvider CreateDeckProvider(int numberOfDecks) => new RandomDeckProvider(numberOfDecks: numberOfDecks,
        numberOfCards: 36,
        new DeckShuffler());

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

    [Fact]
    public void AddThreeExperiments_ThreeExperimentsRead()
    {
        using var appContext = CreateContext();

        int numberOfExperiments = 3;

        IDeckProvider deckProvider = CreateDeckProvider(numberOfExperiments);

        for (int i = 0; i < numberOfExperiments; ++i)
        {
            var experimentEntity = new ExperimentEntity
            {
                Deck = deckProvider.GetDeck()
            };
            appContext.Experiments.Add(experimentEntity);
        }

        appContext.SaveChanges();

        Assert.Equal(numberOfExperiments, appContext.Experiments.Count());
    }

    [Fact]
    public void AddExperiment_ReadWrittenDeck()
    {
        using var appContext = CreateContext();

        IDeckProvider deckProvider = CreateDeckProvider(1);
        Deck.Deck deck = deckProvider.GetDeck()!;

        Deck.Deck deckCopy = new Deck.Deck(deck);

        appContext.Experiments.Add(new ExperimentEntity
        {
            Deck = deck
        });

        appContext.SaveChanges();

        Assert.Equal(appContext.Experiments.First().Deck, deckCopy);
    }
}