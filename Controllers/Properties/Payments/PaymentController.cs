using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Payments;
using RealEstate.Services.Repositories.Properties.Payments;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Payments;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class PaymentController(PaymentRepository service, ILogger<PaymentController> logger) : ControllerBase
{
    private readonly PaymentRepository _service = service;

    private readonly ILogger<PaymentController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetListAsync() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{paymentId}")]
    public async Task<ActionResult<Payment>> GetAsync(string paymentId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return BadRequest("PaymentId is empty!");

            if (!Guid.TryParse(paymentId, out Guid id))
                return BadRequest("PaymentId must be a valid GUID!");

            var payment = await _service.GetAsync(id).ConfigureAwait(false);

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

           var payment = await _service.AddAsync(paymentDTO).ConfigureAwait(false);

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
    public async Task<IActionResult> UpdateAsync(string paymentId, [FromBody] PaymentDTO paymentDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return BadRequest("PaymentId is empty!");

            if (!Guid.TryParse(paymentId, out Guid id))
                return BadRequest("PaymentId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id,paymentDTO).ConfigureAwait(false);

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

            if (!Guid.TryParse(paymentId, out Guid id))
                return BadRequest("PaymentId must be a valid GUID!");

            await _service.DeleteAsync(id).ConfigureAwait(false);

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

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllAsync()
    {
        try
        {
            await _service.DeleteAllAsync().ConfigureAwait(false);
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
