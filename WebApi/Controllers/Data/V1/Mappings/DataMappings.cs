using Api.Controllers.Data.V1.Models.Requests;
using Domain;

namespace Api.Controllers.Data.V1.Mappings;

internal static class DataMappings
{
    internal static DataItem ToDomain(this CreateDataRequest request) => new()
    {
        Name = request.Name,
        Age = request.Age,
        Description = request.Description,
        Title = request.Title,
        Content = request.Content
    };
}
