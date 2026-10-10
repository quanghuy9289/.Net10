using System.Collections.Concurrent;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Domain.Repository;

public class InMemoryStore
{
    private readonly ConcurrentDictionary<Type, object> _store = new();

    public InMemoryStore()
    {
        // Initialize the store with empty dictionaries for each entity type
        _store[typeof(Customer)] = new ConcurrentDictionary<Guid, Customer>();
    }

    public ConcurrentDictionary<Guid, T> GetStore<T>() where T : class
    {
        if (_store.TryGetValue(typeof(T), out var store))
        {
            return (ConcurrentDictionary<Guid, T>)store;
        }
        throw new InvalidOperationException($"No store found for type {typeof(T).Name}");
    }
}