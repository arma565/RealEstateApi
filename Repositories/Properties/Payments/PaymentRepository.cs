using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Payments;

namespace RealEstate.Repositories.Properties.Payments;

interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetListAsync();
    Task<Payment?> GetAsync(Guid id);
    Task<Payment> AddAsync(Payment payment);
    Task UpdateAsync( Payment payment);
    Task DeleteAsync(Payment payment);
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

    public async Task<Payment> AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return payment;
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Payment payment)
    {
        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Payments.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
