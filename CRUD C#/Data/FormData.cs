using Entity.Context;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;

        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //consulta en la base de datos Sql
        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT 
                                Id, 
                                Name, 
                                Description, 
                                Active 
                            FROM Form 
                            WHERE Active = 1";
                return await _context.QueryAsync<Form>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form");
                throw;
            }
        }

            // Método para obtener Form ID SQL
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name,  
                                    Description, 
                                    Active 
                                FROM Form 
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Form>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener From con el ID {Formd}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }


        //Metodo para crear un Form SQL
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                string query = @"
                            INSERT INTO Form (Name,Description, Active) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Description, @Active)";

                form.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                
                    form.Name,
                    form.Description,
                    Active = true,
                });

                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el form.");
                throw;
            }
        }

        //Metodo para actualizar Form con SQL
        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                string query = @"UPDATE Form
                                SET 
                                    Name = @Name,
                                    Description = @Description, 
                                    Active = @Active 
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    form.Id,
                    form.Name,
                    form.Description,
                    form.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }


        //Metodo para eliminar Form logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Form
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente Form: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Form persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Form
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

        //<summary> Obtiene un rol especifíco por su identificador.



        // Consulta con LINQ
        public async Task<Form> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener form con el ID {FormId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo rol en la base de datos.
        //</summary>
        //<param name="rol">Instancia del rol a crear.</param>
        //<returns>El rol creado.</returns>

        public async Task<Form> CreateLinQAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el form {Form}", form);
                throw;
            }
        }

        //<summary>
        //    Actualiza un rol existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el form {Form}", form);
                throw;
            }
        }


        //Metodo para eliminar Rol logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var form = await GetByIdAsyncLinq(id);
                if (form == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                form.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el form con ID {id}", id);
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
                var form = await _context.Set<Form>().FindAsync(id);
                if (form == null)
                    return false;

                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el form: {ex.Message}");
                return false;
            }
        }
    }
}
