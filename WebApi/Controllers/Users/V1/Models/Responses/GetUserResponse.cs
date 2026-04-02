namespace Api.Controllers.Users.V1.Models.Responses;

public sealed record GetUserResponse(Guid Id, string Name, int Age);
