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
    public class RolFormPermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolFormPermissionData> _logger;

        //<summary>
        // Constructor que recibe el contexto de la base de datos.
        //</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext">

        public RolFormPermissionData(ApplicationDbContext context, ILogger<RolFormPermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        /// Obtiene todos los permisos de formulario por rol almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de permisos por rol.</returns>
        public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            try
            {
                string query = @"
            SELECT RFP.Id,
                   RFP.PermissionId,
                   RFP.RolId,
                   RFP.FormId,
                   RFP.Active,
                   P.Name AS PermissionName,
                   R.Name AS RolName,
                   F.Name AS FormName
            FROM RolFormPermission RFP
            INNER JOIN Permission P ON RFP.PermissionId = P.Id
            INNER JOIN Rol R ON RFP.RolId = R.Id
            INNER JOIN Form F ON RFP.FormId = F.Id
            WHERE RFP.Active = 1";

                return await _context.QueryAsync<RolFormPermission>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos por rol.");
                throw;
            }
        }

        ///<summary>
        /// Obtiene un permiso de formulario por rol específico por su ID.
        ///</summary>
        ///<param name="id">ID del RolFormPermission.</param>
        ///<returns>Objeto RolFormPermission correspondiente al ID, o null si no se encuentra.</returns>
        public async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"
            SELECT RFP.Id,
                   RFP.PermissionId,
                   RFP.RolId,
                   RFP.FormId,
                   RFP.Active,
                   P.Name AS PermissionName,
                   R.Name AS RolName,
                   F.Name AS FormName
            FROM RolFormPermission RFP
            INNER JOIN Permission P ON RFP.PermissionId = P.Id
            INNER JOIN Rol R ON RFP.RolId = R.Id
            INNER JOIN Form F ON RFP.FormId = F.Id
            WHERE RFP.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<RolFormPermission>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolFormPermission con ID {RolFormPermissionId}", id);
                throw;
            }
        }

        //Metodo para crear un ROL SQL
        public async Task<RolFormPermission> CreateAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                string query = @"
                            INSERT INTO RolFormPermission ( PermissionId, RolId, FormId, Active)
                            OUTPUT INSERTED.Id
                            VALUES ( @PermissionId, @RolId, @FormId, @Active);";

                rolFormPermission.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    rolFormPermission.PermissionId,
                    rolFormPermission.RolId,
                    rolFormPermission.FormId,
                    Active = true,
                });

                return rolFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el RolFormPermission.");
                throw;
            }
        }


        //Metodo para actualizar RolFormPermission con SQL
        public async Task<bool> UpdateAsync(RolFormPermission RolFormPermission)
        {
            try
            {
                string query = @"
                                UPDATE [RolFormPermission]
                                SET 
                                    RolId = @RolId,
                                    PermissionId = @PermissionId,
                                    FormId = @FormId,
                                    Active = @Active
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    RolFormPermission.Id,
                    RolFormPermission.RolId,
                    RolFormPermission.PermissionId,
                    RolFormPermission.FormId,
                    RolFormPermission.Active
                });

                return rowsAffected > 0;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }
















        //Metodo para eliminar RolFormPermission logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE RolFormPermission
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente rolFormPermission: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar RolFormPermission persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE RolFormPermission
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar rolFormPermission: {ex.Message}");
                return false;
            }
        }


        // Consulta con LINQ
        public async Task<IEnumerable<RolFormPermission>> GetAllAsyncLinq()
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                                .Where(fn => fn.Active)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los RolFormPermission con LINQ.");
                throw;
            }
        }

        //<summary> Obtiene un rolFormPermission especifíco por su identificador.

        // Consulta con LINQ
        public async Task<RolFormPermission?> GetByIdAsyncLinq(int id)
        {
            try
            {
                return await _context.Set<RolFormPermission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rolFormPermission con el ID {RolFormPermissionId}", id);
                throw; //Re-lanzamos la excepción para que sea manejada en capas superiores.
            }
        }

        //<summary>
        //    Crea un nuevo rolFormPermission en la base de datos.
        //</summary>
        //<param name="rolFormPermission">Instancia del rolFormPermission a crear.</param>
        //<returns>El rolFormPermission creado.</returns>

        public async Task<RolFormPermission> CreateLinQAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                await _context.Set<RolFormPermission>().AddAsync(rolFormPermission);
                await _context.SaveChangesAsync();
                return rolFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rolFormPermission {rolFormPermission}", rolFormPermission);
                throw;
            }
        }

        //<summary>
        //    Actualiza un rolFormPermission existente en la base de datos.
        //</summary>
        ///<param name="rolFormPermission">Objeto con la información actualizada.</param>
        // <returns> True si la operación fue exitosa, False en caso contrario. </returns>

        public async Task<bool> UpdateLinQAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                _context.Set<RolFormPermission>().Update(rolFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolFormPermission {RolFormPermission}", rolFormPermission);
                throw;
            }
        }

        //Metodo para eliminar RolFormPermission logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var rolFormPermission = await GetByIdAsyncLinq(id);
                if (rolFormPermission == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                rolFormPermission.Active = false; // O rolFormPermission.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el rolFormPermission con ID {id}", id);
                throw;
            }
        }


        //<summary>
        //    Elimina un rolFormPermission de la base de datos.
        //</summary>
        //<param name="rolFormPermission">Identificador único del rolFormPermission a eliminar.</param>
        //<returns>True si la eliminación fue exitosa, False en caso contrario.</returns>

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolFormPermission = await _context.Set<RolFormPermission>().FindAsync(id);
                if (rolFormPermission == null)
                    return false;

                _context.Set<RolFormPermission>().Remove(rolFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rolFormPermission: {ex.Message}");
                return false;
            }
        }
    }
}