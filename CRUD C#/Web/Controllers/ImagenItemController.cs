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
    public class ImagenItemController : ControllerBase
    {
        private readonly ImagenItemBusiness _ImagenItemBusiness;
        private readonly ILogger<ImagenItemController> _logger;

        /// <summary>
        /// Constructor del controlador de permisos
        /// </summary>
        /// <param name="ImagenItemBusiness">Capa de negocio de permisos</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public ImagenItemController(ImagenItemBusiness ImagenItemBusiness, ILogger<ImagenItemController> logger)
        {
            _ImagenItemBusiness = ImagenItemBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos del sistema
        /// </summary>
        /// <returns>Lista de permisos</returns>
        /// <response code="200">Retorna la lista de permisos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImagenItemDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllImagenItems()
        {
            try
            {
                var ImagenItems = await _ImagenItemBusiness.GetAllImagenItemesAsync();
                return Ok(ImagenItems);
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
        [ProducesResponseType(typeof(ImagenItemDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetImagenItemById(int id)
        {
            try
            {
                var ImagenItem = await _ImagenItemBusiness.GetImagenItemByIdAsync(id);
                return Ok(ImagenItem);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {ImagenItemId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {ImagenItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {ImagenItemId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }



        /// <summary>
        /// Crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="ImagenItemDto">Datos del permiso a crear</param>
        /// <returns>Permiso creado</returns>
        /// <response code="201">Retorna el permiso creado</response>
        /// <response code="400">Datos del permiso no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ImagenItemDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateImagenItem([FromBody] ImagenItemDto ImagenItemDto)
        {
            try
            {
                var createdImagenItem = await _ImagenItemBusiness.CreateImagenItemAsync(ImagenItemDto);
                return CreatedAtAction(nameof(GetImagenItemById), new { id = createdImagenItem.Id }, createdImagenItem);
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
        [ProducesResponseType(typeof(ImagenItemDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateImagenItemAsync(int id, [FromBody] ImagenItemDto ImagenItemDto)
        {
            if (id != ImagenItemDto.Id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto." });
            }
            try
            {


                var updatedImagenItem = await _ImagenItemBusiness.UpdateImagenItemAsync(ImagenItemDto);
                return Ok(updatedImagenItem);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el ImagenItem con ID: {ImagenItemId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "ImagenItem no encontrado con ID: {ImagenItemId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID: {ImagenItemId}", id);
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
        public async Task<IActionResult> DeleteImagenItemLogical(int id)
        {
            try
            {
                var deleted = await _ImagenItemBusiness.DeleteImagenItemLogicalAsync(id);
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
                var success = await _ImagenItemBusiness.DeleteImagenItemPersistentAsync(id);
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
