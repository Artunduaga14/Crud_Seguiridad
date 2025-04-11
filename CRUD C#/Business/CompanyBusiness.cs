using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class CompanyBusiness
    {

        private readonly CompanyData _companyData;
        private readonly ILogger<CompanyBusiness> _logger;

        public CompanyBusiness(CompanyData companyData, ILogger<CompanyBusiness> logger)
        {
            _companyData = companyData;
            _logger = logger;
        }

        // Método para obtener todos los companyes como DTOs
        public async Task<IEnumerable<CompanyDto>> GetAllCompanyesAsync()
        {
            try
            {

                //==============================================
                // Corrección 
                //==============================================

                var companys = await _companyData.GetAllAsync();
                var companysDTO = MapToDTOList(companys);

                return companysDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los companys");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de companys", ex);
            }
        }

        // Método para obtener un company por ID como DTO
        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un company con ID inválido: {CompanyId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del company debe ser mayor que cero");
            }

            try
            {
                var company = await _companyData.GetByIdAsync(id);
                if (company == null)
                {
                    _logger.LogInformation("No se encontró ningún company con ID: {CompanyId}", id);
                    throw new EntityNotFoundException("Company", id);
                }

                return MapToDTO(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el company con ID: {CompanyId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el company con ID {id}", ex);
            }
        }

        // Método para crear un company desde un DTO
        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto CompanyDto)
        {
            try
            {
                ValidateCompany(CompanyDto);

                var company = MapToEntity(CompanyDto);

                var companyCreado = await _companyData.CreateAsync(company);

                return MapToDTO(companyCreado);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo company: {CompanyNombre}", CompanyDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el company", ex);
            }
        }


        // Método para actualizar un company desde un DTO
        public async Task<bool> UpdateCompanyAsync(CompanyDto companyDto)
        {
            try
            {
                ValidateCompany(companyDto);

                var existingCompany = await _companyData.GetByIdAsync(companyDto.Id);
                if (existingCompany == null)
                {
                    throw new EntityNotFoundException("Company", companyDto.Id);
                }

                // Actualizar propiedades
                existingCompany.Id = companyDto.Id;
                existingCompany.Name = companyDto.Name;
                existingCompany.Address = companyDto.Address;
                existingCompany.Phone = companyDto.Phone;
                existingCompany.Email = companyDto.Email;
                existingCompany.Logo = companyDto.Logo;
                existingCompany.DataRegistry = companyDto.DataRegistry;
                existingCompany.Active = companyDto.Active;

                return await _companyData.UpdateAsync(existingCompany);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el company con ID: {CompanyId}", companyDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el company.", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteCompanyLogicalAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("CompanyId", "El CompanyId debe ser mayor a 0");

            try
            {
                var existingCompany = await _companyData.GetByIdAsync(id);
                if (existingCompany == null)
                    throw new EntityNotFoundException("CompanyUser", id);

                return await _companyData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el CompanyUserDto con ID {CompanyUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el CompanyUser", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteCompanyPersistentAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("CompanyId", "El CompanyId debe ser mayor a 0");

            try
            {
                var existingForm = await _companyData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _companyData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el CompanyUser con ID {CompanyUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el CompanyUser", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateCompany(CompanyDto CompanyDto)
        {
            if (CompanyDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto company no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(CompanyDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un company con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del company es obligatorio");
            }
        }

        // Método para mapear de Company a RolDTO
        private CompanyDto MapToDTO(Company company)

        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                Phone = company.Phone,
                Email = company.Email,
                Logo = company.Logo,
                Active = company.Active, // Si existe en la entidad
                DataRegistry = company.DataRegistry
            };
        }

        // Método para mapear de CompanyDTO a Company
        private Company MapToEntity(CompanyDto companyDTO)
        {
            return new Company
            {
                Id = companyDTO.Id,
                Name = companyDTO.Name,
                Address = companyDTO.Address,
                Phone = companyDTO.Phone,
                Email = companyDTO.Email,
                Logo = companyDTO.Logo,
                Active = companyDTO.Active, // Si existe en la entidad
                DataRegistry = companyDTO.DataRegistry
            };
        }

        // Método para mapear una lista de Company a una lista de CompanyDTO
        private IEnumerable<CompanyDto> MapToDTOList(IEnumerable<Company> companys)
        {
            var companysDTO = new List<CompanyDto>();
            foreach (var company in companys)
            {
                companysDTO.Add(MapToDTO(company));
            }
            return companysDTO;
        }
    }
}
