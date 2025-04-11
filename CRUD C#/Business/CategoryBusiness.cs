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
    public class CategoryBusiness
    {
        private readonly CategoryData _categoryData;
        private readonly ILogger<CategoryBusiness> _logger;

        public CategoryBusiness(CategoryData categoryData, ILogger<CategoryBusiness> logger)
        {
            _categoryData = categoryData;
            _logger = logger;
        }

        // Método para obtener todos los categoryes como DTOs
        public async Task<IEnumerable<CategoryDto>> GetAllCategoryesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var categoryes = await _categoryData.GetAllAsync();
                var categoryesDTO = MapToDTOList(categoryes);

                return categoryesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los categoryes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de categoryes", ex);
            }
        }


        // Método para obtener un category por ID como DTO
        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un category con ID inválido: {CategoryId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del category debe ser mayor que cero");
            }

            try
            {
                var category = await _categoryData.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogInformation("No se encontró ningún category con ID: {CategoryId}", id);
                    throw new EntityNotFoundException("Category", id);
                }

                return MapToDTO(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el category con ID: {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el category con ID {id}", ex);
            }
        }

        // Método para crear un category desde un DTO
        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto CategoryDto)
        {
            try
            {
                ValidateCategory(CategoryDto);

                var category = MapToEntity(CategoryDto);

                var categoryCreado = await _categoryData.CreateAsync(category);

                return MapToDTO(categoryCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo category: {CategoryNombre}", CategoryDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el category", ex);
            }
        }


        // Método para actualizar un category desde un DTO
        public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                ValidateCategory(categoryDto);

                var existingCategory = await _categoryData.GetByIdAsync(categoryDto.Id);
                if (existingCategory == null)
                {
                    throw new EntityNotFoundException("Category", categoryDto.Id);
                }

                // Actualizar propiedades
                existingCategory.Id = categoryDto.Id;
                existingCategory.Name = categoryDto.Name;
                existingCategory.Description = categoryDto.Description;
                existingCategory.Active = categoryDto.Active;

                return await _categoryData.UpdateAsync(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el category con ID: {CategoryId}", categoryDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el category.", ex);
            }
        }



        // Método para eliminar un Caregoryulario Logicamente 
        public async Task<bool> DeleteCategoryLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("CategoryId", "El CategoryId debe ser mayor a 0");

            try
            {
                var existingCategory = await _categoryData.GetByIdAsync(id);
                if (existingCategory == null)
                    throw new EntityNotFoundException("CategoryUser", id);

                return await _categoryData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el CategoryUserDto con ID {CategoryUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el CategoryUser", ex);
            }
        }

        //  Método para eliminar un Caregoryulario de manera persistente 
        public async Task<bool> DeleteCategoryPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("CategoryId", "El CategoryId debe ser mayor a 0");

            try
            {
                var existingCaregory = await _categoryData.GetByIdAsync(id);
                if (existingCaregory == null)
                    throw new EntityNotFoundException("Caregory", id);

                return await _categoryData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el CategoryUser con ID {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el Category", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto category no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un category con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del category es obligatorio");
            }
        }



        // Método para mapear de Category a CategoryDTO
        private CategoryDto MapToDTO(Category category)

        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Active = category.Active
            };
        }

        // Método para mapear de CategoryDTO a Category
        private Category MapToEntity(CategoryDto categoryDTO)
        {
            return new Category
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                Active = categoryDTO.Active // Si existe en la entidad
            };
        }



        // Método para mapear una lista de Category a una lista de CategoryDTO
        private IEnumerable<CategoryDto> MapToDTOList(IEnumerable<Category> categorys)
        {
            var categorysDTO = new List<CategoryDto>();
            foreach (var category in categorys)
            {
                categorysDTO.Add(MapToDTO(category));
            }
            return categorysDTO;
        }
    }

}
