using Domain.Users;

namespace Application.Users;

/// <summary>
/// Handles business logic for user operations including validation, creation, and retrieval.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Validates and persists a new user, then returns the generated ID.
    /// </summary>
    /// <param name="user">The user domain object to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <see cref="Guid"/> assigned to the new user.</returns>
    /// <exception cref="Common.Exceptions.ValidationException">Thrown when the user fails validation rules.</exception>
    Task<Guid> Create(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching <see cref="User"/> domain object.</returns>
    /// <exception cref="Common.Exceptions.NotFoundException{User}">Thrown when no user exists with the given ID.</exception>
    Task<User> Get(Guid id, CancellationToken cancellationToken);
}
