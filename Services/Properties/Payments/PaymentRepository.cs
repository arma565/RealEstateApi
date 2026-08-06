using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Payments;

namespace RealEstate.Repositories.Properties.Payments;

interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetListAsync();
    Task<Payment?> GetAsync(Guid id);
    Task<Payment> AddAsync(PaymentDTO paymentDTO);
    Task UpdateAsync(Guid id , PaymentDTO paymentDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Payment>> GetListAsync() =>
     await _context
        .Payments
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<Payment?> GetAsync(Guid id) =>
    await _context
       .Payments
       .AsNoTracking()
       .SingleOrDefaultAsync(payment => payment.Id == id)
       .ConfigureAwait(false);

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

        await _context.Payments.AddAsync(payment).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return payment;
    }

    public async Task UpdateAsync(Guid id , PaymentDTO paymentDTO)
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

        _context.Payments.Update(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var payment = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(payment);

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Payments.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
