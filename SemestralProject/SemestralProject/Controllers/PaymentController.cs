using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestralProject.Services;

namespace SemestralProject.Controllers;

[ApiController]
public class PaymentController(IPaymentService service, IClientService clientService) : ControllerBase
{
    private IPaymentService Service { get; set; } = service;
    private IClientService ClientService { get; set; } = clientService;
    
    [AllowAnonymous]
    [HttpGet]
    [Route("api/payments/{idClient:int}")]
    public async Task<IActionResult> ShowClientPayments(int idClient, int? idSale = null)
    {
        if (idSale != null)
        {
            int sale = (int)idSale;
            if (!await Service.DoesSaleExist(sale))
            {
                return NotFound($"Sale with id: {idSale} doesn't exist");
            }
        }
        if (!await ClientService.DoesClientExist(idClient))
        {
            return NotFound($"Client with id: {idClient} doesn't exist or has already been deleted");
        }
        return Ok(await Service.GetAllClientPayments(idClient, idSale));
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("api/payments/unrealized/{idClient:int}")]
    public async Task<IActionResult> ShowClientUnrealizedPayments(int idClient, int? idSale = null)
    {
        if (idSale != null)
        {
            int sale = (int)idSale;
            if (!await Service.DoesSaleExist(sale))
            {
                return NotFound($"Sale with id: {idSale} doesn't exist");
            }
        }
        if (!await ClientService.DoesClientExist(idClient))
        {
            return NotFound($"Client with id: {idClient} doesn't exist or has already been deleted");
        }
        return Ok(await Service.GetUnrealizedClientPayments(idClient, idSale));
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("api/payment/implementation/{idSale:int}/{amount:Decimal}")]
    public async Task<IActionResult> PayForContract(int idSale, Decimal amount)
    {
        if (!await Service.DoesSaleExist(idSale))
        {
            return NotFound($"Sale with id: {idSale} doesn't exist");
        }
        
        if (await Service.IsSaleSigned(idSale))
        {
            return BadRequest($"Sale with id: {idSale} has already been signed");
        }
        
        if (await Service.IsSaleExpired(idSale))
        {
            return NotFound($"Sale with id: {idSale} has already been expired");
        }
        
        if (!await Service.DoesAmountCorrelatesToSomePayment(idSale, amount))
        {
            return BadRequest($"The amount: {amount} doesn't correlates to any payment required");
        }

        await Service.AddPaymentAndApplyPaymentLogic(idSale, amount); // todo
        
        return Ok();
    }
}