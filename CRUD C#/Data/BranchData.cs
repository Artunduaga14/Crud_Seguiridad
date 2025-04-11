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
    public class BranchData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BranchData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public BranchData(ApplicationDbContext context, ILogger<BranchData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //<summary>
        // Obtiene todos los roles almacenados en la base de datos.
        // </summary>
        // <returns>Lista de roles.</returns>

        //Consulta estructurada con SQL
        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            try
            {
                string Query = @"SELECT B.Id,    
                                        B.Name, 
                                        B.Address, 
                                        B.Phone,
                                        B.Email,
                                        B.Incharge,
                                        B.Active,
                                        B.LocationFurrow, 
                                        B.CompanyId,
                                        C.Name AS CompanyName
                                FROM Branch B
                                INNER JOIN Company C ON B.CompanyId = C.Id
                                WHERE B.Active = 1";
                return await _context.QueryAsync<Branch>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los Branch ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

         // Método para obtener Branch ID SQL
        public async Task<Branch?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT B.Id,    
                                        B.Name, 
                                        B.Address, 
                                        B.Phone,
                                        B.Email,
                                        B.Incharge,
                                        B.Active,
                                        B.LocationFurrow, 
                                        B.CompanyId,
                                        C.Name AS CompanyName
                                FROM Branch B
                                INNER JOIN Company C ON B.CompanyId = C.Id
                                WHERE B.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<Branch>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Branch con el ID {BranchId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }
        //Metodo para crear un ROL SQL
        public async Task<Branch> CreateAsync(Branch branch)
        {
            try
            {
                string query = @"
                            INSERT INTO Branch ( Name, Address, Phone, Email, Incharge, Active, LocationFurrow, CompanyId )
                            OUTPUT INSERTED.Id
                            VALUES (  @Name, @Address, @Phone, @Email, @Incharge, @Active, @LocationFurrow, @CompanyId );";

                branch.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    branch.Name,
                    branch.Address,
                    branch.Phone,
                    branch.Email,
                    branch.Incharge,
                    branch.LocationFurrow,
                    branch.CompanyId,
                    Active = true,
                });

                return branch;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el Branch.");
                throw;
            }
        }

        //Metodo para actualizar Branch con SQL
        public async Task<bool> UpdateAsync(Branch branch)
        {
            try
            {
                string query = @"
                                UPDATE Branch
                                SET 
                                   
                                    Name = @Name,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email,
                                    Incharge = @Incharge,
                                    LocationFurrow = @LocationFurrow,
                                    CompanyId = @CompanyId,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    branch.Id,
                    branch.Name,
                    branch.Address,
                    branch.Phone,
                    branch.Email,
                    branch.Incharge,
                    branch.LocationFurrow ,
                    branch.CompanyId ,
                    branch.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar Branch logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Branch
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

        //Metodo para eliminar Branch persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE Branch
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
        public async Task<IEnumerable<Branch>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<Branch>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los Branch con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un rol especifíco por su identificador.

        // Consulta con LINQ
        public async Task<Branch?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<Branch>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con el ID {BranchId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo rol en la base de datos.
        //</summary>
        //<param name="rol">Instancia del rol a crear.</param>
        //<returns>El rol creado.</returns>

        public async Task<Branch> CreateLinQAsync(Branch branch)
        {
            try
            {
                await _context.Set<Branch>().AddAsync(branch);
                await _context.SaveChangesAsync();
                return branch;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol {Branch}", branch);
                throw;
            }
        }

        //<summary>
        //    Actualiza un rol existente en la base de datos.
        //</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(Branch branch)
        {
            try
            {
                _context.Set<Branch>().Update(branch);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Branch {Branch}", branch);
                throw;
            }
        }

        //Metodo para eliminar User logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var branch = await GetByIdAsyncLinq(id);
                if (branch == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                branch.Active = false; // O rol.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el branch con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un branch de la base de datos.
        //</summary>
        //<param name="rol">Identificador único del branch a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var branch = await _context.Set<User>().FindAsync(id);
                if (branch == null)
                    return false;

                _context.Set<User>().Remove(branch);
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
