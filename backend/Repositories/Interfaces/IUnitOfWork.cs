using GEEKS.Models;

namespace GEEKS.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para la unidad de trabajo que coordina múltiples repositorios
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repositorio de productos
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Repositorio de categorías
        /// </summary>
        IRepository<Category> Categories { get; }

        /// <summary>
        /// Repositorio de usuarios
        /// </summary>
        IRepository<User> Users { get; }

        /// <summary>
        /// Repositorio de roles
        /// </summary>
        IRepository<Role> Roles { get; }

        /// <summary>
        /// Repositorio de carritos
        /// </summary>
        IRepository<Cart> Carts { get; }

        /// <summary>
        /// Repositorio de elementos del carrito
        /// </summary>
        IRepository<CartItem> CartItems { get; }

        /// <summary>
        /// Guarda todos los cambios pendientes
        /// </summary>
        /// <returns>Número de entidades afectadas</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Inicia una transacción
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Ejecuta una operación dentro de una transacción
        /// </summary>
        /// <param name="operation">Operación a ejecutar</param>
        /// <returns>Resultado de la operación</returns>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
    }
}
