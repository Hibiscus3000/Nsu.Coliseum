using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Nsu.Coliseum.Database;

public class ExperimentEntity
{
    private static readonly string Separator = "; ";

    public int Id { get; set; }
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

public class ExperimentsContext : DbContext
{
    internal DbSet<ExperimentEntity> Experiments => Set<ExperimentEntity>();

    private readonly string _connectionString;

    public ExperimentsContext(DbContextOptions options) : base(options) => Database.EnsureCreated();

    public ExperimentsContext()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();

        _connectionString = "DataSource=" + GetProjectPath() + "/" + config.GetConnectionString("DataSource");
        
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }

    private static string GetProjectPath([CallerFilePath] string? callerFilePath = null)
    {
        return Directory.GetParent(callerFilePath).FullName;
    }
}

public interface IExperimentRepository
{
    IEnumerable<ExperimentEntity> GetAllExperiments();

    void AddExperiment(ExperimentEntity experiment);

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