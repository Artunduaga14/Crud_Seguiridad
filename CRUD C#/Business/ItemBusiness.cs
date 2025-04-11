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
    public class ItemBusiness
    {
        private readonly ItemData _itemData;
        private readonly ILogger<ItemBusiness> _logger;

        public ItemBusiness(ItemData itemData, ILogger<ItemBusiness> logger)
        {
            _itemData = itemData;
            _logger = logger;
        }

        // Método para obtener todos los itemes como DTOs
        public async Task<IEnumerable<ItemDto>> GetAllItemesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var itemes = await _itemData.GetAllAsync();
                var itemesDTO = MapToDTOList(itemes);

                return itemesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los itemes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de itemes", ex);
            }
        }


        // Método para obtener un item por ID como DTO
        public async Task<ItemDto> GetItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un item con ID inválido: {ItemId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del item debe ser mayor que cero");
            }

            try
            {
                var item = await _itemData.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogInformation("No se encontró ningún item con ID: {ItemId}", id);
                    throw new EntityNotFoundException("Item", id);
                }

                return MapToDTO(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el item con ID: {ItemId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el item con ID {id}", ex);
            }
        }

        // Método para crear un item desde un DTO
        public async Task<ItemDto> CreateItemAsync(ItemDto ItemDto)
        {
            try
            {
                ValidateItem(ItemDto);

                var item = MapToEntity(ItemDto);

                var itemCreado = await _itemData.CreateAsync(item);

                return MapToDTO(itemCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo item: {ItemNombre}", ItemDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el item", ex);
            }
        }


        // Método para actualizar un item desde un DTO
        public async Task<bool> UpdateItemAsync(ItemDto itemDto)
        {
            try
            {
                ValidateItem(itemDto);

                var existingItem = await _itemData.GetByIdAsync(itemDto.Id);
                if (existingItem == null)
                {
                    throw new EntityNotFoundException("Item", itemDto.Id);
                }

                // Actualizar propiedades
                existingItem.Id = itemDto.Id;
                existingItem.Code = itemDto.Code;
                existingItem.CodeQr = itemDto.CodeQr;
                existingItem.Name = itemDto.Name;
                existingItem.Description = itemDto.Description;
                existingItem.Active = itemDto.Active;
                existingItem.CreatedAt = itemDto.CreatedAt;
                existingItem.CategoryId = itemDto.CategoryId;
                existingItem.ZoneId = itemDto.ZoneId;

                return await _itemData.UpdateAsync(existingItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el item con ID: {ItemId}", itemDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el item.", ex);
            }
        }

        // Método para eliminar un Itemulario Logicamente 
        public async Task<bool> DeleteItemLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ItemId", "El ItemId debe ser mayor a 0");

            try
            {
                var existingItem = await _itemData.GetByIdAsync(id);
                if (existingItem == null)
                    throw new EntityNotFoundException("ItemUser", id);

                return await _itemData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el ItemUserDto con ID {ItemUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el ItemUser", ex);
            }
        }


        //  Método para eliminar un Itemulario de manera persistente 
        public async Task<bool> DeleteItemPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ItemId", "El ItemId debe ser mayor a 0");

            try
            {
                var existingItem = await _itemData.GetByIdAsync(id);
                if (existingItem == null)
                    throw new EntityNotFoundException("Item", id);

                return await _itemData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el ItemUser con ID {ItemUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el ItemUser", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateItem(ItemDto ItemDto)
        {
            if (ItemDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto item no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ItemDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un item con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del item es obligatorio");
            }
        }


        // Método para mapear de Item a ItemDTO
        private ItemDto MapToDTO(Item item)

        {
            return new ItemDto
            {
                Id = item.Id,
                Code = item.Code,
                CodeQr = item.CodeQr,
                Name = item.Name,
                Description = item.Description,
                Active = item.Active,
                CreatedAt = item.CreatedAt,
                CategoryId = item.CategoryId,
                ZoneId = item.ZoneId,
            };
        }

        // Método para mapear de ItemDTO a Item
        private Item MapToEntity(ItemDto itemDTO)
        {
            return new Item
            {
                Id = itemDTO.Id,
                Code = itemDTO.Code,
                CodeQr = itemDTO.CodeQr,
                Name = itemDTO.Name,
                Description = itemDTO.Description,
                Active = itemDTO.Active,
                CreatedAt = itemDTO.CreatedAt,
                CategoryId = itemDTO.CategoryId,
                ZoneId = itemDTO.ZoneId,
            };

        }

        // Método para mapear una lista de Item a una lista de ItemDTO
        private IEnumerable<ItemDto> MapToDTOList(IEnumerable<Item> itemes)
        {
            var itemesDTO = new List<ItemDto>();
            foreach (var item in itemes)
            {
                itemesDTO.Add(MapToDTO(item));
            }
            return itemesDTO;
        }
    }
}
