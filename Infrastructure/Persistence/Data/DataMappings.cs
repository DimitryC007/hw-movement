using Domain;
using Domain;

namespace Infrastructure.Persistence.Data;

internal static class DataMappings
{
    internal static DataItem ToDomain(this DataEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Age = entity.Age,
        Description = entity.Description,
        Title = entity.Title,
        Content = entity.Content
    };

    internal static DataEntity ToEntity(this DataItem data) => new()
    {
        Id = data.Id,
        Name = data.Name,
        Age = data.Age,
        Description = data.Description,
        Title = data.Title,
        Content = data.Content
    };
}
