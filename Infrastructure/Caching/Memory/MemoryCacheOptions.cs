using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Caching.Memory;

public class MemoryCacheOptions
{
    public const string SectionName = "MemoryCache";

    [Range(3, 100)]
    public int Capacity { get; set; }
}
