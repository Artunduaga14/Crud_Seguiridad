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
    public class BranchBusiness
    {
        private readonly BranchData _branchData;
        private readonly ILogger<BranchBusiness> _logger;

        public BranchBusiness(BranchData branchData, ILogger<BranchBusiness> logger)
        {
            _branchData = branchData;
            _logger = logger;
        }

        // Método para obtener todos los branches como DTOs
        public async Task<IEnumerable<BranchDto>> GetAllBranchesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var branches = await _branchData.GetAllAsync();
                var branchesDTO = MapToDTOList(branches);

                return branchesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los branches");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de branches", ex);
            }
        }

        // Método para obtener un branch por ID como DTO
        public async Task<BranchDto> GetBranchByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un branch con ID inválido: {BranchId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del branch debe ser mayor que cero");
            }

            try
            {
                var branch = await _branchData.GetByIdAsync(id);
                if (branch == null)
                {
                    _logger.LogInformation("No se encontró ningún branch con ID: {BranchId}", id);
                    throw new EntityNotFoundException("Branch", id);
                }

                return MapToDTO(branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el branch con ID: {BranchId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el branch con ID {id}", ex);
            }
        }

        // Método para crear un branch desde un DTO
        public async Task<BranchDto> CreateBranchAsync(BranchDto BranchDto)
        {
            try
            {
                ValidateBranch(BranchDto);

                var branch = MapToEntity(BranchDto);

                var branchCreado = await _branchData.CreateAsync(branch);

                return MapToDTO(branchCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo branch: {BranchNombre}", BranchDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el branch", ex);
            }
        }

        // Método para actualizar un branch desde un DTO
        public async Task<bool> UpdateBranchAsync(BranchDto branchDto)
        {
            try
            {
                ValidateBranch(branchDto);

                var existingBranch = await _branchData.GetByIdAsync(branchDto.Id);
                if (existingBranch == null)
                {
                    throw new EntityNotFoundException("Branch", branchDto.Id);
                }

                // Actualizar propiedades
                existingBranch.Id = branchDto.Id;
                existingBranch.Name = branchDto.Name;
                existingBranch.Address = branchDto.Address;
                existingBranch.Phone = branchDto.Phone;
                existingBranch.Email = branchDto.Email;
                existingBranch.Incharge = branchDto.Incharge;
                existingBranch.Active = branchDto.Active;
                existingBranch.LocationFurrow = branchDto.LocationFurrow;
                existingBranch.CompanyId = branchDto.CompanyId;

                return await _branchData.UpdateAsync(existingBranch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el branch con ID: {BranchId}", branchDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el branch.", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteBranchLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("BranchId", "El BranchId debe ser mayor a 0");

            try
            {
                var existingBranch = await _branchData.GetByIdAsync(id);
                if (existingBranch == null)
                    throw new EntityNotFoundException("BranchUser", id);

                return await _branchData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el BranchUserDto con ID {BranchUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el BranchUser", ex);
            }
        }

        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteBranchPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("BranchId", "El BranchId debe ser mayor a 0");

            try
            {
                var existingBranch = await _branchData.GetByIdAsync(id);
                if (existingBranch == null)
                    throw new EntityNotFoundException("Branch", id);

                return await _branchData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Branch con ID {BranchId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el Branch", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateBranch(BranchDto branchDto)
        {
            if (branchDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto branch no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(branchDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un branch con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del branch es obligatorio");
            }
        }

        // Método para mapear de Branch a BranchDTO
        private BranchDto MapToDTO(Branch branch)

        {
            return new BranchDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Phone = branch.Phone,
                Email = branch.Email,
                Incharge = branch.Incharge,
                Active = branch.Active, // Si existe en la entidad
                LocationFurrow = branch.LocationFurrow,
                CompanyId = branch.CompanyId
            };
        }

        // Método para mapear de BranchDTO a Branch
        private Branch MapToEntity(BranchDto branchDTO)
        {
            return new Branch
            {
                Id = branchDTO.Id,
                Name = branchDTO.Name,
                Address = branchDTO.Address,
                Phone = branchDTO.Phone,
                Email = branchDTO.Email,
                Incharge = branchDTO.Incharge,
                Active = branchDTO.Active, // Si existe en la entidad
                LocationFurrow = branchDTO.LocationFurrow,
                CompanyId = branchDTO.CompanyId
               
            };
        }

        // Método para mapear una lista de Branch a una lista de BranchDTO
        private IEnumerable<BranchDto> MapToDTOList(IEnumerable<Branch> branchs)
        {
            var branchsDTO = new List<BranchDto>();
            foreach (var rol in branchs)
            {
                branchsDTO.Add(MapToDTO(rol));
            }
            return branchsDTO;
        }
    }
}
