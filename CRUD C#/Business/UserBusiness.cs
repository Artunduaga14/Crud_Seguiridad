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
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<UserDto>> GetAllUseresAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var users = await _userData.GetAllAsync();
                var usersDTO = MapToDTOList(users);

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDTO(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<UserDto> CreateRolAsync(UserDto UserDto)
        {
            try
            {
                ValidateUser(UserDto);

                var rol = MapToEntity(UserDto);

                var rolCreado = await _userData.CreateAsync(rol);

                return MapToDTO(rolCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", UserDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para actualizar un User desde un DTO
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var existingUser = await _userData.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                {
                    throw new EntityNotFoundException("User", userDto.Id);
                }

                // Actualizar propiedades
                existingUser.Id = userDto.Id;
                existingUser.Username = userDto.Username;
                existingUser.Password = userDto.Password;
                existingUser.Active = userDto.Active;
                existingUser.PersonId = userDto.PersonId;

                return await _userData.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {UserId}", userDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol.", ex);
            }
        }





        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteUserLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                    throw new EntityNotFoundException("UserUser", id);

                return await _userData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUserDto con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteUserPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                    throw new EntityNotFoundException("User", id);

                return await _userData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUser(UserDto UserDto)
        {
            if (UserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(UserDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }


        // Método para mapear de Rol a RolDTO
        private UserDto MapToDTO(User user)

        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Active = user.Active,
                PersonId = user.PersonId
                //Person = user.Person
            };
        }

        // Método para mapear de RolDTO a Rol
        private User MapToEntity(UserDto userDTO)
        {
            return new User
            {
                Id = userDTO.Id,
                Username = userDTO.Username,
                Password = userDTO.Password,
                Active = userDTO.Active,
                PersonId = userDTO.PersonId,
                /*Active = userDTO.Active*/ // Si existe en la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            var usersDTO = new List<UserDto>();
            foreach (var user in users)
            {
                usersDTO.Add(MapToDTO(user));
            }
            return usersDTO;
        }
    }
}
