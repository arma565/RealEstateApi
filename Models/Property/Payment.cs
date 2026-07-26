namespace RealEstate.Models.Property
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid LeaseId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaidAt { get; set; }

        public PaymentType Type { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
