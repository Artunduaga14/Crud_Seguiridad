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
    public class ImagenItemBusiness
    {
        private readonly ImagenItemData _imagenItemData;
        private readonly ILogger<ImagenItemBusiness> _logger;

        public ImagenItemBusiness(ImagenItemData imagenItemData, ILogger<ImagenItemBusiness> logger)
        {
            _imagenItemData = imagenItemData;
            _logger = logger;
        }

        // Método para obtener todos los imagenItemes como DTOs
        public async Task<IEnumerable<ImagenItemDto>> GetAllImagenItemesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var imagenItemes = await _imagenItemData.GetAllAsync();
                var imagenItemesDTO = MapToDTOList(imagenItemes);

                return imagenItemesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los imagenItemes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de imagenItemes", ex);
            }
        }

        // Método para obtener un imagenItem por ID como DTO
        public async Task<ImagenItemDto> GetImagenItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un imagenItem con ID inválido: {ImagenItemId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del imagenItem debe ser mayor que cero");
            }

            try
            {
                var imagenItem = await _imagenItemData.GetByIdAsync(id);
                if (imagenItem == null)
                {
                    _logger.LogInformation("No se encontró ningún imagenItem con ID: {ImagenItemId}", id);
                    throw new EntityNotFoundException("ImagenItem", id);
                }

                return MapToDTO(imagenItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el imagenItem con ID: {ImagenItemId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el imagenItem con ID {id}", ex);
            }
        }

        // Método para crear un imagenItem desde un DTO
        public async Task<ImagenItemDto> CreateImagenItemAsync(ImagenItemDto imagenItemDto)
        {
            try
            {
                ValidateImagenItem(imagenItemDto);

                var imagenItem = MapToEntity(imagenItemDto);

                var imagenItemCreado = await _imagenItemData.CreateAsync(imagenItem);

                return MapToDTO(imagenItemCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo imagenItem: {ImagenItemNombre}", imagenItemDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el imagenItem", ex);
            }
        }

        // Método para actualizar un imagenItem desde un DTO
        public async Task<bool> UpdateImagenItemAsync(ImagenItemDto imagenItemDto)
        {
            try
            {
                ValidateImagenItem(imagenItemDto);

                var existingImagenItem = await _imagenItemData.GetByIdAsync(imagenItemDto.Id);
                if (existingImagenItem == null)
                {
                    throw new EntityNotFoundException("ImagenItem", imagenItemDto.Id);
                }

                // Actualizar propiedades
                existingImagenItem.Id = imagenItemDto.Id;
                existingImagenItem.ItemId = imagenItemDto.ItemId;
                existingImagenItem.UrlImage = imagenItemDto.UrlImage;
                existingImagenItem.DateRegistry = imagenItemDto.DateRegistry;
                existingImagenItem.Active = imagenItemDto.Active;

                return await _imagenItemData.UpdateAsync(existingImagenItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el imagenItem con ID: {ImagenItemId}", imagenItemDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el imagenItem.", ex);
            }
        }

        // Método para eliminar un ImagenItemulario Logicamente 
        public async Task<bool> DeleteImagenItemLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ImagenItemId", "El ImagenItemId debe ser mayor a 0");

            try
            {
                var existingImagenItem = await _imagenItemData.GetByIdAsync(id);
                if (existingImagenItem == null)
                    throw new EntityNotFoundException("ImagenItemUser", id);

                return await _imagenItemData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el ImagenItemUserDto con ID {ImagenItemUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el ImagenItemUser", ex);
            }
        }

        //  Método para eliminar un ImagenItemulario de manera persistente 
        public async Task<bool> DeleteImagenItemPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ImagenItemId", "El ImagenItemId debe ser mayor a 0");

            try
            {
                var existingImagenItem = await _imagenItemData.GetByIdAsync(id);
                if (existingImagenItem == null)
                    throw new EntityNotFoundException("ImagenItem", id);

                return await _imagenItemData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el ImagenItemUser con ID {ImagenItemUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el ImagenItemUser", ex);
            }
        }


        // Validación de DTO
        //private void ValidateImagenItem(ImagenItemDto imagenItemDto)
        //{
        //    if (imagenItemDto == null)
        //    {
        //        throw new ValidationException("El objeto ImagenItem no puede ser nulo");
        //    }

        //    if (imagenItemDto.Id <= 0)
        //    {
        //        _logger.LogWarning("FormId inválido: {FormId}", imagenItemDto.Id);
        //        throw new ValidationException("FormId", "El FormId debe ser mayor a cero");
        //    }

        //    if (imagenItemDto.ItemId <= 0)
        //    {
        //        _logger.LogWarning("ModuleId inválido: {ModuleId}", imagenItemDto.ItemId);
        //        throw new ValidationException("ModuleId", "El ModuleId debe ser mayor a cero");
        //    }
        //}
        private void ValidateImagenItem(ImagenItemDto imagenItemDto)
        {
            if (imagenItemDto == null)
            {
                throw new ValidationException("El objeto ImagenItem no puede ser nulo");
            }

            if (imagenItemDto.ItemId <= 0)
            {
                _logger.LogWarning("ItemId inválido: {ItemId}", imagenItemDto.ItemId);
                throw new ValidationException("ItemId", "El ItemId debe ser mayor a cero");
            }

            if (string.IsNullOrWhiteSpace(imagenItemDto.UrlImage))
            {
                _logger.LogWarning("Se intentó crear/actualizar un ImagenItem con UrlImage vacío");
                throw new ValidationException("UrlImage", "La UrlImage del ImagenItem es obligatoria");
            }

            
        }


        // Método para mapear de ImagenItem a ImagenItemDTO
        private ImagenItemDto MapToDTO(ImagenItem imagenItem)

        {
            return new ImagenItemDto
            {
                Id = imagenItem.Id,
                ItemId = imagenItem.ItemId,
                UrlImage = imagenItem.UrlImage,
                DateRegistry = imagenItem.DateRegistry,
                Active = imagenItem.Active,
            };
        }

        // Método para mapear de ImagenItemDTO a ImagenItem
        private ImagenItem MapToEntity(ImagenItemDto imagenItemDTO)
        {
            return new ImagenItem
            {
                Id = imagenItemDTO.Id,
                ItemId = imagenItemDTO.ItemId,
                UrlImage = imagenItemDTO.UrlImage,
                DateRegistry = imagenItemDTO.DateRegistry,
                Active = imagenItemDTO.Active,
            };
        }

        // Método para mapear una lista de ImagenItem a una lista de ImagenItemDTO
        private IEnumerable<ImagenItemDto> MapToDTOList(IEnumerable<ImagenItem> imagenItemes)
        {
            var imagenItemesDTO = new List<ImagenItemDto>();
            foreach (var imagenItem in imagenItemes)
            {
                imagenItemesDTO.Add(MapToDTO(imagenItem));
            }
            return imagenItemesDTO;
        }

    }
}

