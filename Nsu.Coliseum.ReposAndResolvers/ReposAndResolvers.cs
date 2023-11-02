using System.Collections.Concurrent;

namespace ReposAndResolvers;

public interface IRepo<T>
{
    void AddT(Guid id, T color);
    T? GetT(Guid id);
    bool ContainsId(Guid id);
}

public class Repo<T> : IRepo<T>
{
    private readonly IDictionary<Guid, T> _repoDict = new ConcurrentDictionary<Guid, T>();

    public void AddT(Guid id, T t) => _repoDict.Add(id, t);

    public T? GetT(Guid id) => _repoDict.TryGetValue(id, out T value) ? value : default;

    public bool ContainsId(Guid id) => _repoDict.ContainsKey(id);
}

public enum OpponentType
{
    Elon,
    Mark
}

public class OpponentUrl
{
    public string Value { get; init; }
}

public interface IResolver<T>
{
    void AddT(OpponentType type, T t);

    public T GetT(OpponentType type);
}

public class Resolver<T> : IResolver<T>
{
    private readonly IDictionary<OpponentType, T> _resolverDict = new Dictionary<OpponentType, T>();

    public void AddT(OpponentType type, T t) => _resolverDict.Add(type, t);

    public T GetT(OpponentType type) => _resolverDict[type];
}