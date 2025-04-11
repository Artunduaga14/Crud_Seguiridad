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
    public class ZoneBusiness
    {
        private readonly ZoneData _zoneData;
        private readonly ILogger<ZoneBusiness> _logger;

        public ZoneBusiness(ZoneData zoneData, ILogger<ZoneBusiness> logger)
        {
            _zoneData = zoneData;
            _logger = logger;
        }

        // Método para obtener todos los Zones como DTOs
        public async Task<IEnumerable<ZoneDto>> GetAllZoneesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var Zones = await _zoneData.GetAllAsync();
                var ZonesDTO = MapToDTOList(Zones);

                return ZonesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Zones");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Zones", ex);
            }
        }

        // Método para obtener un zone por ID como DTO
        public async Task<ZoneDto> GetZoneByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un zone con ID inválido: {ZoneId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del zone debe ser mayor que cero");
            }

            try
            {
                var zone = await _zoneData.GetByIdAsync(id);
                if (zone == null)
                {
                    _logger.LogInformation("No se encontró ningún zone con ID: {ZoneId}", id);
                    throw new EntityNotFoundException("Zone", id);
                }

                return MapToDTO(zone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el zone con ID: {ZoneId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el zone con ID {id}", ex);
            }
        }


        // Método para crear un zone desde un DTO
        public async Task<ZoneDto> CreateZoneAsync(ZoneDto ZoneDto)
        {
            try
            {
                ValidateZone(ZoneDto);

                var zone = MapToEntity(ZoneDto);

                var zoneCreado = await _zoneData.CreateAsync(zone);

                return MapToDTO(zoneCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo zone: {ZoneNombre}", ZoneDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el zone", ex);
            }
        }


        // Método para actualizar un zone desde un DTO
        public async Task<bool> UpdateZoneAsync(ZoneDto zoneDto)
        {
            try
            {
                ValidateZone(zoneDto);

                var existingZone = await _zoneData.GetByIdAsync(zoneDto.Id);
                if (existingZone == null)
                {
                    throw new EntityNotFoundException("Zone", zoneDto.Id);
                }

                // Actualizar propiedades
                existingZone.Id = zoneDto.Id;
                existingZone.BranchId = zoneDto.BranchId;
                existingZone.Active = zoneDto.Active;

                return await _zoneData.UpdateAsync(existingZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el zone con ID: {ZoneId}", zoneDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el zone.", ex);
            }
        }

        // Método para eliminar un Zoneulario Logicamente 
        public async Task<bool> DeleteZoneLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ZoneId", "El ZoneId debe ser mayor a 0");

            try
            {
                var existingZone = await _zoneData.GetByIdAsync(id);
                if (existingZone == null)
                    throw new EntityNotFoundException("ZoneUser", id);

                return await _zoneData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el ZoneUserDto con ID {ZoneUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el ZoneUser", ex);
            }
        }

        //  Método para eliminar un Zoneulario de manera persistente 
        public async Task<bool> DeleteZonePersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ZoneId", "El ZoneId debe ser mayor a 0");

            try
            {
                var existingZone = await _zoneData.GetByIdAsync(id);
                if (existingZone == null)
                    throw new EntityNotFoundException("Zone", id);

                return await _zoneData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el ZoneUser con ID {ZoneUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el ZoneUser", ex);
            }
        }



        // Validar DTO
        public void ValidateZone(ZoneDto zoneDtop)
        {
            if (zoneDtop == null)
                throw new ValidationException("Zone", "El rolUser no puede ser nulo");

            //if (zoneDtop.Id <= 0)
            //    throw new ValidationException("ZoneId", "El ZoneId debe ser mayor a 0");

            if (zoneDtop.BranchId <= 0)
                throw new ValidationException("UserId", "El UserId debe ser mayor a 0");
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }


        // Método para mapear de Zone a ZoneDTO
        private ZoneDto MapToDTO(Zone zone)

        {
            return new ZoneDto
            {
                Id = zone.Id,
                BranchId = zone.BranchId,
                Active = zone.Active,
            };
        }

        // Método para mapear de ZoneDTO a Zone
        private Zone MapToEntity(ZoneDto zoneDTO)
        {
            return new Zone
            {
                Id = zoneDTO.Id,
                BranchId = zoneDTO.BranchId,
                Active = zoneDTO.Active,
            };
        }

        // Método para mapear una lista de Zone a una lista de ZoneDTO
        private IEnumerable<ZoneDto> MapToDTOList(IEnumerable<Zone> zonees)
        {
            var zoneesDTO = new List<ZoneDto>();
            foreach (var zone in zonees)
            {
                zoneesDTO.Add(MapToDTO(zone));
            }
            return zoneesDTO;
        }
    }
}
