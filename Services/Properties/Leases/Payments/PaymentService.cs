using RealEstate.DTOs.Properties.Leases.Payments;
using RealEstate.Entities.Properties.Leases.Payments;
using RealEstate.Repositories.Properties.Leases.Payments;

namespace RealEstate.Services.Properties.Leases.Payments;

interface IPaymentService
{
    Task<IEnumerable<Payment>> GetListAsync();
    Task<Payment> GetAsync(Guid id);
    Task<Payment> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PaymentService(PaymentRepository<Payment> repository) : IPaymentService
{
    private readonly PaymentRepository<Payment> _repository = repository;

    public async Task<IEnumerable<Payment>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Payment> GetAsync(Guid id)
    {
        var payment = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(payment);

        return payment;
    }

    public async Task<Payment> AddAsync(CreateDTO createDTO)
    {

        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new Payment
        {
            Amount = createDTO.Amount,
            PaymentType = createDTO.PaymentType,
            PaymentStatus = createDTO.PaymentStatus,
            LeaseId = createDTO.LeaseId
        }).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO)
    {
        var payment = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(payment);

        ArgumentNullException.ThrowIfNull(updateDTO);

        payment.Amount = updateDTO.Amount != payment.Amount ? updateDTO.Amount : payment.Amount;
        payment.PaymentType = updateDTO.PaymentType;
        payment.PaymentStatus = updateDTO.PaymentStatus;
        payment.LeaseId = updateDTO.LeaseId != payment.LeaseId ? updateDTO.LeaseId : payment.LeaseId;

        await _repository.UpdateAsync(payment).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var payment = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(payment);

        await _repository.DeleteAsync(payment).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
         await _repository.DeleteAllAsync().ConfigureAwait(false);
}
