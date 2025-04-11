using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CompanyData
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public CompanyData(ApplicationDbContext context, ILogger<CompanyData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //<summary>
        // Obtiene todos los companyes almacenados en la base de datos.
        // </summary>
        // <returns>Lista de companyes.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT 
                                Id, 
                                Name,
                                Address, 
                                Phone, 
                                Email,
                                Logo,
                                Active,
                                DataRegistry
                            FROM Company
                            WHERE Active = 1";
                return await _context.QueryAsync<Company>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los Company ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        // Método para obtener Company ID SQL
        public async Task<Company?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id, 
                                    Name,
                                    Address, 
                                    Phone, 
                                    Email,
                                    Logo,
                                    Active,
                                    DataRegistry
                                FROM Company
                                WHERE Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Company>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Company con el ID {CompabyId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //Metodo para crear un Company SQL
        public async Task<Company> CreateAsync(Company company)
        {
            try
            {
                string query = @"
                            INSERT INTO Company (Name,Address,Phone,Email,Logo,Active,DataRegistry) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name,@Address,@Phone,@Email,@Logo,@Active,@DataRegistry)";

                company.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    company.Name,
                    company.Address,
                    company.Phone,
                    company.Email,
                    company.Logo,
                    DataRegistry = DateTime.Now,
                    Active = true,
                });

                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el company.");
                throw;
            }
        }

        //Metodo para actualizar Company con SQL
        public async Task<bool> UpdateAsync(Company company)
        {
            try
            {
                string query = @"UPDATE Company 
                                SET 
                                    Name = @Name,
                                    Address = @Address, 
                                    Phone = @Phone, 
                                    Email = @Email,
                                    Logo = @Logo,
                                    Active = @Active,
                                    DataRegistry = @DataRegistry
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    company.Id,
                    company.Name,
                    company.Address,
                    company.Phone,
                    company.Email,
                    company.Logo,
                    company.DataRegistry,
                    company.Active,
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el company: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Company logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Company
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente company: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Company persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Company
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar company: {ex.Message}");
                return false;
            }
        }








        // Consulta con LINQ
        public async Task<IEnumerable<Company>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Company>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los companyes con LINQ.");
                throw;
            }
        }


        //<summary> Obtiene un company especifíco por su identificador.



        // Consulta con LINQ
        public async Task<Company?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Company>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener company con el ID {CompanyId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo company en la base de datos.
        //</summary>
        //<param name="company">Instancia del company a crear.</param>
        //<returns>El company creado.</returns>

        public async Task<Company> CreateLinQAsync(Company company)
        {
            try
            {
                await _context.Set<Company>().AddAsync(company);
                await _context.SaveChangesAsync();
                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el company {Company}", company);
                throw;
            }
        }




        //<summary>
        //    Actualiza un company existente en la base de datos.
        //</summary>
        ///<param name="company">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Company company)
        {
            try
            {
                _context.Set<Company>().Update(company);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el company {Company}", company);
                throw;
            }
        }


        //Metodo para eliminar Company logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var company = await GetByIdAsyncLinq(id);
                if (company == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                company.Active = false; // O company.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el company con ID {id}", id);
                throw;
            }
        }

        //<summary>
        //    Elimina un company de la base de datos.
        //</summary>
        //<param name="company">Identificador único del company a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var company = await _context.Set<Company>().FindAsync(id);
                if (company == null)
                    return false;

                _context.Set<Company>().Remove(company);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el company: {ex.Message}");
                return false;
            }
        }
    }
}
