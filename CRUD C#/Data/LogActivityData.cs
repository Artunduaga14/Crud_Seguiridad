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
    public class LogActivityData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogActivityData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public LogActivityData(ApplicationDbContext context, ILogger<LogActivityData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //<summary>
        // Obtiene todos los logActivityes almacenados en la base de datos.
        // </summary>
        // <returns>Lista de logActivityes.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<LogActivity>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Action, 
                                DataPrevious, 
                                DataNew, 
                                Data 
                            FROM LogActivity
                            WHERE Active = 1";
                return await _context.QueryAsync<LogActivity>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        // Método para obtener LogActivity ID SQL
        public async Task<LogActivity?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Action, 
                                    DataPrevious, 
                                    DataNew, 
                                    Data, 
                                    Active
                                FROM LogActivity
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<LogActivity>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logActivity con el ID {LogActivityId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<LogActivity> CreateAsync(LogActivity logActivity)
        {
            try
            {
                string query = @"
                            INSERT INTO LogActivity (Action,Active, DataPrevious, DataNew, Data) 
                            OUTPUT INSERTED.Id
                            VALUES (@Action,@Active, @DataPrevious, @DataNew, @Data)";

                logActivity.Id = await _context.ExecuteScalarAsync<int>(query, new
                {
                    logActivity.Action,
                    logActivity.DataPrevious,
                    logActivity.DataNew,
                    Data = DateTime.Now,
                    Active = true,
                });

                return logActivity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el logActivity.");
                throw;
            }
        }


        //Metodo para actualizar LogActivity con SQL
        public async Task<bool> UpdateAsync(LogActivity logActivity)
        {
            try
            {
                string query = @"
                                UPDATE LogActivity
                                SET 
                                    Action = @Action,
                                    DataPrevious = @DataPrevious,
                                    DataNew = @DataNew,
                                    Data = @Data,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    logActivity.Id,
                    logActivity.DataPrevious,
                    logActivity.DataNew,
                    logActivity.Data,
                    logActivity.Active,
                    logActivity.Action
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el logActivity: {ex.Message}");
                return false;
            }
        }




        //Metodo para eliminar LogActivity logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE LogActivity
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente logActivity: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar LogActivity persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE LogActivity
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logActivity: {ex.Message}");
                return false;
            }
        }

        // Consulta con LINQ
        public async Task<IEnumerable<LogActivity>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<LogActivity>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los logActivityes con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un logActivity especifíco por su identificador.



        // Consulta con LINQ
        public async Task<LogActivity?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<LogActivity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener logActivity con el ID {LogActivityId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }


        //<summary>
        //    Crea un nuevo logActivity en la base de datos.
        //</summary>
        //<param name="logActivity">Instancia del logActivity a crear.</param>
        //<returns>El logActivity creado.</returns>

        public async Task<LogActivity> CreateLinQAsync(LogActivity logActivity)
        {
            try
            {
                await _context.Set<LogActivity>().AddAsync(logActivity);
                await _context.SaveChangesAsync();
                return logActivity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el logActivity {LogActivity}", logActivity);
                throw;
            }
        }

        //<summary>
        //    Actualiza un logActivity existente en la base de datos.
        //</summary>
        ///<param name="logActivity">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(LogActivity logActivity)
        {
            try
            {
                _context.Set<LogActivity>().Update(logActivity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el logActivity {LogActivity}", logActivity);
                throw;
            }
        }


        //Metodo para eliminar LogActivity logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var logActivity = await GetByIdAsyncLinq(id);
                if (logActivity == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                logActivity.Active = false; // O logActivity.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el logActivity con ID {id}", id);
                throw;
            }
        }

        //<summary>
        //    Elimina un logActivity de la base de datos.
        //</summary>
        //<param name="logActivity">Identificador único del logActivity a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var logActivity = await _context.Set<LogActivity>().FindAsync(id);
                if (logActivity == null)
                    return false;

                _context.Set<LogActivity>().Remove(logActivity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el logActivity: {ex.Message}");
                return false;
            }
        }
    }
}
