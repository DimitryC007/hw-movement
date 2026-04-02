using Application.Common.Exceptions;
using Domain;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.Data;

public class DataService : IDataService
{
    private readonly IDataRepository _repository;
    private readonly ILogger<DataService> _logger;
    private readonly IValidator<DataItem> _validator;

    public DataService(IDataRepository repository, ILogger<DataService> logger, IValidator<DataItem> validator)
    {
        _repository = repository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Guid> Create(DataItem data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating data with name: {Name}", data.Name);

        var validationResult = await _validator.ValidateAsync(data, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            _logger.LogWarning("Data creation failed - validation errors: {Errors}", errors);

            throw new ValidationException(validationResult.Errors);
        }

        data.Id = Guid.NewGuid();

        try
        {
            var saved = await _repository.AddAsync(data, cancellationToken);

            _logger.LogInformation("Data created successfully with Id {DataId}", saved.Id);

            return saved.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save data with name: {Name}", data.Name);
            throw;
        }
    }

    public async Task<DataItem> Get(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching data with Id {DataId}", id);

        DataItem? data = null;
        try
        {
            data = await _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve data with Id {DataId}", id);
            throw;
        }

        if (data is null)
        {
            _logger.LogWarning("Data with Id {DataId} was not found", id);
            throw new NotFoundException<DataItem>(id);
        }

        _logger.LogInformation("Data with Id {DataId} retrieved successfully", id);

        return data;
    }
}
