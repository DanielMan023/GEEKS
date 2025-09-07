using GEEKS.Data;
using GEEKS.Models;
using GEEKS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GEEKS.Repositories
{
    /// <summary>
    /// Implementación de la unidad de trabajo
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(DBContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new BaseRepository<Category>(_context);
            Users = new BaseRepository<User>(_context);
            Roles = new BaseRepository<Role>(_context);
            Carts = new BaseRepository<Cart>(_context);
            CartItems = new BaseRepository<CartItem>(_context);
        }

        public IProductRepository Products { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<User> Users { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<Cart> Carts { get; }
        public IRepository<CartItem> CartItems { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            await BeginTransactionAsync();
            try
            {
                var result = await operation();
                await CommitTransactionAsync();
                return result;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
