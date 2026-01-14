namespace PasswordChecker.Server.Services.Interfaces;

public interface IMemoryCacheService
{
    /// <summary>
    /// Gets a value from cache or executes the factory function if not found
    /// </summary>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Gets a value from cache
    /// </summary>
    T? Get<T>(string key) where T : class;

    /// <summary>
    /// Sets a value in cache
    /// </summary>
    void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Removes a value from cache
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Removes multiple cache entries by key pattern
    /// </summary>
    void RemoveByPattern(string pattern);

    /// <summary>
    /// Clears all cache entries
    /// </summary>
    void Clear();
}
