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
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //<summary>
        // Obtiene todos los roles almacenados en la base de datos.
        // </summary>
        // <returns>Lista de roles.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Name, 
                                LastName,
                                NumberIdentification,
                                Phone,
                                Active
                            FROM Person
                            WHERE Active = 1";
                return await _context.QueryAsync<Person>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        // Método para obtener Rol ID SQL
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name, 
                                    LastName,
                                    NumberIdentification,
                                    Phone,
                                    Active
                                FROM Person 
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Person>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con el ID {RolId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                string query = @"
                            INSERT INTO Person (Name, LastName, NumberIdentification, Phone,Active) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @LastName, @NumberIdentification, @Phone, @Active)";

                person.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    person.Name,
                    person.LastName,
                    person.NumberIdentification,
                    person.Phone,
                    Active = true

                });

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol.");
                throw;
            }
        }

        //Metodo para actualizar Rol con SQL
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                string query = @"UPDATE Person 
                                SET 
                                    Name = @Name, 
                                    LastName = @LastName, 
                                    NumberIdentification = @NumberIdentification, 
                                    Phone = @Phone, 
                                    Active = @Active 
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    person.Id,
                    person.Name,
                    person.LastName,
                    person.NumberIdentification,
                    person.Phone,
                    person.Active
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
                string query = @"UPDATE Person
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
                               DELETE Person
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
        public async Task<IEnumerable<Person>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Person>()
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
        public async Task<Person?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
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

        public async Task<Person> CreateLinQAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol {Rol}", person);
                throw;
            }
        }
        //<summary>
        //    Actualiza un rol existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Person person)
        {
            try
            {
                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol {Rol}", person);
                throw;
            }
        }

        //Metodo para eliminar Rol logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var person = await GetByIdAsyncLinq(id);
                if (person == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                person.Active = false; // O rol.Active = 0; si es un campo numérico
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
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                    return false;

                _context.Set<Person>().Remove(person);
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
