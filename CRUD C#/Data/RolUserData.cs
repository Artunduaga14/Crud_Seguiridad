using Entity.Context;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolUserData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public RolUserData(ApplicationDbContext context, ILogger<RolUserData> logger)
        {
            _context = context;
            _logger = logger;
        }


        ///<summary>
        /// Obtiene todos los registros de RolUser activos almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de relaciones entre usuarios y roles.</returns>
        public async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            try
            {
                string query = @"
                                SELECT RU.Id,
                                       RU.UserId,
                                       RU.RolId,
                                       RU.Active,
                                       U.Username AS Username,
                                       R.Name AS RolName
                                FROM RolUser RU
                                INNER JOIN [User] U ON RU.UserId = U.Id
                                INNER JOIN Rol R ON RU.RolId = R.Id
                                WHERE RU.Active = 1";

                return await _context.QueryAsync<RolUser>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los registros de RolUser.");
                throw;
            }
        }

        ///<summary>
        /// Obtiene una relación entre usuario y rol específica por su identificador.
        ///</summary>
        ///<param name="id">ID del RolUser.</param>
        ///<returns>Objeto RolUser correspondiente al ID, o null si no se encuentra.</returns>
        public async Task<RolUser?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"
                                SELECT RU.Id,
                                       RU.UserId,
                                       RU.RolId,
                                       RU.Active,
                                       U.Username AS Username,
                                       R.Name AS RolName
                                FROM RolUser RU
                                INNER JOIN [User] U ON RU.UserId = U.Id
                                INNER JOIN Rol R ON RU.RolId = R.Id
                                WHERE RU.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<RolUser>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolUser con ID {RolUserId}", id);
                throw;
            }
        }

        ///<summary>
        /// Crea un nuevo registro en la tabla RolUser.
        ///</summary>
        ///<param name="rolUser">Instancia de RolUser a crear.</param>
        ///<returns>El RolUser creado con su ID asignado.</returns>
        public async Task<RolUser> CreateAsync(RolUser rolUser)
        {
            try
            {
                string query = @"
                                INSERT INTO RolUser (UserId, RolId, Active)
                                OUTPUT INSERTED.Id
                                VALUES (@UserId, @RolId, @Active);";

                rolUser.Id = await _context.ExecuteScalarAsync<int>(query, new
                {
                    rolUser.UserId,
                    rolUser.RolId,
                    Active = true
                });

                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el RolUser.");
                throw;
            }
        }

        ///<summary>
        /// Actualiza un registro existente en la tabla RolUser.
        ///</summary>
        ///<param name="rolUser">Instancia de RolUser con los nuevos datos.</param>
        ///<returns>True si la actualización fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RolUser rolUser)
        {
            try
            {
                string query = @"
                                UPDATE RolUser
                                SET 
                                    UserId = @UserId,
                                    RolId = @RolId,
                                    Active = @Active
                                WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    rolUser.Id,
                    rolUser.UserId,
                    rolUser.RolId,
                    rolUser.Active
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el RolUser: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar User logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE RolUser
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente rolUser: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar User persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE RolUser
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar rolUser: {ex.Message}");
                return false;
            }
        }

        // Consulta con LINQ
        public async Task<IEnumerable<RolUser>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<RolUser>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los RolUser con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un rolUser especifíco por su identificador.

        // Consulta con LINQ
        public async Task<RolUser?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<RolUser>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rolUser con el ID {RolUserId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }


        //<summary>
        //    Crea un nuevo rolUser en la base de datos.
        //</summary>
        //<param name="rol">Instancia del rolUser a crear.</param>
        //<returns>El rolUser creado.</returns>

        public async Task<RolUser> CreateLinQAsync(RolUser rolUser)
        {
            try
            {
                await _context.Set<RolUser>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rolUser {rolUser}", rolUser);
                throw;
            }
        }


        //<summary>
        //    Actualiza un rolUser existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(RolUser rolUser)
        {
            try
            {
                _context.Set<RolUser>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolUser {User}", rolUser);
                throw;
            }
        }

        //Metodo para eliminar User logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var rolUser = await GetByIdAsyncLinq(id);
                if (rolUser == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                rolUser.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el rolUser con ID {id}", id);
                throw;
            }
        }



        //<summary>
        //    Elimina un rolUser de la base de datos.
        //</summary>
        //<param name="rol">Identificador único del rolUser a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<RolUser>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<RolUser>().Remove(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rolUser: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<RolUserDto>> GetAllAsyncSQL()
        {
            throw new NotImplementedException();
        }
    }
}
