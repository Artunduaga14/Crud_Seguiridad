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
   public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }


        //<summary>
        // Obtiene todos los roles almacenados en la base de datos.
        // </summary>
        // <returns>Lista de roles.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT U.Id,    
                                        U.Username, 
                                        U.Password, 
                                        U.CreationDate, 
                                        U.PersonId,
                                        U.Active,
                                         
                                        P.Name AS PersonName
                                FROM [User] U
                                INNER JOIN Person P ON U.PersonId = P.Id
                                WHERE U.Active = 1";
                return await _context.QueryAsync<User>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los User ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        // Método para obtener User ID SQL
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT U.Id, 
                                        U.Username, 
                                        U.Password, 
                                        U.CreationDate, 
                                        U.PersonId,
                                        U.Active,
                                        P.Id AS PersonId, 
                                        P.Name AS PersonName
                                FROM [User] u
                                INNER JOIN Person P ON U.PersonId = P.Id
                                WHERE U.Id = @Id";
                
                return await _context.QueryFirstOrDefaultAsync<User>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener user con el ID {UserId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                string query = @"
                            INSERT INTO [User] ( Username, Password, CreationDate, PersonId, Active)
                            OUTPUT INSERTED.Id
                            VALUES ( @Username, @Password, @CreationDate, @PersonId, @Active);";
                
                user.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    user.Username,
                    user.Password,
                    user.PersonId,
                    CreationDate = DateTime.Now,
                    Active = true,
                });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el User.");
                throw;
            }
        }



        //Metodo para actualizar User con SQL
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                string query = @"
                                UPDATE [User]
                                SET 
                                    Username = @Username,
                                    Password = @Password,
                                    CreationDate = @CreationDate,
                                    PersonId = @PersonId,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    user.Id,
                    user.Username,
                    user.Password,
                    user.CreationDate,
                    user.PersonId,
                    user.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }




        //Metodo para eliminar User logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE [User]
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

        //Metodo para eliminar User persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE [User]
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
        public async Task<IEnumerable<User>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<User>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los User con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un user especifíco por su identificador.

        // Consulta con LINQ
        public async Task<User?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener user con el ID {UserId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo user en la base de datos.
        //</summary>
        //<param name="rol">Instancia del user a crear.</param>
        //<returns>El user creado.</returns>

        public async Task<User> CreateLinQAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el user {user}", user);
                throw;
            }
        }

        //<summary>
        //    Actualiza un user existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el User {User}", user);
                throw;
            }
        }

        //Metodo para eliminar User logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var user = await GetByIdAsyncLinq(id);
                if (user == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                user.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el user con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un user de la base de datos.
        //</summary>
        //<param name="rol">Identificador único del user a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                _context.Set<User>().Remove(user);
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
