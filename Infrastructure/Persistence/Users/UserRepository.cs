using Application.Users;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Users;

internal class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return entity?.ToDomain();
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        var entity = user.ToEntity();
        var saved = await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return saved.Entity.ToDomain();
    }
}
