using Api.Controllers.Data.V1.Mappings;
using Api.Controllers.Data.V1.Models.Requests;
using Api.Controllers.Data.V1.Models.Responses;
using Application.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Data;

/// <summary>
/// Manages user resources.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
public class DataController : ControllerBase
{
    private readonly IUserService _userService;

    public DataController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user creation request containing name and age.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created user.</returns>
    /// <response code="200">User created successfully.</response>
    /// <response code="400">Validation failed (e.g. missing name, invalid age).</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var userId = await _userService.Create(request.ToDomain(), cancellationToken);

        return Ok(new CreateUserResponse(userId));
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's details.</returns>
    /// <response code="200">User found and returned.</response>
    /// <response code="404">No user exists with the given ID.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserResponse>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.Get(id, cancellationToken);

        return Ok(new GetUserResponse(user.Id, user.Name, user.Age));
    }
}
