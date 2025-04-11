using Entity.Context;
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
   public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }


        //<summary>
        // Obtiene todos los roles almacenados en la base de datos.
        // </summary>
        // <returns>Lista de roles.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Name, 
                                Code, 
                                Description 
                            FROM Permission
                            WHERE Active = 1";
                return await _context.QueryAsync<Permission>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        // Método para obtener Rol ID SQL
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name, 
                                    Code, 
                                    Description 
                                FROM Permission 
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Permission>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permission con el ID {PermissionId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }


        //Metodo para crear un ROL SQL
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                string query = @"
                            INSERT INTO Permission (Name, Code, Description, Active) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Code, @Description, @Active)";

                permission.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    permission.Name,
                    permission.Code,
                    permission.Description,
                    Active = true,
                });

                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permission.");
                throw;
            }
        }


        //Metodo para actualizar Rol con SQL
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                string query = @"UPDATE Permission 
                                SET 
                                    Name = @Name, 
                                    Code = @Code, 
                                    Description = @Description 
                                  
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    permission.Id,
                    permission.Name,
                    permission.Code,
                    permission.Description
                   
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Rol logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Permission
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente permission: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Rol persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Permission
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar permision: {ex.Message}");
                return false;
            }
        }

        // Consulta con LINQ
        public async Task<IEnumerable<Permission>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Permission>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un rol especifíco por su identificador.



        // Consulta con LINQ
        public async Task<Permission?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con el ID {RolId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo rol en la base de datos.
        //</summary>
        //<param name="rol">Instancia del rol a crear.</param>
        //<returns>El rol creado.</returns>

        public async Task<Permission> CreateLinQAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permission {Permission}", permission);
                throw;
            }
        }

        //<summary>
        //    Actualiza un rol existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permission {Permission}", permission);
                throw;
            }
        }

        //Metodo para eliminar Rol logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var permission = await GetByIdAsyncLinq(id);
                if (permission == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                permission.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el permission con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un rol de la base de datos.
        //</summary>
        //<param name="rol">Identificador único del rol a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var permission = await _context.Set<Permission>().FindAsync(id);
                if (permission == null)
                    return false;

                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol: {ex.Message}");
                return false;
            }
        }
    }
}
