using Domain.Users;

using Domain.Users;

namespace Infrastructure.Persistence.Users;

internal static class UserMappings
{
    internal static User ToDomain(this UserEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Age = entity.Age
    };

    internal static UserEntity ToEntity(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Age = user.Age
    };
}
