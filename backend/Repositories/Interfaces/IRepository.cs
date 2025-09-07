using GEEKS.Models;
using System.Linq.Expressions;

namespace GEEKS.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz base para repositorios
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que hereda de Base</typeparam>
    public interface IRepository<T> where T : Base
    {
        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <returns>Entidad encontrada o null</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        /// <returns>Lista de entidades</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene entidades que cumplen una condición
        /// </summary>
        /// <param name="predicate">Condición a evaluar</param>
        /// <returns>Lista de entidades que cumplen la condición</returns>
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Obtiene la primera entidad que cumple una condición
        /// </summary>
        /// <param name="predicate">Condición a evaluar</param>
        /// <returns>Primera entidad que cumple la condición o null</returns>
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Agrega una nueva entidad
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Elimina una entidad por su ID (soft delete)
        /// </summary>
        /// <param name="id">ID de la entidad a eliminar</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Elimina una entidad físicamente
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Verifica si existe una entidad con el ID especificado
        /// </summary>
        /// <param name="id">ID a verificar</param>
        /// <returns>True si existe, false en caso contrario</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Verifica si existe una entidad que cumple la condición especificada
        /// </summary>
        /// <param name="predicate">Condición a evaluar</param>
        /// <returns>True si existe, false en caso contrario</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Cuenta el número de entidades que cumplen una condición
        /// </summary>
        /// <param name="predicate">Condición a evaluar</param>
        /// <returns>Número de entidades</returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Obtiene entidades paginadas
        /// </summary>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="predicate">Condición opcional</param>
        /// <param name="orderBy">Función de ordenamiento opcional</param>
        /// <returns>Lista paginada de entidades</returns>
        Task<IEnumerable<T>> GetPagedAsync(
            int page, 
            int pageSize, 
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    }
}
