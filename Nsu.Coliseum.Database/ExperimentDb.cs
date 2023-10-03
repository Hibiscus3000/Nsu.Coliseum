﻿using System.ComponentModel.DataAnnotations.Schema;
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
}

public class ApplicationContext : DbContext
{
    public DbSet<ExperimentEntity> Experiments => Set<ExperimentEntity>();

    private readonly string _connectionString;

    public ApplicationContext()
    {
        string projectPath = GetProjectPath();

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

    private static string GetProjectPath([CallerFilePath] string? callerFilePath = null)
    {
        return Directory.GetParent(callerFilePath).FullName;
    }
}