using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date
{
    public class InventaryDetailsData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventaryDetailsData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public InventaryDetailsData(ApplicationDbContext context, ILogger<InventaryDetailsData> logger)
        {
            _context = context;
            _logger = logger;
        }
        //<summary>
        // Obtiene todos los inventaryDetailses almacenados en la base de datos.
        // </summary>
        // <returns>Lista de inventaryDetailses.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<InventaryDetails>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT I.Id,    
                                        I.StatusPrevious, 
                                        I.StatusNew, 
                                        I.Observations, 
                                        I.Date,
                                        I.Description,
                                        I.Active,
                                        I.ZoneId,
                                        Z.Name AS ZoneName
                                FROM InventaryDates I
                                INNER JOIN Zone Z ON I.ZoneId = Z.Id
                                WHERE I.Active = 1";
                return await _context.QueryAsync<InventaryDetails>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los InventaryDetails ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        // Método para obtener InventaryDetails ID SQL
        public async Task<InventaryDetails?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT I.Id,    
                                        I.StatusPrevious, 
                                        I.StatusNew, 
                                        I.Observations, 
                                        I.[Date,
                                        I.Description,
                                        I.Active,
                                        I.ZoneId,
                                        Z.Name AS ZoneName
                                FROM InventaryDates I
                                INNER JOIN Zone Z ON I.ZoneId = Z.Id
                                WHERE U.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<InventaryDetails>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inventaryDetails con el ID {InventaryDetailsId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<InventaryDetails> CreateAsync(InventaryDetails inventaryDetails)
        {
            try
            {
                string query = @"
                            INSERT INTO InventaryDetails ( StatusPrevious, StatusNew, Observations, [Date],Description, Active, ZoneId)
                            OUTPUT INSERTED.Id
                            VALUES ( @StatusPrevious, @StatusNew, @Observations, @Date,@Description, @Active,@ZoneId);";

                inventaryDetails.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    inventaryDetails.StatusPrevious,
                    inventaryDetails.StatusNew,
                    inventaryDetails.Observations,
                    inventaryDetails.Date,
                    inventaryDetails.Description,
                    inventaryDetails.ZoneId,
                    Active = true,
                });

                return inventaryDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el InventaryDetails.");
                throw;
            }
        }

        //Metodo para actualizar InventaryDetails con SQL
        public async Task<bool> UpdateAsync(InventaryDetails inventaryDetails)
        {
            try
            {
                string query = @"
                                UPDATE InventaryDetails
                                SET 
                                    StatusPrevious = @StatusPrevious,
                                    StatusNew = @StatusNew,
                                    Observations = @Observations,
                                    Date = @Date,
                                    Description = @Description,
                                    ZoneId = @ZoneId,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    inventaryDetails.Id,
                    inventaryDetails.StatusPrevious,
                    inventaryDetails.StatusNew,
                    inventaryDetails.Observations,
                    inventaryDetails.Date,
                    inventaryDetails.Description,
                    inventaryDetails.ZoneId,
                    inventaryDetails.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el InventaryDetails {ex.Message}");
                return false;

            }
        }


        //Metodo para eliminar InventaryDetails logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE InventaryDetails
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente InventaryDetails: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar InventaryDetails persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE InventaryDetails
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar inventaryDetails: {ex.Message}");
                return false;
            }
        }


        // Consulta con LINQ
        public async Task<IEnumerable<InventaryDetails>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<InventaryDetails>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los InventaryDetails con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un inventaryDetails especifíco por su identificador.

        // Consulta con LINQ
        public async Task<InventaryDetails?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<InventaryDetails>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inventaryDetails con el ID {InventaryDetailsId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo inventaryDetails en la base de datos.
        //</summary>
        //<param name="inventaryDetails">Instancia del inventaryDetails a crear.</param>
        //<returns>El inventaryDetails creado.</returns>

        public async Task<InventaryDetails> CreateLinQAsync(InventaryDetails inventaryDetails)
        {
            try
            {
                await _context.Set<InventaryDetails>().AddAsync(inventaryDetails);
                await _context.SaveChangesAsync();
                return inventaryDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el inventaryDetails {inventaryDetails}", inventaryDetails);
                throw;
            }
        }

        //<summary>
        //    Actualiza un inventaryDetails existente en la base de datos.
        //</summary>
        ///<param name="inventaryDetails">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(InventaryDetails inventaryDetails)
        {
            try
            {
                _context.Set<InventaryDetails>().Update(inventaryDetails);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el InventaryDetails {InventaryDetails}", inventaryDetails);
                throw;
            }
        }

        //Metodo para eliminar InventaryDetails logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var inventaryDetails = await GetByIdAsyncLinq(id);
                if (inventaryDetails == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                inventaryDetails.Active = false; // O inventaryDetails.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el inventaryDetails con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un inventaryDetails de la base de datos.
        //</summary>
        //<param name="inventaryDetails">Identificador único del inventaryDetails a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var inventaryDetails = await _context.Set<InventaryDetails>().FindAsync(id);
                if (inventaryDetails == null)
                    return false;

                _context.Set<InventaryDetails>().Remove(inventaryDetails);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el inventaryDetails: {ex.Message}");
                return false;
            }
        }
    }
}
