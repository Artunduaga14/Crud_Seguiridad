using Data;
using Date;
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
    public class InventaryDetailsBusiness
    {
    
        private readonly InventaryDetailsData _inventaryDetailsData;
        private readonly ILogger<InventaryDetailsBusiness> _logger;

        public InventaryDetailsBusiness(InventaryDetailsData inventaryDetailsData, ILogger<InventaryDetailsBusiness> logger)
        {
            _inventaryDetailsData = inventaryDetailsData;
            _logger = logger;
        }

        // Método para obtener todos los inventaryDetails como DTOs
        public async Task<IEnumerable<InventaryDetailsDto>> GetAllInventaryDetailsesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var inventaryDetails = await _inventaryDetailsData.GetAllAsync();
                var inventaryDetailsDTO = MapToDTOList(inventaryDetails);

                return inventaryDetailsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los inventaryDetails");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de inventaryDetails", ex);
            }
        }


        // Método para obtener un inventaryDetails por ID como DTO
        public async Task<InventaryDetailsDto> GetInventaryDetailsByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un inventaryDetails con ID inválido: {InventaryDetailsId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del inventaryDetails debe ser mayor que cero");
            }

            try
            {
                var inventaryDetails = await _inventaryDetailsData.GetByIdAsync(id);
                if (inventaryDetails == null)
                {
                    _logger.LogInformation("No se encontró ningún inventaryDetails con ID: {InventaryDetailsId}", id);
                    throw new EntityNotFoundException("InventaryDetails", id);
                }

                return MapToDTO(inventaryDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el inventaryDetails con ID: {InventaryDetailsId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el inventaryDetails con ID {id}", ex);
            }
        }

        // Método para crear un inventaryDetails desde un DTO
        public async Task<InventaryDetailsDto> CreateInventaryDetailsAsync(InventaryDetailsDto inventaryDetailsDto)
        {
            try
            {
                ValidateInventaryDetails(inventaryDetailsDto);

                var inventaryDetails = MapToEntity(inventaryDetailsDto);

                var inventaryDetailsCreado = await _inventaryDetailsData.CreateAsync(inventaryDetails);

                return MapToDTO(inventaryDetailsCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo inventaryDetails: {InventaryDetailsNombre}", inventaryDetailsDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el inventaryDetails", ex);
            }
        }


        // Método para actualizar un inventaryDetails desde un DTO
        public async Task<bool> UpdateInventaryDetailsAsync(InventaryDetailsDto inventaryDetailsDto)
        {
            try
            {
                ValidateInventaryDetails(inventaryDetailsDto);

                var existingInventaryDetails = await _inventaryDetailsData.GetByIdAsync(inventaryDetailsDto.Id);
                if (existingInventaryDetails == null)
                {
                    throw new EntityNotFoundException("InventaryDetails", inventaryDetailsDto.Id);
                }

                // Actualizar propiedades
                existingInventaryDetails.Id = inventaryDetailsDto.Id;
                existingInventaryDetails.StatusPrevious = inventaryDetailsDto.StatusPrevious;
                existingInventaryDetails.StatusNew = inventaryDetailsDto.StatusNew;
                existingInventaryDetails.Observations = inventaryDetailsDto.Observations;
                existingInventaryDetails.Date = inventaryDetailsDto.Date;
                existingInventaryDetails.Description = inventaryDetailsDto.Description;
                existingInventaryDetails.Active = inventaryDetailsDto.Active;
                existingInventaryDetails.ZoneId = inventaryDetailsDto.ZoneId;

                return await _inventaryDetailsData.UpdateAsync(existingInventaryDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el inventaryDetails con ID: {InventaryDetailsId}", inventaryDetailsDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el inventaryDetails.", ex);
            }
        }


        // Método para eliminar un InventaryDetailsulario Logicamente 
        public async Task<bool> DeleteInventaryDetailsLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("InventaryDetailsId", "El InventaryDetailsId debe ser mayor a 0");

            try
            {
                var existingInventaryDetails = await _inventaryDetailsData.GetByIdAsync(id);
                if (existingInventaryDetails == null)
                    throw new EntityNotFoundException("InventaryDetails", id);

                return await _inventaryDetailsData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el InventaryDetailsDto con ID {InventaryDetailsId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el InventaryDetails", ex);
            }
        }

        //  Método para eliminar un InventaryDetailsulario de manera persistente 
        public async Task<bool> DeleteInventaryDetailsPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("InventaryDetailsId", "El InventaryDetailsId debe ser mayor a 0");

            try
            {
                var existingInventaryDetails = await _inventaryDetailsData.GetByIdAsync(id);
                if (existingInventaryDetails == null)
                    throw new EntityNotFoundException("InventaryDetails", id);

                return await _inventaryDetailsData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el InventaryDetails con ID {InventaryDetailsId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el InventaryDetails", ex);
            }
        }


        // Validar DTO
        public void ValidateInventaryDetails(InventaryDetailsDto inventaryDetailsDto)
        {
            if (inventaryDetailsDto == null)
                throw new ValidationException("InventaryDetails", "El rol no puede ser nulo");

            if (inventaryDetailsDto.Id <= 0)
                throw new ValidationException("InventaryDetailsId", "El InventaryDetailsId debe ser mayor a 0");

            if (inventaryDetailsDto.ZoneId <= 0)
                throw new ValidationException("ZoneId", "El ZoneId debe ser mayor a 0");
        }

        private void ValidateInventaryDetails(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }


        // Método para mapear de InventaryDetails a InventaryDetailsDTO
        private InventaryDetailsDto MapToDTO(InventaryDetails inventaryDetails)

        {
            return new InventaryDetailsDto
            {
                Id = inventaryDetails.Id,
                StatusPrevious = inventaryDetails.StatusPrevious,
                StatusNew = inventaryDetails.StatusNew,
                Observations = inventaryDetails.Observations,
                Date = inventaryDetails.Date,
                Description = inventaryDetails.Description,
                Active = inventaryDetails.Active,
                ZoneId = inventaryDetails.ZoneId,
            };
        }

        // Método para mapear de InventaryDetailsDTO a InventaryDetails
        private InventaryDetails MapToEntity(InventaryDetailsDto inventaryDetailsDTO)
        {
            return new InventaryDetails
            {
                Id = inventaryDetailsDTO.Id,
                StatusPrevious = inventaryDetailsDTO.StatusPrevious,
                StatusNew = inventaryDetailsDTO.StatusNew,
                Observations = inventaryDetailsDTO.Observations,
                Date = inventaryDetailsDTO.Date,
                Description = inventaryDetailsDTO.Description,
                Active = inventaryDetailsDTO.Active,
                ZoneId = inventaryDetailsDTO.ZoneId,
            };
        }

        // Método para mapear una lista de InventaryDetails a una lista de InventaryDetailsDTO
        private IEnumerable<InventaryDetailsDto> MapToDTOList(IEnumerable<InventaryDetails> inventaryDetailses)
        {
            var inventaryDetailsesDTO = new List<InventaryDetailsDto>();
            foreach (var inventaryDetails in inventaryDetailses)
            {
                inventaryDetailsesDTO.Add(MapToDTO(inventaryDetails));
            }
            return inventaryDetailsesDTO;
        }
    }
}
