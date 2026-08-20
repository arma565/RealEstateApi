using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Leases.Payments;

namespace RealEstate.Repositories.Properties.Leases.Payments;

#pragma warning disable CA1515
public class PaymentRepository<TPayment>(AppDbContext context) : BaseRepository<Payment>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<Payment>> GetListAsync() =>
     await _context
        .Payments
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public override async Task<Payment?> GetAsync(Guid id) =>
    await _context
       .Payments
       .AsNoTracking()
       .SingleOrDefaultAsync(payment => payment.Id == id)
       .ConfigureAwait(false);

    public override async Task<Payment> AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return payment;
    }

    public override async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(Payment payment)
    {
        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.Payments.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
