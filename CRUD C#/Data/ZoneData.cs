

using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ZoneData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ZoneData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public ZoneData(ApplicationDbContext context, ILogger<ZoneData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las zonas activas junto con la información de su sucursal.
        /// </summary>
        /// <returns>Lista de zonas con nombre de la sucursal.</returns>
        public async Task<IEnumerable<Zone>> GetAllAsync()
        {
            try
            {
                string query = @"
                                SELECT Z.Id,
                                       Z.BranchId,
                                       Z.Active,
                                       B.Name AS BranchName
                                FROM Zone Z
                                INNER JOIN Branch B ON Z.BranchId = B.Id
                                WHERE Z.Active = 1";

                return await _context.QueryAsync<Zone>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las zonas.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una zona específica por su ID, incluyendo el nombre de la sucursal.
        /// </summary>
        /// <param name="id">ID de la zona.</param>
        /// <returns>Zona con información de sucursal.</returns>
        public async Task<Zone?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"
                                SELECT Z.Id,
                                       Z.BranchId,
                                       Z.Active,
                                       B.Name AS BranchName
                                FROM Zone Z
                                INNER JOIN Branch B ON Z.BranchId = B.Id
                                WHERE Z.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Zone>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la zona con ID {ZoneId}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva zona en la base de datos.
        /// </summary>
        /// <param name="zone">Objeto Zone a insertar.</param>
        /// <returns>Zona creada con su ID asignado.</returns>
        public async Task<Zone> CreateAsync(Zone zone)
        {
            try
            {
                string query = @"
                                INSERT INTO Zone (BranchId, Active)
                                OUTPUT INSERTED.Id
                                VALUES (@BranchId, @Active);";

                zone.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    zone.BranchId,
                    zone.Active
                });

                return zone;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la zona.");
                throw;
            }
        }


        /// <summary>
        /// Actualiza una zona existente en la base de datos.
        /// </summary>
        /// <param name="zone">Zona con los nuevos datos.</param>
        /// <returns>True si se actualizó al menos una fila; de lo contrario, false.</returns>
        public async Task<bool> UpdateAsync(Zone zone)
        {
            try
            {
                string query = @"
                                UPDATE Zone
                                SET 
                                    BranchId = @BranchId,
                                    Active = @Active
                                WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    zone.Id,
                    zone.BranchId,
                    zone.Active
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la zona con ID {ZoneId}", zone.Id);
                return false;
            }
        }




















        //Metodo para eliminar Zone logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Zone
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente zone: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Zone persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Zone
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar zone: {ex.Message}");
                return false;
            }
        }


        // Consulta con LINQ
        public async Task<IEnumerable<Zone>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Zone>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los Zone con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un zone especifíco por su identificador.

        // Consulta con LINQ
        public async Task<Zone?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Zone>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener zone con el ID {ZoneId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo zone en la base de datos.
        //</summary>
        //<param name="zone">Instancia del zone a crear.</param>
        //<returns>El zone creado.</returns>

        public async Task<Zone> CreateLinQAsync(Zone zone)
        {
            try
            {
                await _context.Set<Zone>().AddAsync(zone);
                await _context.SaveChangesAsync();
                return zone;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el zone {zone}", zone);
                throw;
            }
        }

        //<summary>
        //    Actualiza un zone existente en la base de datos.
        //</summary>
        ///<param name="zone">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Zone zone)
        {
            try
            {
                _context.Set<Zone>().Update(zone);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Zone {Zone}", zone);
                throw;
            }
        }

        //Metodo para eliminar Zone logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var zone = await GetByIdAsyncLinq(id);
                if (zone == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                zone.Active = false; // O zone.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el zone con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un zone de la base de datos.
        //</summary>
        //<param name="zone">Identificador único del zone a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var zone = await _context.Set<Zone>().FindAsync(id);
                if (zone == null)
                    return false;

                _context.Set<Zone>().Remove(zone);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el zone: {ex.Message}");
                return false;
            }
        }
    }
}

