namespace Infrastructure.Caching.Distributed;

public class RedisCacheOptions
{
    public const string SectionName = "Redis";

    public int DefaultTTLSeconds { get; set; }
}
