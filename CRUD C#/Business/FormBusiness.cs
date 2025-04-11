using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger<FormBusiness> _logger;

        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData;
            _logger = logger;
        }

        // Método para obtener todos los form como DTOs
        public async Task<IEnumerable<FormDto>> GetAllRolesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var forms = await _formData.GetAllAsync();
                var formsDTO = MapToDTOList(forms);

                return formsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Forms");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Método para obtener un form por ID como DTO
        public async Task<FormDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {Form}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);

                var form = MapToEntity(formDto);

                var formCreado = await _formData.CreateAsync(form);

                return MapToDTO(formCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo form: {FormNombre}", formDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el form", ex);
            }
        }

        // Método para actualizar un rol desde un DTO
        public async Task<bool> UpdateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto);

                var existingForm = await _formData.GetByIdAsync(formDto.Id);
                if (existingForm == null)
                {
                    throw new EntityNotFoundException("Forn", formDto.Id);
                }

                // Actualizar propiedades
                existingForm.Id = formDto.Id;
                existingForm.Name = formDto.Name;
                existingForm.Description = formDto.Description;
                existingForm.Active = formDto.Active;

                return await _formData.UpdateAsync(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RolId}", formDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol.", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteFormLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("PermissionId", "El PermissionId debe ser mayor a 0");

            try
            {
                var existingRol = await _formData.GetByIdAsync(id);
                if (existingRol == null)
                    throw new EntityNotFoundException("pérmissionUser", id);

                return await _formData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUserDto con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteFormPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("permissionId", "El PermissionId debe ser mayor a 0");

            try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _formData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }







        // Método para validar el DTO
        private void ValidateForm(FormDto formDto)
        {
            if (formDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto form no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un form con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del form es obligatorio");
            }
        }


        // Método para mapear de Rol a RolDTO
        private FormDto MapToDTO(Form form)

        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Active = form.Active,
                Description = form.Description
            };
        }

        // Método para mapear de RolDTO a Rol
        private Form MapToEntity(FormDto formDTO)
        {
            return new Form
            {
                Id = formDTO.Id,
                Name = formDTO.Name,
                Description = formDTO.Description,
                Active = formDTO.Active // Si existe en la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
        {
            var formsDTO = new List<FormDto>();
            foreach (var form in forms)
            {
                formsDTO.Add(MapToDTO(form));
            }
            return formsDTO;
        }
    }
}
