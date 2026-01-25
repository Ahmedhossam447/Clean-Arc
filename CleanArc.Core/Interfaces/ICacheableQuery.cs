namespace CleanArc.Core.Interfaces
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
    }
}