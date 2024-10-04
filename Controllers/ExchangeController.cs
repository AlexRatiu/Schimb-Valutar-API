using Schimb_Valutar_API.Models;
using Schimb_Valutar_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Schimb_Valutar_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public ExchangeController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddCurrency([FromBody] CurrencyModel currency)
        {
            if (currency == null)
            {
                return BadRequest("Currency model is null");
            }

            await _currencyService.CreateCurrencyAsync(currency);
            return CreatedAtAction(nameof(GetAllCurrencies), new { id = currency.Id }, currency);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<CurrencyModel>>> GetAllCurrencies()
        {
            var currencies = await _currencyService.GetAllCurrenciesAsync();
            return Ok(currencies);
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> UpdateCurrency([FromBody] CurrencyModel currency)
        {
            if (currency == null || currency.Id <= 0)
            {
                return BadRequest("Invalid currency model");
            }

            await _currencyService.UpdateCurrencyAsync(currency);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateCurrenciesFromBnr()
        {
            try
            {
                await _currencyService.UpdateAllCurrenciesFromBnrAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating currencies: {ex.Message}");
            }
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string fromCurrency, [FromQuery] string toCurrency, [FromQuery] decimal amount)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(fromCurrency) || string.IsNullOrWhiteSpace(toCurrency) || amount <= 0)
            {
                // Return 400 Bad Request response with error message
                return new ObjectResult("Invalid parameters.")
                {
                    StatusCode = 400 // Set HTTP status code to 400
                };
            }

            try
            {
                // Attempt currency conversion using the service
                var convertedAmount = await _currencyService.ConvertCurrencyAsync(fromCurrency.ToUpper(), toCurrency.ToUpper(), amount);

                // Return 200 OK response with the converted amount
                return new ObjectResult(convertedAmount)
                {
                    StatusCode = 200 // Set HTTP status code to 200
                };
            }
            catch (Exception ex)
            {
                // Return 400 Bad Request response with the exception message
                return new ObjectResult(ex.Message)
                {
                    StatusCode = 400 // Set HTTP status code to 400
                };
            }
        }
    }
}
