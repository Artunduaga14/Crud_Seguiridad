

using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<PermissionBusiness> _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<PermissionDto>> GetAllPermissionAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var permissions = await _permissionData.GetAllAsync();
                var permissionDTO = MapToDTOList(permissions);

                return permissionDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permission");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permission", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<PermissionDto> GetPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permission con ID inválido: {PermissinId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del permission debe ser mayor que cero");
            }

            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {Permissiond}", id);
                    throw new EntityNotFoundException("Permission", id);
                }

                return MapToDTO(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto PermissionDto)
        {
            try
            {
                ValidatePermission(PermissionDto);

                var permission = MapToEntity(PermissionDto);

                var permissionCreado = await _permissionData.CreateAsync(permission);

                return MapToDTO(permissionCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", PermissionDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }



        // Método para actualizar un rol desde un DTO
        public async Task<bool> UpdatePermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                ValidatePermission(permissionDto);

                var existingPermission = await _permissionData.GetByIdAsync(permissionDto.Id);
                if (existingPermission == null)
                {
                    throw new EntityNotFoundException("Rol", permissionDto.Id);
                }

                // Actualizar propiedades
                existingPermission.Id = permissionDto.Id;
                existingPermission.Name = permissionDto.Name;
                existingPermission.Code = permissionDto.Code;
                existingPermission.Active = permissionDto.Active;

                return await _permissionData.UpdateAsync(existingPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RolId}", permissionDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol.", ex);
            }
        }


        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeletePermissionLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("PermissionId", "El PermissionId debe ser mayor a 0");

            try
            {
                var existingRol = await _permissionData.GetByIdAsync(id);
                if (existingRol == null)
                    throw new EntityNotFoundException("pérmissionUser", id);

                return await _permissionData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUserDto con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeletePermissionPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("permissionId", "El PermissionId debe ser mayor a 0");

            try
            {
                var existingForm = await _permissionData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _permissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }

        // Método para mapear de Rol a RolDTO
        private PermissionDto MapToDTO(Permission permission)

        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Code = permission.Code,
                Active = permission.Active,
                Description = permission.Description
            };
        }


        // Método para validar el DTO
        private void ValidatePermission(PermissionDto PermissionDto)
        {
            if (PermissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PermissionDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }

        // Método para mapear de RolDTO a Rol
        private Permission MapToEntity(PermissionDto permissionDTO)
        {
            return new Permission
            {
                Id = permissionDTO.Id,
                Name = permissionDTO.Name,
                Code = permissionDTO.Code,
                Description = permissionDTO.Description,
                Active = permissionDTO.Active // Si existe en la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<PermissionDto> MapToDTOList(IEnumerable<Permission> permissions)
        {
            var permissionDTO = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                permissionDTO.Add(MapToDTO(permission));
            }
            return permissionDTO;
        }
    }
}
