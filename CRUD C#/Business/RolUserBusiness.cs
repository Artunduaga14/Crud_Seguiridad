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
    public class RolUserBusiness
    {
        private readonly RolUserData _rolUserData;
        private readonly ILogger<RolUserBusiness> _logger;

        public RolUserBusiness(RolUserData rolUserData, ILogger<RolUserBusiness> logger)
        {
            _rolUserData = rolUserData;
            _logger = logger;
        }

        // Método para obtener todos los rolUsers como DTOs
        public async Task<IEnumerable<RolUserDto>> GetAllRolUseresAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var rolUsers = await _rolUserData.GetAllAsync();
                var rolUsersDTO = MapToDTOList(rolUsers);

                return rolUsersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rolUsers");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de rolUsers", ex);
            }
        }


        // Método para obtener un rol por ID como DTO
        public async Task<RolUserDto> GetRolUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolUserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("No se encontró ningún rolUser con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                return MapToDTO(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rolUser con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<RolUserDto> CreateRolUserAsync(RolUserDto RolUserDto)
        {
            try
            {
                ValidateRolUser(RolUserDto);

                var rolUser = MapToEntity(RolUserDto);

                var rolUserCreado = await _rolUserData.CreateAsync(rolUser);

                return MapToDTO(rolUserCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rolUser: {RolUserNombre}", RolUserDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }




        // Método para actualizar un rol desde un DTO
        public async Task<bool> UpdateRolUserAsync(RolUserDto rolUserDto)
        {
            try
            {
                ValidateRolUser(rolUserDto);

                var existingRolUser = await _rolUserData.GetByIdAsync(rolUserDto.Id);
                if (existingRolUser == null)
                {
                    throw new EntityNotFoundException("RolUser", rolUserDto.Id);
                }

                // Actualizar propiedades
                existingRolUser.Id = rolUserDto.Id;
                existingRolUser.UserId = rolUserDto.UserId;
                existingRolUser.RolId = rolUserDto.RolId;
                existingRolUser.Active = rolUserDto.Active;


                return await _rolUserData.UpdateAsync(existingRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rolUser con ID: {RolUserId}", rolUserDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol.", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteRolUserLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("RolUserId", "El RolUserId debe ser mayor a 0");

            try
            {
                var existingRolUser = await _rolUserData.GetByIdAsync(id);
                if (existingRolUser == null)
                    throw new EntityNotFoundException("RolUserUser", id);

                return await _rolUserData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUserUserDto con ID {RolUserUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser", ex);
            }
        }

        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteRolUserPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("RolUserId", "El RolUserId debe ser mayor a 0");

            try
            {
                var existingForm = await _rolUserData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _rolUserData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }

        // Validar DTO
        public void ValidateRolUser(RolUserDto rolUserDto)
        {
            if (rolUserDto == null)
                throw new ValidationException("RolUser", "El rolUser no puede ser nulo");

            //if (rolUserDto.Id <= 0)
            //    throw new ValidationException("RolId", "El RolId debe ser mayor a 0");

            if (rolUserDto.RolId <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");

            if (rolUserDto.UserId <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }

        // Método para mapear de Rol a RolDTO
        private RolUserDto MapToDTO(RolUser rolUser)

        {
            return new RolUserDto
            {
                Id = rolUser.Id,
                UserId = rolUser.UserId,
                RolId = rolUser.RolId,
                Active = rolUser.Active,
            };
        }

        // Método para mapear de RolUserDTO a RolUser
        private RolUser MapToEntity(RolUserDto rolUserDTO)
        {
            return new RolUser
            {
                Id = rolUserDTO.Id,
                UserId = rolUserDTO.UserId,
                RolId = rolUserDTO.RolId,
                Active = rolUserDTO.Active,
            };
        }

        // Método para mapear una lista de RolUser a una lista de RolUserDTO
        private IEnumerable<RolUserDto> MapToDTOList(IEnumerable<RolUser> roles)
        {
            var rolesDTO = new List<RolUserDto>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(MapToDTO(rol));
            }
            return rolesDTO;
        }

    }
}
