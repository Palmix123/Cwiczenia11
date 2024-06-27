using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestralProject.DTOs;

using SemestralProject.Services;

namespace SemestralProject.Controllers;

[ApiController]
public class ClientController(IClientService service) : ControllerBase
{
    private IClientService Service { get; set; } = service;

    [Authorize(Roles = "admin")]
    [HttpPost]
    [Route("api/person/client")]
    public async Task<IActionResult> AddPersonClient(PersonAddClientDto personAdd)
    {
        if (await Service.DoesPeselExist(personAdd.Pesel))
        {
            return BadRequest("Something went wrong");
        }
        int clientId = await Service.AddPerson(personAdd);
        return clientId == -1 ? BadRequest("Pesel doesn't have 11 characters or contains non digit characters") : Ok($"Person client created with id: {clientId}");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [Route("api/company/client")]
    public async Task<IActionResult> AddCompanyClient(CompanyAddClientDto companyAdd)
    {
        if (await Service.DoesKrsExist(companyAdd.KrsNumber))
        {
            return BadRequest("Something went wrong");
        }
        int clientId = await Service.AddCompany(companyAdd);
        return Ok($"Company client created with id: {clientId}");
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete]
    [Route("api/client/{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        if (!await Service.DoesClientExist(id))
        {
            return NotFound($"Client with id: {id} doesn't exist or has already been deleted");
        }
        await Service.DeleteClient(id);
        return Ok($"Client with id: {id} was successfully deleted");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut]
    [Route("api/company/{id:int}")]
    public async Task<IActionResult> UpdateCompany(int id, CompanyUpdateDto company)
    {
        if (!await Service.DoesClientExist(id))
        {
            return NotFound($"Client with id: {id} doesn't exist or has already been deleted");
        }
        if (!await Service.DoesCompanyExist(id))
        {
            return NotFound($"Company with id: {id} doesn't exist");
        }
        await Service.UpdateCompany(id, company);
        return Ok($"Client with id: {id} was successfully deleted");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut]
    [Route("api/person/{id:int}")]
    public async Task<IActionResult> UpdatePerson(int id, PersonUpdateDto person)
    {
        if (!await Service.DoesClientExist(id))
        {
            return NotFound($"Client with id: {id} doesn't exist or has already been deleted");
        }
        if (!await Service.DoesPersonExist(id))
        {
            return NotFound($"Person with id: {id} doesn't exist");
        }
        await Service.UpdatePerson(id, person);
        return Ok($"Client with id: {id} was successfully deleted");
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("api/persons")]
    public async Task<IActionResult> GetPersons()
    {
        ICollection<object> clients = await Service.GetPersons();
        return Ok(clients);
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("api/companies")]
    public async Task<IActionResult> GetCompanies()
    {
        ICollection<object> clients = await Service.GetCompanies();
        return Ok(clients);
    }
}