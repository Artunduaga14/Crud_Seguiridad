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
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        /// Obtiene todos los formularios activos junto con su módulo asociado.
        ///</summary>
        ///<returns>Lista de formularios.</returns>
        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            try
            {
                string Query = @"
                                SELECT 
                                    F.Id,    
                                    F.FormId, 
                                    F.ModuleId, 
                                    F.Active,
                                    M.Name AS ModuleName
                                FROM [FormModule] F
                                INNER JOIN [Module] M ON F.ModuleId = M.Id
                                WHERE F.Active = 1";

                return await _context.QueryAsync<FormModule>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los formularios.");
                throw;
            }
        }


        // Método para obtener FormModule ID SQL
        public async Task<FormModule?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT
                                    F.Id,    
                                    F.FormId, 
                                    F.ModuleId, 
                                    F.Active,
                                    M.Name AS ModuleName
                                FROM [FormModule] F
                                INNER JOIN [Module] M ON F.ModuleId = M.Id
                                WHERE F.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<FormModule>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formModule con el ID {FormModuleId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                string query = @"
                            INSERT INTO FormModule ( FormId, ModuleId, Active)
                            OUTPUT INSERTED.Id
                            VALUES (@FormId, @ModuleId, @Active );";

                formModule.Id = await _context.ExecuteScalarAsync<int>(query, new
                {
                    formModule.FormId,
                    formModule.ModuleId,
                    Active = true
                });

                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, " - Error al crear el FormModule.");
                throw;
            }
        }

        //Metodo para actualizar FormModule con SQL
        public async Task<bool> UpdateAsync(FormModule formModule)
        {
            try
            {
                string query = @"
                                UPDATE FormModule
                                SET 
                                   
                                    FormId = @FormId, 
                                    ModuleId = @ModuleId, 
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    formModule.Id,
                    formModule.FormId,
                    formModule.ModuleId,
                    formModule.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar FormModule logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE FormModule
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

        //Metodo para eliminar FormModule persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE FormModule
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar formModule: {ex.Message}");
                return false;
            }
        }

        // Consulta con LINQ
        public async Task<IEnumerable<FormModule>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<FormModule>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los FormModule con LINQ.");
                throw;
            }
        }


        //<summary> Obtiene un formModule especifíco por su identificador.

        // Consulta con LINQ
        public async Task<FormModule?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<FormModule>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formModule con el ID {FormModuleId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo formModule en la base de datos.
        //</summary>
        //<param name="rol">Instancia del formModule a crear.</param>
        //<returns>El formModule creado.</returns>

        public async Task<FormModule> CreateLinQAsync(FormModule formModule)
        {
            try
            {
                await _context.Set<FormModule>().AddAsync(formModule);
                await _context.SaveChangesAsync();
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el formModule {formModule}", formModule);
                throw;
            }
        }


        //<summary>
        //    Actualiza un formModule existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(FormModule formModule)
        {
            try
            {
                _context.Set<FormModule>().Update(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el FormModule {FormModule}", formModule);
                throw;
            }
        }

        //Metodo para eliminar FormModule logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var formModule = await GetByIdAsyncLinq(id);
                if (formModule == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                formModule.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el formModule con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un formModule de la base de datos.
        //</summary>
        //<param name="rol">Identificador único del formModule a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                    return false;

                _context.Set<FormModule>().Remove(formModule);
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
