using Api.Controllers.Users.V1.Models.Requests;
using Domain.Users;

namespace Api.Controllers.Users.V1.Mappings;

internal static class UserMappings
{
    internal static User ToDomain(this CreateUserRequest request) => new()
    {
        Name = request.Name,
        Age = request.Age
    };
}
