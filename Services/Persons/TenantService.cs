using RealEstate.DTOs.Persons;
using RealEstate.Entities.Persons.Owners;
using RealEstate.Entities.Persons.Tenants;
using RealEstate.Repositories.Persons;

namespace RealEstate.Services.Persons;

interface ITenantService
{
    Task<IEnumerable<Tenant>> GetListAsync();
    Task<Tenant> GetAsync(Guid id);
    Task<Tenant> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(UpdateDTO updateDTO, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class TenantService(TenantRepository<Tenant> repository) : ITenantService
{
    private readonly TenantRepository<Tenant> _repository = repository;

    public async Task<IEnumerable<Tenant>> GetListAsync() =>
     await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Tenant> GetAsync(Guid id) {
        var tenant = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(tenant);
        return tenant;
    }

    public async Task<Tenant> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new Tenant
        {
            FirstName = createDTO.FirstName,
            LastName = createDTO.LastName,
            FatherName = createDTO.FatherName,
            BirthCertificateNumber = createDTO.BirthCertificateNumber,
            BirthCertificateIssued = createDTO.BirthCertificateIssued,
            NationalId = createDTO.NationalId,
            Born = createDTO.Born,
            Phone = createDTO.Phone,
            Address = createDTO.Address
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(UpdateDTO updateDTO, Guid id)
    {

        var tenant = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(tenant);

        ArgumentNullException.ThrowIfNull(updateDTO);

        tenant.FirstName = string.IsNullOrEmpty(updateDTO.FirstName) ? tenant.FirstName : updateDTO.FirstName;
        tenant.LastName = string.IsNullOrEmpty(updateDTO.LastName) ? tenant.LastName : updateDTO.LastName;
        tenant.FatherName = string.IsNullOrEmpty(updateDTO.FatherName) ? tenant.FatherName : updateDTO.FatherName;
        tenant.BirthCertificateNumber = updateDTO.BirthCertificateNumber != tenant.BirthCertificateNumber ? updateDTO.BirthCertificateNumber : tenant.BirthCertificateNumber;
        tenant.BirthCertificateIssued = string.IsNullOrEmpty(updateDTO.BirthCertificateIssued) ? tenant.BirthCertificateIssued : updateDTO.BirthCertificateIssued;
        tenant.NationalId = updateDTO.NationalId != tenant.NationalId ? updateDTO.NationalId : tenant.NationalId;
        tenant.Born = string.IsNullOrEmpty(updateDTO.Born) ? tenant.Born : updateDTO.Born;
        tenant.Phone = string.IsNullOrEmpty(updateDTO.Phone) ? tenant.Phone : updateDTO.Phone;
        tenant.Address = string.IsNullOrEmpty(updateDTO.Address) ? tenant.Address : updateDTO.Address;

        await _repository.UpdateAsync(tenant).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenant = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(tenant);

        await _repository.DeleteAsync(tenant).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);

}



