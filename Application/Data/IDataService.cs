using Domain;
using Domain;

namespace Application.Data;

/// <summary>
/// Handles business logic for data operations including validation, creation, and retrieval.
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Validates and persists a new data item, then returns the generated ID.
    /// </summary>
    /// <param name="data">The <see cref="DataItem"/> domain object to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <see cref="Guid"/> assigned to the new record.</returns>
    /// <exception cref="Common.Exceptions.ValidationException">Thrown when the data fails validation rules.</exception>
    Task<Guid> Create(DataItem data, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a data item by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching <see cref="DataItem"/> domain object.</returns>
    /// <exception cref="Common.Exceptions.NotFoundException{DataItem}">Thrown when no record exists with the given ID.</exception>
    Task<DataItem> Get(Guid id, CancellationToken cancellationToken);
}
