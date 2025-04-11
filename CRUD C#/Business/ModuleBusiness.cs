using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var modules = await _moduleData.GetAllAsync();
                var moduleDTO = MapToDTOList(modules);

                return moduleDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los modules");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modules", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un module con ID inválido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del module debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún Module con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return MapToDTO(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Module con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Module con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto ModuleDto)
        {
            try
            {
                ValidateModule(ModuleDto);

                var module = MapToEntity(ModuleDto);

                var moduleCreado = await _moduleData.CreateAsync(module);

                return MapToDTO(moduleCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo module: {ModuleNombre}", ModuleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }


        // Método para actualizar un rol desde un DTO
        public async Task<bool> UpdateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var existingModule = await _moduleData.GetByIdAsync(moduleDto.Id);
                if (existingModule == null)
                {
                    throw new EntityNotFoundException("Module", moduleDto.Id);
                }

                // Actualizar propiedades
                existingModule.Id = moduleDto.Id;
                existingModule.Name = moduleDto.Name;
                existingModule.Description = moduleDto.Description;
                //existingModule.CreationData = moduleDto.CreationData;
                existingModule.Active = moduleDto.Active;

                return await _moduleData.UpdateAsync(existingModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID: {RolId}", moduleDto.Id);
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
                var existingRol = await _moduleData.GetByIdAsync(id);
                if (existingRol == null)
                    throw new EntityNotFoundException("pérmissionUser", id);

                return await _moduleData.DeleteLogicAsync(id);
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
                var existingForm = await _moduleData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _moduleData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateModule(ModuleDto ModuleDto)
        {
            if (ModuleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ModuleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Module con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del module es obligatorio");
            }
        }

        // Método para mapear de Rol a RolDTO
        private ModuleDto MapToDTO(Module module)

        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description
            };
        }

        // Método para mapear de RolDTO a Rol
        private Module MapToEntity(ModuleDto moduleDTO)
        {
            return new Module
            {
                Id = moduleDTO.Id,
                Name = moduleDTO.Name,
                Description = moduleDTO.Description,
                /*Active = moduleDTO.Active*/ // Si existe en la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<ModuleDto> MapToDTOList(IEnumerable<Module> Modules)
        {
            var modulesDTO = new List<ModuleDto>();
            foreach (var module in Modules)
            {
                modulesDTO.Add(MapToDTO(module));
            }
            return modulesDTO;
        }

    }
}
