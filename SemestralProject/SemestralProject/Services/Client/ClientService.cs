using System.Transactions;
using Microsoft.EntityFrameworkCore;
using SemestralProject.Data;
using SemestralProject.DTOs;
using SemestralProject.Models;

namespace SemestralProject.Services;

public class ClientService(ProjectContext context) : IClientService
{
    private ProjectContext Context { get; set; } = context;

    public async Task<bool> DoesPeselExist(string pesel)
    {
        return await Context.Persons
            .Include(e => e.IdClientNavigation)
            .FirstOrDefaultAsync(person => person.Pesel == pesel && person.IdClientNavigation.IsDeleted == "N") != null;
    }
    
    public async Task<bool> DoesKrsExist(string krs)
    {
        return await Context.Companies.Include(e => e.IdClientNavigation)
            .FirstOrDefaultAsync(company => company.Krs == krs && company.IdClientNavigation.IsDeleted == "N") != null;
    }

    public async Task<int> AddPerson(PersonAddClientDto personAdd)
    {
        if (personAdd.Pesel.Length != 11 || !personAdd.Pesel.All(char.IsDigit))
        {
            return -1;
        }

        int clientId;
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            Client client = new Client()
                { Email = personAdd.Email, Adress = personAdd.Adress, PhoneNumber = personAdd.PhoneNumber, IdDiscount = null, IsDeleted = "N"};

            Context.Clients.Add(client);
            await Context.SaveChangesAsync();

            clientId = client.IdClient;

            Context.Persons.Add(new Person()
            {
                IdClient = clientId,
                FirstName = personAdd.FirstName,
                LastName = personAdd.LastName,
                Pesel = personAdd.Pesel
            });
            
            await Context.SaveChangesAsync();
            scope.Complete();
        }

        return clientId;
    }

    public async Task<int> AddCompany(CompanyAddClientDto companyAdd)
    {
        if (companyAdd.KrsNumber.Length is not (9 or 14))
        {
            throw new Exception("Krs doesn't have 9 or 14 characters");
        }
        
        int clientId;
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            Client client = new Client()
                { Email = companyAdd.Email, Adress = companyAdd.Adress, PhoneNumber = companyAdd.PhoneNumber, IdDiscount = null, IsDeleted = "N"};

            Context.Clients.Add(client);
            await Context.SaveChangesAsync();

            clientId = client.IdClient;

            Context.Companies.Add(new Company()
            {
                IdClient = clientId,
                Krs = companyAdd.KrsNumber,
                Name = companyAdd.CompanyName
            });
            
            await Context.SaveChangesAsync();
            scope.Complete();
        }

        return clientId;
    }

    public async Task DeleteClient(int id)
    {
        var client = await Context.Clients.FirstOrDefaultAsync(client => client.IdClient == id && client.IsDeleted == "N");
        if (client != null)
        {
            client.IsDeleted = "Y";
            Context.Clients.Update(client);
        }
        await Context.SaveChangesAsync();
    }

    public async Task<bool> DoesClientExist(int id)
    {
        return await Context.Clients.FirstOrDefaultAsync(e => e.IdClient == id && e.IsDeleted == "N") != null;
    }

    public async Task<bool> DoesCompanyExist(int id)
    {
        return await Context.Companies.FindAsync(id) != null;
    }

    public async Task<bool> DoesPersonExist(int id)
    {
        return await Context.Persons.FindAsync(id) != null;
    }

    public async Task UpdateCompany(int id, CompanyUpdateDto company)
    {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            Company? com = await Context.Companies.FindAsync(id);
            Client? client = await Context.Clients.FindAsync(id);

            if (com != null && client != null && client.IsDeleted == "N")
            {
                com.Name = company.CompanyName;
                client.Adress = company.Adress;
                client.PhoneNumber = company.PhoneNumber;
                client.Email = company.Email;
                
                Context.Clients.Update(client);
                Context.Companies.Update(com);
            }
            await Context.SaveChangesAsync();
            scope.Complete();
        }
    }

    public async Task UpdatePerson(int id, PersonUpdateDto person)
    {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            Person? per = await Context.Persons.FindAsync(id);
            Client? client = await Context.Clients.FindAsync(id);

            if (per != null && client != null && client.IsDeleted == "N")
            {
                per.FirstName = person.FirstName;
                per.LastName = person.LastName;
                client.Adress = person.Adress;
                client.PhoneNumber = person.PhoneNumber;
                client.Email = person.Email;
                
                Context.Clients.Update(client);
                Context.Persons.Update(per);
            }
            await Context.SaveChangesAsync();
            scope.Complete();
        }
    }

    public async Task<ICollection<object>> GetCompanies()
    {
        var result = await Context.Clients
            .Include(client => client.Company)
            .Where(client => client.IsDeleted == "N" && client.Company != null)
            .Select(e => new
            {
                e.IdClient,
                e.IdDiscount,
                e.Adress,
                e.Email,
                e.PhoneNumber,
                CompanyName = e.Company.Name,
                e.Company.Krs
            })
            .ToListAsync();

        return result.Cast<object>().ToList();
    }


    public async Task<ICollection<object>> GetPersons()
    {
        var result = await Context.Clients
            .Include(client => client.Person)
            .Where(client => client.IsDeleted == "N" && client.Person != null)
            .Select(e => new
            {
                e.IdClient,
                e.IdDiscount,
                e.Adress,
                e.Email,
                e.PhoneNumber,
                e.Person.FirstName,
                e.Person.LastName,
                e.Person.Pesel
            })
            .ToListAsync();

        return result.Cast<object>().ToList();
    }
}