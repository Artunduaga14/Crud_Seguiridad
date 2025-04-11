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
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //<summary>
        // Obtiene todos los roles almacenados en la base de datos.
        // </summary>
        // <returns>Lista de roles.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Name, 
                                Description, 
                                CreationDate
                            FROM Module
                            WHERE Active = 1";
                return await _context.QueryAsync<Module>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        // Método para obtener Rol ID SQL
        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name,  
                                    Description, 
                                    CreationDate
                                FROM Module 
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Module>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Module con el ID {ModuleId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un Model SQL
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                string query = @"
                            INSERT INTO Module (Name,Description, CreationDate, Active) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name,@Description, @CreationDate, @Active)";

                module.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    module.Name,
                    module.Description,
                    CreationDate = DateTime.Now,
                    Active = true,
                });

                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el model.");
                throw;
            }
        }

        //Metodo para actualizar Rol con SQL
        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                string query = @"UPDATE Module 
                        SET 
                            Name = @Name, 
                            Description = @Description 

                        WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    module.Id,
                    module.Name,
                    module.Description
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
                string query = @"UPDATE Module
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
                               DELETE Module
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
        public async Task<IEnumerable<Module>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Module>()
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
        public async Task<Module?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
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

        public async Task<Module> CreateLinQAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol {Rol}", module);
                throw;
            }
        }

        //<summary>
        //    Actualiza un rol existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol {Rol}", module);
                throw;
            }
        }

        //Metodo para eliminar Rol logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var module = await GetByIdAsyncLinq(id);
                if (module == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                module.Active = false; // O rol.Active = 0; si es un campo numérico
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
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                _context.Set<Module>().Remove(module);
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
