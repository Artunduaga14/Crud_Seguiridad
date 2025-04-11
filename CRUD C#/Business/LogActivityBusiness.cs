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
    public class LogActivityBusiness
    {
        private readonly LogActivityData _logActivityData;
        private readonly ILogger<LogActivityBusiness> _logger;

        public LogActivityBusiness(LogActivityData logActivityData, ILogger<LogActivityBusiness> logger)
        {
            _logActivityData = logActivityData;
            _logger = logger;
        }





        // Método para obtener todos los LogActivitys como DTOs
        public async Task<IEnumerable<LogActivityDto>> GetAllLogActivityesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var LogActivitys = await _logActivityData.GetAllAsync();
                var LogActivitysDTO = MapToDTOList(LogActivitys);

                return LogActivitysDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los LogActivitys");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de LogActivitys", ex);
            }
        }

        // Método para obtener un logActivity por ID como DTO
        public async Task<LogActivityDto> GetLogActivityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un logActivity con ID inválido: {LogActivityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del logActivity debe ser mayor que cero");
            }

            try
            {
                var logActivity = await _logActivityData.GetByIdAsync(id);
                if (logActivity == null)
                {
                    _logger.LogInformation("No se encontró ningún logActivity con ID: {LogActivityId}", id);
                    throw new EntityNotFoundException("LogActivity", id);
                }

                return MapToDTO(logActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el logActivity con ID: {LogActivityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el logActivity con ID {id}", ex);
            }
        }

        // Método para crear un logActivity desde un DTO
        public async Task<LogActivityDto> CreateLogActivityAsync(LogActivityDto LogActivityDto)
        {
            try
            {
                ValidateLogActivity(LogActivityDto);

                var logActivity = MapToEntity(LogActivityDto);

                var logActivityCreado = await _logActivityData.CreateAsync(logActivity);

                return MapToDTO(logActivityCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo logActivity: {LogActivityNombre}", LogActivityDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el logActivity", ex);
            }
        }

        // Método para actualizar un logActivity desde un DTO
        public async Task<bool> UpdateLogActivityAsync(LogActivityDto logActivityDto)
        {
            try
            {
                ValidateLogActivity(logActivityDto);

                var existingLogActivity = await _logActivityData.GetByIdAsync(logActivityDto.Id);
                if (existingLogActivity == null)
                {
                    throw new EntityNotFoundException("LogActivity", logActivityDto.Id);
                }

                // Actualizar propiedades
                existingLogActivity.Id = logActivityDto.Id;
                existingLogActivity.Action = logActivityDto. Action;
                existingLogActivity.DataPrevious = logActivityDto.DataPrevious;
                existingLogActivity.DataNew = logActivityDto.DataNew;
                existingLogActivity.Data = logActivityDto.Data;
                existingLogActivity.Active = logActivityDto.Active;

                return await _logActivityData.UpdateAsync(existingLogActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el logActivity con ID: {LogActivityId}", logActivityDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el logActivity.", ex);
            }
        }

        // Método para eliminar un LogActivityulario Logicamente 
        public async Task<bool> DeleteLogActivityLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("LogActivityId", "El LogActivityId debe ser mayor a 0");

            try
            {
                var existingLogActivity = await _logActivityData.GetByIdAsync(id);
                if (existingLogActivity == null)
                    throw new EntityNotFoundException("LogActivityUser", id);

                return await _logActivityData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el LogActivityUserDto con ID {LogActivityUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el LogActivityUser", ex);
            }
        }

        //  Método para eliminar un LogActivity de manera persistente 
        public async Task<bool> DeleteLogActivityPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("LogActivityId", "El LogActivityId debe ser mayor a 0");

            try
            {
                var existingLogActivity = await _logActivityData.GetByIdAsync(id);
                if (existingLogActivity == null)
                    throw new EntityNotFoundException("LogActivity", id);

                return await _logActivityData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el LogActivityUser con ID {LogActivityUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el LogActivityUser", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateLogActivity(LogActivityDto LogActivityDto)
        {
            if (LogActivityDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto logActivity no puede ser nulo");
            }

            //if (LogActivityDto.Id <= 0)
            //{
            //    _logger.LogWarning("Se intentó crear/actualizar un logActivity con ID inválido");
            //    throw new ValidationException("Id", "El ID del logActivity debe ser mayor que cero");
            //}
        }


        // Método para mapear de LogActivity a LogActivityDTO
        private LogActivityDto MapToDTO(LogActivity logActivity)

        {
            return new LogActivityDto
            {
                Id = logActivity.Id,
                Action = logActivity.Action,
                DataPrevious = logActivity.DataPrevious,
                Data = logActivity.Data,
                Active = logActivity.Active,
                DataNew = logActivity.DataNew
            };
        }

        // Método para mapear de LogActivityDTO a LogActivity
        private LogActivity MapToEntity(LogActivityDto logActivityDTO)
        {
            return new LogActivity
            {
                Id = logActivityDTO.Id,
                Action = logActivityDTO.Action,
                DataPrevious = logActivityDTO.DataPrevious,
                Data = logActivityDTO.Data,
                Active = logActivityDTO.Active,
                DataNew = logActivityDTO.DataNew
            };
        }

        // Método para mapear una lista de LogActivity a una lista de LogActivityDTO
        private IEnumerable<LogActivityDto> MapToDTOList(IEnumerable<LogActivity> logActivityes)
        {
            var logActivityesDTO = new List<LogActivityDto>();
            foreach (var logActivity in logActivityes)
            {
                logActivityesDTO.Add(MapToDTO(logActivity));
            }
            return logActivityesDTO;
        }
    }
}
