using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Payments;
using RealEstate.Services.Repositories.Properties.Payments;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Payments;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class PaymentController(PaymentRepository service, ILogger logger) : ControllerBase
{
    private readonly PaymentRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<IEnumerable<Payment>> GetListAsync() => [.. await _service.GetListAsync().ConfigureAwait(false)];

    [HttpGet("/{paymentId}")]
    public async Task<ActionResult<Payment>> GetAsync(string paymentId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return BadRequest("PaymentId is empty!");

            if (!Guid.TryParse(paymentId, out Guid realEstatePaymentId))
                return BadRequest("PaymentId must be a valid GUID!");

            var payment = await _service.GetAsync(realEstatePaymentId).ConfigureAwait(false);

            return payment == null ? NotFound() : Ok(payment);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] PaymentDTO paymentDTO)
    {
        try
        {
            if (paymentDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = new Payment
            {
                Amount = paymentDTO.Amount,
                PaidAt = paymentDTO.PaidAt,
                PaymentType = paymentDTO.PaymentType,
                PaymentStatus = paymentDTO.PaymentStatus,
                LeaseId = paymentDTO.LeaseId
            };

            await _service.AddAsync(payment).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { paymentId = payment.Id }, payment);
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpPut("update/{paymentId}")]
    public async Task<IActionResult> UpdateAsync([FromBody] PaymentDTO paymentDTO, string paymentId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return BadRequest("PaymentId is empty!");

            if (!Guid.TryParse(paymentId, out Guid realEstatePaymentId))
                return BadRequest("PaymentId must be a valid GUID!");

            if (paymentDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPayment = await _service.IsPaymentExistAsync(realEstatePaymentId).ConfigureAwait(false);

            if (!existingPayment)
                return NotFound("Payment not found!");

            var payment = new Payment
            {
                Id = realEstatePaymentId,
                Amount = paymentDTO.Amount,
                PaidAt = paymentDTO.PaidAt,
                PaymentType = paymentDTO.PaymentType,
                PaymentStatus = paymentDTO.PaymentStatus,
                LeaseId = paymentDTO.LeaseId
            };

            await _service.UpdateAsync(payment).ConfigureAwait(false);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpDelete("delete/{paymentId}")]
    public async Task<IActionResult> DeleteAsync(string paymentId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return BadRequest("PaymentId is empty!");

            if (!Guid.TryParse(paymentId, out Guid realEstatePaymentId))
                return BadRequest("PaymentId must be a valid GUID!");

            var existingPayment = await _service.IsPaymentExistAsync(realEstatePaymentId).ConfigureAwait(false);

            if (!existingPayment)
                return NotFound("Payment not found!");

            await _service.DeleteAsync(realEstatePaymentId).ConfigureAwait(false);

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }
}
