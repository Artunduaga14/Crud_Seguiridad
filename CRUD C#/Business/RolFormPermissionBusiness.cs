using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly ILogger<RolFormPermissionBusiness> _logger;

        public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _logger = logger;
        }

        // Método para obtener todos los RolFormPermissiones como DTOs
        public async Task<IEnumerable<RolFormPermissionDto>> GetAllRolFormPermissionAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var rolFormPermissiones = await _rolFormPermissionData.GetAllAsync();
                var rolFormPermissionesDTO = MapToDTOList(rolFormPermissiones);

                return rolFormPermissionesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolFormPermissiones");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolFormPermissiones", ex);
            }
        }



        // Método para obtener un rolFormPermission por ID como DTO
        public async Task<RolFormPermissionDto> GetRolFormPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rolFormPermission con ID inválido: {RolFormPermissionId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rolFormPermission debe ser mayor que cero");
            }

            try
            {
                var rolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
                if (rolFormPermission == null)
                {
                    _logger.LogInformation("No se encontró ningún rolFormPermission con ID: {RolFormPermissionId}", id);
                    throw new EntityNotFoundException("RolFormPermission", id);
                }

                return MapToDTO(rolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rolFormPermission con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rolFormPermission con ID {id}", ex);
            }
        }


        // Método para crear un rol desde un DTO
        public async Task<RolFormPermissionDto> CreateRolFormPermissionAsync(RolFormPermissionDto RolFormPermissionDto)
        {
            try
            {
                ValidateRolFormPermission(RolFormPermissionDto);

                var rolFormPermission = MapToEntity(RolFormPermissionDto);

                var rolFormPermissionCreado = await _rolFormPermissionData.CreateAsync(rolFormPermission);

                return MapToDTO(rolFormPermissionCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rolFormPermission: {RolFormPermissionNombre}", RolFormPermissionDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el rolFormPermission", ex);
            }
        }


        public async Task<bool> UpdateRolFormPermissionAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                ValidateRolFormPermission(rolFormPermissionDto);

                var existingRolFormPermission = await _rolFormPermissionData.GetByIdAsync(rolFormPermissionDto.Id);
                if (existingRolFormPermission == null)
                {
                    throw new EntityNotFoundException("RolFormPermission", rolFormPermissionDto.Id);
                }

                // Actualizar propiedades
                existingRolFormPermission.Id = rolFormPermissionDto.Id;
                existingRolFormPermission.PermissionId = rolFormPermissionDto.PermissionId;
                existingRolFormPermission.RolId = rolFormPermissionDto.RolId;
                existingRolFormPermission.FormId = rolFormPermissionDto.FormId;
                existingRolFormPermission.Active = rolFormPermissionDto.Active;

                return await _rolFormPermissionData.UpdateAsync(existingRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rolFormPermission con ID: {RolFormPermissionId}", rolFormPermissionDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol.", ex);
            }
        }


        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteRolFormPermissionLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("RolFormPermissionId", "El RolFormPermissionId debe ser mayor a 0");

            try
            {
                var existingRolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
                if (existingRolFormPermission == null)
                    throw new EntityNotFoundException("RolFormPermissionUser", id);

                return await _rolFormPermissionData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolFormPermissionUserDto con ID {RolFormPermissionUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolFormPermissionUser", ex);
            }
        }

        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteRolFormPermissionPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("RolFormPermissionId", "El RolFormPermissionId debe ser mayor a 0");

            try
            {
                var existingForm = await _rolFormPermissionData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _rolFormPermissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolFormPermissionUser con ID {RolFormPermissionUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolFormPermissionUser", ex);
            }
        }

        // Validar DTO
        public void ValidateRolFormPermission(RolFormPermissionDto RolFormPermissionDto)
        {
            if (RolFormPermissionDto == null)
                throw new ValidationException("RolFormPermission", "El rolUser no puede ser nulo");

            //if (RolFormPermissionDto.Id <= 0)
            //    throw new ValidationException("RolFormPermissionId", "El RolFormPermissionId debe ser mayor a 0");

            if (RolFormPermissionDto.PermissionId <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");

            if (RolFormPermissionDto.RolId <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");

            if (RolFormPermissionDto.FormId <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }

        // Método para mapear de Rol a RolDTO
        private RolFormPermissionDto MapToDTO(RolFormPermission rolFormPermission)

        {
            return new RolFormPermissionDto
            {
                Id = rolFormPermission.Id,
                PermissionId = rolFormPermission.PermissionId,
                RolId = rolFormPermission.RolId,
                FormId = rolFormPermission.FormId,
                Active = rolFormPermission.Active,

            };
        }

        // Método para mapear de RolFormPermissionDTO a RolFormPermission
        private RolFormPermission MapToEntity(RolFormPermissionDto rolFormPermissionDTO)
        {
            return new RolFormPermission
            {
                Id = rolFormPermissionDTO.Id,
                PermissionId = rolFormPermissionDTO.PermissionId,
                RolId = rolFormPermissionDTO.RolId,
                FormId = rolFormPermissionDTO.FormId,
                Active = rolFormPermissionDTO.Active,
            };
        }

        // Método para mapear una lista de RolFormPermission a una lista de RolFormPermissionDTO
        private IEnumerable<RolFormPermissionDto> MapToDTOList(IEnumerable<RolFormPermission> rolFormPermissiones)
        {
            var rolFormPermissionesDTO = new List<RolFormPermissionDto>();
            foreach (var rolFormPermission in rolFormPermissiones)
            {
                rolFormPermissionesDTO.Add(MapToDTO(rolFormPermission));
            }
            return rolFormPermissionesDTO;
        }
    }
}
