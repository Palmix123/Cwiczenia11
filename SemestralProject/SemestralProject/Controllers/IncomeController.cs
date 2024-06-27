using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestralProject.Services;
using SemestralProject.Services.ExchangeRate;

namespace SemestralProject.Controllers;

[ApiController]
public class IncomeController(IIncomeService service) : ControllerBase
{
    private IIncomeService Service { get; set; } = service;

    [Authorize(Roles = "admin")]
    [HttpGet]
    [Route("api/income/current")]
    public async Task<IActionResult> GetCurrentIncome(string countryCode = "PLN")
    {
        var income = await Service.CalculateCurrentIncome();
        if(countryCode != "PLN"){
            ExchangeRateService ers = new ExchangeRateService();
            decimal exchangeRate;
            try
            {
                exchangeRate = await ers.GetExchangeRate(countryCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            income *= exchangeRate;
        }
        return Ok($"Current income is: {income} {countryCode.ToUpper()}");
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet]
    [Route("api/income/possible")]
    public async Task<IActionResult> GetPossibleIncome(string countryCode = "PLN")
    {
        var income = await Service.CalculatePossibleIncome();
        
        if(countryCode != "PLN"){
            ExchangeRateService ers = new ExchangeRateService();
            decimal exchangeRate;
            try
            {
                exchangeRate = await ers.GetExchangeRate(countryCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            income *= exchangeRate;
        }
        
        return Ok($"Possible income is: {income} {countryCode.ToUpper()}");
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet]
    [Route("api/income/payments")]
    public async Task<IActionResult> GetIncomeFromPayments(string countryCode = "PLN")
    {
        var income = await Service.CalculateIncomeFromCurrentPayments();
        
        if(countryCode != "PLN"){
            ExchangeRateService ers = new ExchangeRateService();
            decimal exchangeRate;
            try
            {
                exchangeRate = await ers.GetExchangeRate(countryCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            income *= exchangeRate;
        }
        
        return Ok($"The sum of profit realized and payments not included in income is: {income} {countryCode.ToUpper()}");
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet]
    [Route("api/income/{idSoftware:int}")]
    public async Task<IActionResult> GetSoftwareIncome(int idSoftware, string countryCode = "PLN")
    {
        var income = await Service.CalculateSoftwareIncome(idSoftware);
        
        if(countryCode != "PLN"){
            ExchangeRateService ers = new ExchangeRateService();
            decimal exchangeRate;
            try
            {
                exchangeRate = await ers.GetExchangeRate(countryCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            income *= exchangeRate;
        }
        
        return Ok($"Income from software with id: {idSoftware} is: {income} {countryCode.ToUpper()}");
    }
}