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
    public class ImagenItemData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImagenItemData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public ImagenItemData(ApplicationDbContext context, ILogger<ImagenItemData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        /// Obtiene todas las imágenes de ítems activas almacenadas en la base de datos.
        ///</summary>
        ///<returns>Lista de imágenes de ítems.</returns>
        public async Task<IEnumerable<ImagenItem>> GetAllAsync()
        {
            try
            {
                string query = @"
                                SELECT II.Id, 
                                       II.ItemId, 
                                       II.UrlImage, 
                                       II.DateRegistry, 
                                       II.Active,
                                       I.Name AS ItemName
                                FROM ImagenItem II
                                INNER JOIN Item I ON II.ItemId = I.Id
                                WHERE II.Active = 1";

                return await _context.QueryAsync<ImagenItem>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las imágenes de los ítems.");
                throw;
            }
        }

        // Método para obtener ImagenItem ID SQL
        public async Task<ImagenItem?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT II.Id, 
                                       II.ItemId, 
                                       II.UrlImage, 
                                       II.DateRegistry, 
                                       II.Active,
                                       I.Name AS ItemName
                                FROM ImagenItem II
                                INNER JOIN Item I ON II.ItemId = I.Id
                                WHERE II.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<ImagenItem>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener imagenItem con el ID {ImagenItemId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }


        //Metodo para crear un ImagenItem SQL
        public async Task<ImagenItem> CreateAsync(ImagenItem imagenItem)
        {
            try
            {
                string query = @"
            INSERT INTO ImagenItem (ItemId, UrlImage, DateRegistry, Active)
            OUTPUT INSERTED.Id
            VALUES (@ItemId, @UrlImage, @DateRegistry, @Active);";

                imagenItem.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    imagenItem.ItemId,
                    imagenItem.UrlImage,
                    DateRegistry = DateTime.Now,
                    Active = true
                });

                return imagenItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el ImagenItem.");
                throw;
            }
        }

        //Metodo para actualizar ImagenItem con SQL
        public async Task<bool> UpdateAsync(ImagenItem imagenItem)
        {
            try
            {
                string query = @"
                                UPDATE [ImagenItem]
                                SET 
                                     ItemId = @ItemId,
                                     UrlImage = @UrlImage,
                                     DateRegistry = @DateRegistry,
                                     Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    imagenItem.Id,
                    imagenItem.ItemId,
                    imagenItem.UrlImage,
                    imagenItem.DateRegistry,
                    imagenItem.Active
                });

                return rowsAffected > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el ImagenItem: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar ImagenItem logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE ImagenItem
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente ImagenItem: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar ImagenItem persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE ImagenItem
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
        public async Task<IEnumerable<ImagenItem>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<ImagenItem>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los ImagenItem con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un imagenItem especifíco por su identificador.

        // Consulta con LINQ
        public async Task<ImagenItem?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<ImagenItem>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener imagenItem con el ID {ImagenItemId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo imagenItem en la base de datos.
        //</summary>
        //<param name="rol">Instancia del imagenItem a crear.</param>
        //<returns>El imagenItem creado.</returns>

        public async Task<ImagenItem> CreateLinQAsync(ImagenItem imagenItem)
        {
            try
            {
                await _context.Set<ImagenItem>().AddAsync(imagenItem);
                await _context.SaveChangesAsync();
                return imagenItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el imagenItem {imagenItem}", imagenItem);
                throw;
            }
        }

        //<summary>
        //    Actualiza un imagenItem existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(ImagenItem imagenItem)
        {
            try
            {
                _context.Set<ImagenItem>().Update(imagenItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el ImagenItem {ImagenItem}", imagenItem);
                throw;
            }
        }

        //Metodo para eliminar ImagenItem logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var imagenItem = await GetByIdAsyncLinq(id);
                if (imagenItem == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                imagenItem.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el imagenItem con ID {id}", id);
                throw;
            }
        }

        //<summary>
        //    Elimina un imagenItem de la base de datos.
        //</summary>
        //<param name="rol">Identificador único del imagenItem a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var imagenItem = await _context.Set<ImagenItem>().FindAsync(id);
                if (imagenItem == null)
                    return false;

                _context.Set<ImagenItem>().Remove(imagenItem);
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
