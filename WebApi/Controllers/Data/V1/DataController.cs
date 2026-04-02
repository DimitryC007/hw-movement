using Api.Controllers.Data.V1.Mappings;
using Api.Controllers.Data.V1.Models.Requests;
using Api.Controllers.Data.V1.Models.Responses;
using Application.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Data;

/// <summary>
/// Manages data resources.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataService _dataService;

    public DataController(IDataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Creates a new data item.
    /// </summary>
    /// <param name="request">The creation request containing name and age.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created data item.</returns>
    /// <response code="200">Data item created successfully.</response>
    /// <response code="400">Validation failed (e.g. missing name, invalid age).</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateDataResponse>> Create([FromBody] CreateDataRequest request, CancellationToken cancellationToken)
    {
        var id = await _dataService.Create(request.ToDomain(), cancellationToken);

        return Ok(new CreateDataResponse(id));
    }

    /// <summary>
    /// Retrieves a data item by its unique identifier.
    /// </summary>
    /// <param name="id">The data item's GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The data item's details.</returns>
    /// <response code="200">Data item found and returned.</response>
    /// <response code="404">No data item exists with the given ID.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetDataResponse>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var data = await _dataService.Get(id, cancellationToken);

        return Ok(new GetDataResponse(data.Id, data.Name, data.Age, data.Description, data.Title, data.Content));
    }
}
