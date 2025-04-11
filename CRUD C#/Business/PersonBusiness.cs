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
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<PersonBusiness> _logger;

        public PersonBusiness(PersonData personData, ILogger<PersonBusiness> logger)
        {
            _personData = personData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<PersonDto>> GetAllRolesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var persons = await _personData.GetAllAsync();
                var personsDTO = MapToDTOList(persons);

                return personsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<PersonDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }
        // Método para crear un rol desde un DTO
        public async Task<PersonDto> CreatePersonAsync(PersonDto PersonDto)
        {
            try
            {
                ValidatePerson(PersonDto);

                var person = MapToEntity(PersonDto);

                var personCreado = await _personData.CreateAsync(person);

                return MapToDTO(personCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", PersonDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para actualizar un rol desde un DTO
        public async Task<bool> UpdatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var existingPerson = await _personData.GetByIdAsync(personDto.Id);
                if (existingPerson == null)
                {
                    throw new EntityNotFoundException("Rol", personDto.Id);
                }

                // Actualizar propiedades
                existingPerson.Id = personDto.Id;
                existingPerson.Name = personDto.Name;
                existingPerson.NumberIdentification = personDto.NumberIdentification;
                existingPerson.Phone = personDto.Phone;
                existingPerson.Active = personDto.Active;

                return await _personData.UpdateAsync(existingPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RolId}", personDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol.", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeletePersonLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("PerosnId", "El RolId debe ser mayor a 0");

            try
            {
                var existingRol = await _personData.GetByIdAsync(id);
                if (existingRol == null)
                    throw new EntityNotFoundException("Person", id);

                return await _personData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUserDto con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser", ex);
            }
        }

        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeletePerosnPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("PersonId", "El RolId debe ser mayor a 0");

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                    throw new EntityNotFoundException("Person", id);

                return await _personData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }

        // Método para validar el DTO
        private void ValidatePerson(PersonDto PersonDto)
        {
            if (PersonDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PersonDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }


        // Método para mapear de Rol a RolDTO
        private PersonDto MapToDTO(Person person)

        {
            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                LastName = person.LastName,
                NumberIdentification = person.NumberIdentification,
                Phone = person.Phone,
                Active = person.Active
            };
        }

        // Método para mapear de RolDTO a Rol
        private Person MapToEntity(PersonDto personDTO)
        {
            return new Person
            {
                Id = personDTO.Id,
                Name = personDTO.Name,
                LastName = personDTO.LastName,
                NumberIdentification = personDTO.NumberIdentification,
                Phone = personDTO.Phone,
                Active = personDTO.Active// Si existe en la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> persons)
        {
            var personsDTO = new List<PersonDto>();
            foreach (var person in persons)
            {
                personsDTO.Add(MapToDTO(person));
            }
            return personsDTO;
        }
    }
}
