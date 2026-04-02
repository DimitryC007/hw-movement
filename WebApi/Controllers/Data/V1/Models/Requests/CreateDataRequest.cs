namespace Api.Controllers.Data.V1.Models.Requests;

public sealed record CreateDataRequest(
    string Name,
    int Age,
    string? Description,
    string? Title,
    string? Content);
