using Microsoft.AspNetCore.Mvc;
using SemestralProject.DTOs;

namespace SemestralProject.Services;

public interface IClientService
{
    public Task<bool> DoesPeselExist(string pesel);
    public Task<bool> DoesKrsExist(string krs);
    Task<int> AddPerson(PersonAddClientDto personAdd);
    Task<int> AddCompany(CompanyAddClientDto companyAdd);
    Task DeleteClient(int id);
    Task<bool> DoesClientExist(int id);
    Task<bool> DoesCompanyExist(int id);
    Task<bool> DoesPersonExist(int id);
    Task UpdateCompany(int id, CompanyUpdateDto company);
    Task UpdatePerson(int id, PersonUpdateDto person);
    Task<ICollection<object>> GetCompanies();
    Task<ICollection<object>> GetPersons();
}