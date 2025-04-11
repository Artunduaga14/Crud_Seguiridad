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
    public class ItemData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ItemData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public ItemData(ApplicationDbContext context, ILogger<ItemData> logger)
        {
            _context = context;
            _logger = logger;
        }

       

        // Obtiene todos los items almacenados en la base de datos.
        // </summary>
        // <returns>Lista de items.</returns>

        // Consulta estructurada con SQL
        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT I.Id,    
                                I.Code, 
                                I.CodeQr, 
                                I.Name, 
                                I.Description,
                                I.Active,
                                I.CreatedAt,
                                I.CategoryId,
                                I.ZoneId,
                                C.Name AS CategoryName
                         FROM Item I
                         INNER JOIN Category C ON I.CategoryId = C.Id
                         INNER JOIN Zone Z ON I.ZoneId = Z.Id
                         WHERE I.Active = 1";

                return await _context.QueryAsync<Item>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los Items.");
                throw; // Relanza la excepción para que sea manejada por las capas superiores
            }
        }


        // Método para obtener Item ID SQL
        ///<summary>
        // Obtiene un item por su ID.
        // </summary>
        // <param name="id">ID del item.</param>
        // <returns>Item correspondiente al ID, o null si no se encuentra.</returns>
        public async Task<Item?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT I.Id,    
                                I.Code, 
                                I.CodeQr, 
                                I.Name, 
                                I.Description,
                                I.Active,
                                I.CreatedAt,
                                I.CategoryId,
                                I.ZoneId,
                                C.Name AS CategoryName
                         FROM Item I
                         INNER JOIN Category C ON I.CategoryId = C.Id
                         INNER JOIN Zone Z ON I.ZoneId = Z.Id
                         WHERE I.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Item>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Item con el ID {ItemId}", id);
                throw; // Relanza la excepción para que sea manejada por las capas superiores
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<Item> CreateAsync(Item item)
        {
            try
            {
                string query = @"
                            INSERT INTO Item ( Code, CodeQr, Name, Description, Active, CreatedAt, CategoryId, ZoneId)
                            OUTPUT INSERTED.Id
                            VALUES ( @Code, @CodeQr, @Name, @Description, @Active, @CreatedAt, @CategoryId, @ZoneId);";
                
                item.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    item.Code,
                    item.CodeQr,
                    item.Name,
                    item.Description,
                    item.CreatedAt,
                    item.CategoryId,
                    item.ZoneId,
                    Active = true,
                });

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el Item.");
                throw;
            }
        }

        //Metodo para actualizar Item con SQL
        public async Task<bool> UpdateAsync(Item item)
        {
            try
            {
                string query = @"
                                UPDATE Item
                                SET 
                                    Code = @Code,
                                    CodeQr = @CodeQr,
                                    Name = @Name,
                                    Description = @Description,
                                    CreatedAt = @CreatedAt,
                                    CategoryId = @CategoryId,
                                    ZoneId = @ZoneId,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    item.Id,
                    item.Code,
                    item.CodeQr,
                    item.Name,
                    item.Description,
                    item.CreatedAt,
                    item.CategoryId,
                    item.ZoneId,
                    item.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el item: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Item logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Item
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente item: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Item persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Item
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar item: {ex.Message}");
                return false;
            }
        }


        // Consulta con LINQ
        public async Task<IEnumerable<Item>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Item>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los Item con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un item especifíco por su identificador.

        // Consulta con LINQ
        public async Task<Item?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Item>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener item con el ID {ItemId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo item en la base de datos.
        //</summary>
        //<param name="item">Instancia del item a crear.</param>
        //<returns>El item creado.</returns>

        public async Task<Item> CreateLinQAsync(Item item)
        {
            try
            {
                await _context.Set<Item>().AddAsync(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el item {item}", item);
                throw;
            }
        }

        //<summary>
        //    Actualiza un item existente en la base de datos.
        //</summary>
        ///<param name="item">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Item item)
        {
            try
            {
                _context.Set<Item>().Update(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Item {Item}", item);
                throw;
            }
        }

        //Metodo para eliminar Item logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var item = await GetByIdAsyncLinq(id);
                if (item == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                item.Active = false; // O item.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el item con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un item de la base de datos.
        //</summary>
        //<param name="item">Identificador único del item a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var item = await _context.Set<Item>().FindAsync(id);
                if (item == null)
                    return false;

                _context.Set<Item>().Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el item: {ex.Message}");
                return false;
            }
        }

    }
}
