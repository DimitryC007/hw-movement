using Application.Common.Exceptions;
using Domain.Users;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;
    private readonly IValidator<User> _validator;

    public UserService(IUserRepository repository, ILogger<UserService> logger, IValidator<User> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Guid> Create(User user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with name: {Name}", user.Name);

        var validationResult = await _validator.ValidateAsync(user, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            _logger.LogWarning("User creation failed — validation errors: {Errors}", errors);

            throw new ValidationException(validationResult.Errors);
        }

        user.Id = Guid.NewGuid();

        try
        {
            var savedUser = await _repository.AddAsync(user, cancellationToken);

            _logger.LogInformation("User created successfully with Id {UserId}", savedUser.Id);

            return savedUser.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user with name: {Name}", user.Name);
            throw;
        }
    }

    public async Task<User> Get(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching user with Id {UserId}", id);

        User? user = null;
        try
        {
            user = await _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve user with Id {UserId}", id);
            throw;
        }

        if (user is null)
        {
            _logger.LogWarning("User with Id {UserId} was not found", id);
            throw new NotFoundException<User>(id);
        }

        _logger.LogInformation("User with Id {UserId} retrieved successfully", id);

        return user;
    }
}
