using RealEstate.Entities.Properties.Leases.Payments;
using RealEstate.Repositories.Properties.Leases.Payments;

namespace RealEstate.Services.Properties.Leases.Payments;

interface IPaymentService
{
    Task<IEnumerable<Payment>> GetListAsync();
    Task<Payment?> GetAsync(Guid id);
    Task<Payment> AddAsync(PaymentDTO paymentDTO);
    Task UpdateAsync(Guid id, PaymentDTO paymentDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PaymentService(PaymentRepository repository) : IPaymentService
{
    private readonly PaymentRepository _repository = repository;

    public async Task<IEnumerable<Payment>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Payment?> GetAsync(Guid id) =>
         await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<Payment> AddAsync(PaymentDTO paymentDTO)
    {

        ArgumentNullException.ThrowIfNull(paymentDTO);

        var payment = new Payment
        {
            Amount = paymentDTO.Amount,
            PaidAt = paymentDTO.PaidAt,
            PaymentType = paymentDTO.PaymentType,
            PaymentStatus = paymentDTO.PaymentStatus,
            LeaseId = paymentDTO.LeaseId
        };

        return await _repository.AddAsync(payment).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id, PaymentDTO paymentDTO)
    {
        ArgumentNullException.ThrowIfNull(paymentDTO);

        var payment = new Payment
        {
            Id = id,
            Amount = paymentDTO.Amount,
            PaidAt = paymentDTO.PaidAt,
            PaymentType = paymentDTO.PaymentType,
            PaymentStatus = paymentDTO.PaymentStatus,
            LeaseId = paymentDTO.LeaseId
        };

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
