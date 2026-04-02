using Domain;

namespace Application.Data;

public interface IDataRepository
{
    Task<DataItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DataItem> AddAsync(DataItem data, CancellationToken cancellationToken = default);
}
