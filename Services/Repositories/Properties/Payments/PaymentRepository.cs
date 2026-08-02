using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Payments;

namespace RealEstate.Services.Repositories.Properties.Payments;

interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetListAsync();
    Task<Payment?> GetByIdAsync(Guid id);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(Guid id);
}

#pragma warning disable CA1515
public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Payment>> GetListAsync() =>
     await _context
        .Payments
        .AsNoTracking()
        .Include(payment => payment.Lease)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<Payment?> GetByIdAsync(Guid id) =>
    await _context
       .Payments
       .AsNoTracking()
       .Include(payment => payment.Lease)
       .SingleOrDefaultAsync(payment => payment.Id == id)
       .ConfigureAwait(false);

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id).ConfigureAwait(false);

        if (payment == null)
            ArgumentNullException.ThrowIfNull(payment);    

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
