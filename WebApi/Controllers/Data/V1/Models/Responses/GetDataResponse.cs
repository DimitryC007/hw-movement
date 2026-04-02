namespace Api.Controllers.Data.V1.Models.Responses;

public sealed record GetDataResponse(
    Guid Id,
    string Name,
    int Age,
    string? Description,
    string? Title,
    string? Content);
