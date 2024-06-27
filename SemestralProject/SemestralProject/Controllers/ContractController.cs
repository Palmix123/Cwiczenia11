using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestralProject.DTOs.ContractDTOs;
using SemestralProject.Services;

namespace SemestralProject.Controllers;

[ApiController]
public class ContractController(IContractService service, IClientService clientService) : ControllerBase
{
    private IContractService Service { get; set; } = service;
    private IClientService ClientService { get; set; } = clientService;
    
    [AllowAnonymous]
    [HttpPost]
    [Route("api/contract")]
    public async Task<IActionResult> AddPersonClient(ContractAddDto contract)
    {
        if (await Service.HasActiveSubscriptionOrContract(contract.IdClient, contract.IdSoftware)) 
        { // zrobione jest narazie ze subskrypcja nie ma daty rozpoczecia ani daty zakonczenia
            return BadRequest(
                $"Client with id: {contract.IdClient} has active subscription or contact on that product");
        }
        
        if (!await ClientService.DoesClientExist(contract.IdClient))
        {
            return NotFound($"Client with id: {contract.IdClient} doesn't exist or has already been deleted");
        }
        
        if (!await Service.DoesSoftwareExist(contract.IdSoftware))
        {
            return NotFound($"Software with id: {contract.IdSoftware} doesn't exist");
        }
        
        if (contract.ExpireDate < DateTime.Now.AddDays(3) || contract.ExpireDate > DateTime.Now.AddDays(30)){
            return BadRequest("Expire date must be in range from 3 days to 30 days from now");
        }
        
        if (contract.ExtraYearsForUpdates > 3 || contract.ExtraYearsForUpdates < 0){
            return BadRequest("ExtraYearsForUpdate must be in range from 0 to 3 included");
        }

        if (contract.NumberOfRates <= 0)
        {
            return BadRequest("NumberOfRates should be at least 1");
        }
        
        if (contract.NumberOfRates > 10)
        {
            return BadRequest("NumberOfRates should be 10 or lower");
        }

        await Service.AddSaleAndObligatoryPayments(contract);

        return Ok();
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("api/contract/{idClient:int}")]
    public async Task<IActionResult> ShowClientContract(int idClient)
    {
        if (!await ClientService.DoesClientExist(idClient))
        {
            return NotFound($"Client with id: {idClient} doesn't exist or has already been deleted");
        }
        return Ok(await Service.GetContract(idClient));
    }
}