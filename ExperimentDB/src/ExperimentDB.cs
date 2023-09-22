using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StrategyInterface;

namespace ExperimentDB;

public class ExperimentEntity
{
    private static readonly string Separator = "; ";

    public int Id { get; set; }
    public string StringRepresentation { get; set; }

    [NotMapped]
    public Deck Deck
    {
        get => new(StringRepresentation, Separator);
        set => StringRepresentation = value.ToString(Separator);
    }
}

public class ApplicationContext : DbContext
{
    public DbSet<ExperimentEntity> Experiments => Set<ExperimentEntity>();

    private readonly string _connectionString;

    public ApplicationContext()
    {
        string projectPath = ProjectSourcePath.getProjectPath();

        var builder = new ConfigurationBuilder();
        builder.SetBasePath(projectPath);
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        _connectionString = "DataSource=" + projectPath + "/" + config.GetConnectionString("DataSource");

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }
}

internal static class ProjectSourcePath
{
    private const string myRelativePath = nameof(ProjectSourcePath) + ".cs";
    private static string? lazyValue;
    public static string Value => lazyValue ??= calculatePath();

    private static string calculatePath()
    {
        string pathName = GetSourceFilePathName();
        return pathName.Substring(0, pathName.Length - myRelativePath.Length);
    }

    private static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null)
        => callerFilePath ?? "";

    public static string getProjectPath([CallerFilePath] string? callerFilePath = null)
    {
        string sourceFilePathName = GetSourceFilePathName(callerFilePath);
        return Directory.GetParent(sourceFilePathName).Parent.FullName;
    }
}