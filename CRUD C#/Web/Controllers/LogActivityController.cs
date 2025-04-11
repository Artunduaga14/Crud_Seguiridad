using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{

    /// <summary>
    /// Controlador para la gestión de permisos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LogActivityController : ControllerBase
    {
        private readonly LogActivityBusiness _LogActivityBusiness;
        private readonly ILogger<LogActivityController> _logger;

        /// <summary>
        /// Constructor del controlador de permisos
        /// </summary>
        /// <param name="LogActivityBusiness">Capa de negocio de permisos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public LogActivityController(LogActivityBusiness LogActivityBusiness, ILogger<LogActivityController> logger)
        {
            _LogActivityBusiness = LogActivityBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos del sistema
        /// </summary>
        /// <returns>Lista de permisos</returns>
        /// <response code="200">Retorna la lista de permisos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LogActivityDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllLogActivitys()
        {
            try
            {
                var LogActivitys = await _LogActivityBusiness.GetAllLogActivityesAsync();
                return Ok(LogActivitys);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un permiso específico por su ID
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <returns>Permiso solicitado</returns>
        /// <response code="200">Retorna el permiso solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permiso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LogActivityDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLogActivityById(int id)
        {
            try
            {
                var LogActivity = await _LogActivityBusiness.GetLogActivityByIdAsync(id);
                return Ok(LogActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {LogActivityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {LogActivityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {LogActivityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="LogActivityDto">Datos del permiso a crear</param>
        /// <returns>Permiso creado</returns>
        /// <response code="201">Retorna el permiso creado</response>
        /// <response code="400">Datos del permiso no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(LogActivityDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateLogActivity([FromBody] LogActivityDto LogActivityDto)
        {
            try
            {
                var createdLogActivity = await _LogActivityBusiness.CreateLogActivityAsync(LogActivityDto);
                return CreatedAtAction(nameof(GetLogActivityById), new { id = createdLogActivity.Id }, createdLogActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LogActivityDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateLogActivityAsync(int id, [FromBody] LogActivityDto LogActivityDto)
        {
            if (id != LogActivityDto.Id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
            }
            try
            {


                var updatedLogActivity = await _LogActivityBusiness.UpdateLogActivityAsync(LogActivityDto);
                return Ok(updatedLogActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el LogActivity con ID: {LogActivityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "LogActivity no encontrado con ID: {LogActivityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID: {LogActivityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// eliminar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogActivityLogical(int id)
        {
            try
            {
                var deleted = await _LogActivityBusiness.DeleteLogActivityLogicalAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Personas no encontrado o ya eliminado" });

                return Ok(new { message = "Personas eliminado exitosamente" });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el Personas con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("persistent/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePersistent(int id)
        {
            try
            {
                var success = await _LogActivityBusiness.DeleteLogActivityPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "Form no encontrado o ya eliminado" });

                return Ok(new { message = "Form eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Form con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar la Form" });
            }
        }
    }
}