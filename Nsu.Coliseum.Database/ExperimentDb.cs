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
    public DbSet<ExperimentEntity> Experiments => Set<ExperimentEntity>();

    private readonly string _connectionString;

    public ExperimentsContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    public ExperimentsContext()
    {
        string projectPath = GetProjectPath();
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(projectPath);
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();

        _connectionString = "DataSource=" + projectPath + "/" + config.GetConnectionString("DataSource");
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