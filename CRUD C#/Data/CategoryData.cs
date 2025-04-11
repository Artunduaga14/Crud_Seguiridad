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
    public class CategoryData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public CategoryData(ApplicationDbContext context, ILogger<CategoryData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //<summary>
        // Obtiene todos los categoryes almacenados en la base de datos.
        // </summary>
        // <returns>Lista de categoryes.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Name,
                                Description,
                                Active
                            FROM Category
                            WHERE Active = 1";
                return await _context.QueryAsync<Category>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los Category ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        // Método para obtener Category ID SQL
        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name,
                                    Description,
                                    Active
                                FROM Category
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Category>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Category con el ID {CompabyId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }




        //Metodo para crear un Category SQL
        public async Task<Category> CreateAsync(Category category)
        {
            try
            {
                string query = @"
                            INSERT INTO Category (Name,Description,Active) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name,@Description,@Active)";

                category.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    category.Name,
                    category.Description,
                    Active = true,
                });

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el category.");
                throw;
            }
        }



        //Metodo para actualizar Category con SQL
        public async Task<bool> UpdateAsync(Category category)
        {
            try
            {
                string query = @"UPDATE Category 
                                SET 
                                    Name = @Name,
                                    Description = @Description,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    category.Id,
                    category.Name,
                    category.Description,
                    category.Active,
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el category: {ex.Message}");
                return false;
            }
        }


        //Metodo para eliminar Category logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Category
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

        //Metodo para eliminar Category persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Category
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar category: {ex.Message}");
                return false;
            }
        }

        // Consulta con LINQ
        public async Task<IEnumerable<Category>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Category>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los categoryes con LINQ.");
                throw;
            }
        }


        //<summary> Obtiene un category especifíco por su identificador.

        // Consulta con LINQ
        public async Task<Category?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Category>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener category con el ID {CategoryId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }


        //<summary>
        //    Crea un nuevo category en la base de datos.
        //</summary>
        //<param name="category">Instancia del category a crear.</param>
        //<returns>El category creado.</returns>

        public async Task<Category> CreateLinQAsync(Category category)
        {
            try
            {
                await _context.Set<Category>().AddAsync(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el category {Category}", category);
                throw;
            }
        }

      



        //<summary>
        //    Actualiza un category existente en la base de datos.
        //</summary>
        ///<param name="category">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Category category)
        {
            try
            {
                _context.Set<Category>().Update(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el category {Category}", category);
                throw;
            }
        }


        //Metodo para eliminar Category logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var category = await GetByIdAsyncLinq(id);
                if (category == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                category.Active = false; // O category.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el category con ID {id}", id);
                throw;
            }
        }

        //<summary>
        //    Elimina un category de la base de datos.
        //</summary>
        //<param name="category">Identificador único del category a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var category = await _context.Set<Category>().FindAsync(id);
                if (category == null)
                    return false;

                _context.Set<Category>().Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el category: {ex.Message}");
                return false;
            }
        }
    }
}
