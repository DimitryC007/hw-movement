using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

internal class DataRepository : Application.Data.IDataRepository
{
    private readonly AppDbContext _context;

    public DataRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DataItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.DataSet.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return entity?.ToDomain();
    }

    public async Task<DataItem> AddAsync(DataItem data, CancellationToken cancellationToken)
    {
        var entity = data.ToEntity();
        var saved = await _context.DataSet.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return saved.Entity.ToDomain();
    }
}
