using System.Collections.Concurrent;

namespace ReposAndResolvers;

public interface IRepo<T>
{
    void AddT(long experimentNum, T color);
    T? GetT(long experimentNum);
    bool ContainsExpNum(long experimentNum);
}

public class Repo<T> : IRepo<T>
{
    private readonly IDictionary<long, T> _repoDict = new ConcurrentDictionary<long, T>();

    public void AddT(long experimentNum, T t) => _repoDict.Add(experimentNum, t);

    public T? GetT(long experimentNum) => _repoDict[experimentNum];

    public bool ContainsExpNum(long experimentNum) => _repoDict.ContainsKey(experimentNum);
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

public class TupleRepository<F,S>
{
    private IDictionary<long, (F?, S?)> _dict = new Dictionary<long, (F? deck, S? cardNumber)>();
    
    private object _repoLock = new();

    public bool AddFirst(long experimentNum, F first)
    {
        lock (_repoLock)
        {
            if (_dict.TryGetValue(experimentNum, out var t))
            {
                t.Item1 = first;
                _dict[experimentNum] = t;
                return true;
            }

            _dict.Add(experimentNum, (first, default));
            return false;
        }
    }
    
    public bool AddSecond(long experimentNum, S second)
    {
        lock (_repoLock)
        {
            if (_dict.TryGetValue(experimentNum, out var t))
            {
                t.Item2 = second;
                _dict[experimentNum] = t;
                return true;
            }

            _dict.Add(experimentNum, (default, second));
            return false;
        }
    }
    
    public F GetFirst(long experimentNum) => _dict[experimentNum].Item1!;
    
    public S GetSecond(long experimentNum) => _dict[experimentNum].Item2!;
} 