using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class FormModuleBusiness
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<FormModuleBusiness> _logger;

        public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleBusiness> logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        // Método para obtener todos los FormModules como DTOs
        public async Task<IEnumerable<FormModuleDto>> GetAllFormModulesAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                var formModulesDTO = MapToDTOList(formModules);

                return formModulesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los FormModules");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de FormModules", ex);
            }
        }

        // Método para obtener un FormModule por ID como DTO
        public async Task<FormModuleDto> GetFormModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un FormModule con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID del FormModule debe ser mayor que cero");
            }

            try
            {
                var formModule = await _formModuleData.GetByIdAsync(id);
                if (formModule == null)
                {
                    _logger.LogInformation("No se encontró ningún FormModule con ID: {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return MapToDTO(formModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el FormModule con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el FormModule con ID {id}", ex);
            }
        }

        // Método para crear un FormModule desde un DTO
        public async Task<FormModuleDto> CreateFormModuleAsync(FormModuleDto formModuleDto)
        {
            try
            {
                ValidateFormModule(formModuleDto);

                var formModule = MapToEntity(formModuleDto);
                var createdFormModule = await _formModuleData.CreateAsync(formModule);

                return MapToDTO(createdFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo FormModule: {FormModuleName}", formModuleDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el FormModule", ex);
            }
        }

        // Método para actualizar un FormModule desde un DTO
        public async Task<bool> UpdateFormModuleAsync(FormModuleDto formModuleDto)
        {
            try
            {
                ValidateFormModule(formModuleDto);

                var existingFormModule = await _formModuleData.GetByIdAsync(formModuleDto.Id);
                if (existingFormModule == null)
                {
                    throw new EntityNotFoundException("FormModule", formModuleDto.Id);
                }

                // Actualizar propiedades
                existingFormModule.FormId = formModuleDto.FormId;
                existingFormModule.ModuleId = formModuleDto.ModuleId;
                existingFormModule.Active = formModuleDto.Active;

                return await _formModuleData.UpdateAsync(existingFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el FormModule con ID: {FormModuleId}", formModuleDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el FormModule.", ex);
            }
        }

        // Método para eliminar un FormModule lógicamente
        public async Task<bool> DeleteFormModuleLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("FormModuleId", "El FormModuleId debe ser mayor a 0");

            try
            {
                var existingFormModule = await _formModuleData.GetByIdAsync(id);
                if (existingFormModule == null)
                    throw new EntityNotFoundException("FormModule", id);

                return await _formModuleData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el FormModule con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el FormModule", ex);
            }
        }

        // Método para eliminar un FormModule de forma permanente
        public async Task<bool> DeleteFormModulePersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("FormModuleId", "El FormModuleId debe ser mayor a 0");

            try
            {
                var existingFormModule = await _formModuleData.GetByIdAsync(id);
                if (existingFormModule == null)
                    throw new EntityNotFoundException("FormModule", id);

                return await _formModuleData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el FormModule con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el FormModule", ex);
            }
        }




        // Validación de DTO
        private void ValidateFormModule(FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
            {
                throw new ValidationException("El objeto FormModule no puede ser nulo");
            }

            if (formModuleDto.FormId <= 0)
            {
                _logger.LogWarning("FormId inválido: {FormId}", formModuleDto.FormId);
                throw new ValidationException("FormId", "El FormId debe ser mayor a cero");
            }

            if (formModuleDto.ModuleId <= 0)
            {
                _logger.LogWarning("ModuleId inválido: {ModuleId}", formModuleDto.ModuleId);
                throw new ValidationException("ModuleId", "El ModuleId debe ser mayor a cero");
            }
        }


        // Mapeo de entidad a DTO
        private FormModuleDto MapToDTO(FormModule formModule)
        {
            return new FormModuleDto
            {
                Id = formModule.Id,
                FormId = formModule.FormId,
                ModuleId = formModule.ModuleId,
                Active = formModule.Active
            };
        }

        // Mapeo de DTO a entidad
        private FormModule MapToEntity(FormModuleDto formModuleDto)
        {
            return new FormModule
            {
                Id = formModuleDto.Id,
                FormId = formModuleDto.FormId,
                ModuleId = formModuleDto.ModuleId,
                Active = formModuleDto.Active
            };
        }

        // Mapeo de lista de entidades a lista de DTOs
        private List<FormModuleDto> MapToDTOList(IEnumerable<FormModule> formModules)
        {
            return formModules.Select(MapToDTO).ToList();
        }
    }
}

