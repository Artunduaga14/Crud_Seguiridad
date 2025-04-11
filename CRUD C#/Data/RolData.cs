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
    public class RolData
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }


        //<summary>
        // Obtiene todos los roles almacenados en la base de datos.
        // </summary>
        // <returns>Lista de roles.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Name, 
                                Code, 
                                Description, 
                                Active
                            FROM Rol
                            WHERE Active = 1";
                return await _context.QueryAsync<Rol>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        // Método para obtener Rol ID SQL
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name, 
                                    Code, 
                                    Description, 
                                    Active 
                                FROM Rol 
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Rol>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con el ID {RolId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                string query = @"
                            INSERT INTO Rol (Name, Code, Description, Active) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Code, @Description, @Active)";

                rol.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    rol.Name,
                    rol.Code,
                    rol.Description,
                    Active = true,
                });

                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol.");
                throw;
            }
        }


        //Metodo para actualizar Rol con SQL
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                string query = @"UPDATE Rol 
                                SET 
                                    Name = @Name, 
                                    Code = @Code, 
                                    Description = @Description, 
                                    Active = @Active 
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    rol.Id,
                    rol.Name,
                    rol.Code,
                    rol.Description,
                    rol.Active
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
                string query = @"UPDATE Rol
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Rol persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Rol
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar rol: {ex.Message}");
                return false;
            }
        }

        // Consulta con LINQ
        public async Task<IEnumerable<Rol>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Rol>()
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
        public async Task<Rol?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
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

        public async Task<Rol> CreateLinQAsync(Rol rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol {Rol}", rol);
                throw;
            }
        }

        //<summary>
        //    Actualiza un rol existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol {Rol}", rol);
                throw;
            }
        }


        //Metodo para eliminar Rol logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var rol = await GetByIdAsyncLinq(id);
                if (rol == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                rol.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el rol con ID {id}", id);
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
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                _context.Set<Rol>().Remove(rol);
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
