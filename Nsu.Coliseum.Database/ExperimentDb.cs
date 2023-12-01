using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Nsu.Coliseum.Database;

/// <summary>
/// Represent database deck entry
/// </summary>
public class ExperimentEntity
{
    private static readonly string Separator = "; ";
    
    public int Id { get; set; }
    
    /// <summary>
    /// What will actually be stored in db.
    /// </summary>
    /// <remarks>
    /// Formed as all cards from <see cref="Deck"/> string representation (obtained using <c>ToString()</c> method) joined by
    /// <c>Separator</c>.
    /// </remarks>
    public string StringRepresentation { get; set; }

    [NotMapped]
    public Deck.Deck Deck
    {
        get => new(StringRepresentation, Separator);
        set => StringRepresentation = value.ToString(Separator);
    }

    public ExperimentEntity()
    {
    }

    public ExperimentEntity(Deck.Deck deck)
    {
        Deck = deck;
    }
}

/// <summary>
/// DbContext successor, which is used to query database with predefined decks.
/// </summary>
public class ExperimentsContext : DbContext
{
    internal DbSet<ExperimentEntity> Experiments => Set<ExperimentEntity>();

    private readonly string _connectionString;

    /// <summary>
    /// Constructor mainly used for testing uning in memory SQLite.
    /// </summary>
    /// <param name="options"></param>
    public ExperimentsContext(DbContextOptions options) : base(options) => Database.EnsureCreated();

    /// <summary>
    /// Main constructor, which uses SQLite.
    /// </summary>
    public ExperimentsContext()
    {
        // creating connection string
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        
        _connectionString = "DataSource=" + GetProjectPath() + "/" + config.GetConnectionString("DataSource");
        
        //ensuring that database exists
        Database.EnsureCreated();
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // using database on disk
            // _connectionString preconfigured in constructor
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
    
    /// <returns>Nsu.Coliseum.Database project path.</returns>
    private static string GetProjectPath([CallerFilePath] string? callerFilePath = null)
    {
        return Directory.GetParent(callerFilePath).FullName;
    }
}

/// <summary>
/// Middleware between ExperimentContext and other code issuing queries to database. 
/// </summary>
public interface IExperimentRepository
{
    IEnumerable<ExperimentEntity> GetAllExperiments();

    void AddExperiment(ExperimentEntity experiment);

    /// <summary>
    /// Get <c>amount</c> entries from Experiments table, which ids are greater than <c>id</c>.
    /// </summary>
    /// <param name="id">id lower boundary.</param>
    /// <param name="amount">number of entries to fetch.</param>
    /// <returns>list of Experiment Entities.</returns>
    List<ExperimentEntity> GetExperimentsIdGreaterThen(int id, int amount);

    void SaveChanges();
}

public class ExperimentRepository : IExperimentRepository
{
    private readonly ExperimentsContext _context;

    public ExperimentRepository(ExperimentsContext context) => _context = context;

    public IEnumerable<ExperimentEntity> GetAllExperiments() => _context.Experiments;

    public void AddExperiment(ExperimentEntity experiment) => _context.Experiments.Add(experiment);

    public List<ExperimentEntity> GetExperimentsIdGreaterThen(int id, int amount) => _context.Experiments
        .Where(e => e.Id > id)
        .Take(amount)
        .ToList();

    public void SaveChanges() => _context.SaveChanges();
}