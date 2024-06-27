using System.Transactions;
using Microsoft.EntityFrameworkCore;
using SemestralProject.Data;
using SemestralProject.DTOs.ContractDTOs;
using SemestralProject.Models;

namespace SemestralProject.Services;

public class ContractService(ProjectContext context) : IContractService
{
    private ProjectContext Context { get; set; } = context;
    public async Task<decimal> CalculatePrice(ContractAddDto contract)
    {
        var client = await Context.Clients
            .Include(e => e.IdDiscountNavigation)
            .FirstOrDefaultAsync(e => e.IdDiscount != null && e.IdClient == contract.IdClient);
            

        Decimal maxDiscount = 0;
        if (client != null && client.IdDiscountNavigation != null)
        {
            maxDiscount = client.IdDiscountNavigation.Value;
        }

        var software = await Context.Softwares
            .Where(e => e.Id == contract.IdSoftware)
            .Include(e => e.IdDiscounts)
            .FirstOrDefaultAsync();

        DateTime now = DateTime.Now;
        if (software != null)
        {
            foreach (var discount in software.IdDiscounts)
            {
                if (maxDiscount < discount.Value && discount.DateFrom < now && discount.DateTo > now && discount.Type == "P") // type P = singleSale
                {
                    maxDiscount = discount.Value;
                }
            }
        }
        else
        {
            throw new Exception();
        }

        Decimal result = software.Price * (1 - maxDiscount) +
                         (int)Enums.CurrentPaymentForOneMoreYear * contract.ExtraYearsForUpdates;

        return result;
    }

    public async Task<bool> DoesSoftwareExist(int contractIdSoftware)
    {
        return await Context.Softwares.FindAsync(contractIdSoftware) != null;
    }

    public async Task<object> GetSoftwareVersion(int contractIdSoftware)
    {
        var software = await Context.Softwares.FindAsync(contractIdSoftware);
        if (software != null)
        {
            return software.SoftwareVersion;
        }
        throw new Exception();
    }

    public async Task<bool> HasActiveSubscriptionOrContract(int contractIdClient, int contractIdSoftware)
    {
        var element = await Context
            .SingleSales.FirstOrDefaultAsync(e => e.IdClient == contractIdClient
                                                  && e.IsSigned == "Y"
                                                  && DateTime.Now > e.CreatedAt
                                                  && DateTime.Now < e.CreatedAt.AddYears(1 + e.ExtraYearsForUpdates));
        // trzeba by to inaczej zrobic uwzgledaniajc date rozpoczenia i date zakonczenia
        /*var element2 = await Context
            .SubscriptionSales.FirstOrDefaultAsync(e => e.IdClient == contractIdClient);*/

        return element != null;
    }

    public async Task AddSaleAndObligatoryPayments(ContractAddDto contract)
    {
        Decimal price = await CalculatePrice(contract);
        var version = await GetSoftwareVersion(contract.IdSoftware);
        
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {

            DateTime now = DateTime.Now;
            DateTime expire = contract.ExpireDate;
            SingleSale singleSale = new SingleSale()
            {
                IdClient = contract.IdClient,
                IdSoftware = contract.IdSoftware,
                SoftwareVersion = (string)version,
                CreatedAt = now,
                ExpireDate = expire,
                IsSigned = "N",
                Description = contract.Description,
                Price = price,
                ExtraYearsForUpdates = contract.ExtraYearsForUpdates,
                UpdatesInfo = contract.UpdatesInfo,
                NumberOfRates = contract.NumberOfRates,
                EndOfSoftware = now.AddYears(contract.ExtraYearsForUpdates)
            };

            await Context.SingleSales.AddAsync(singleSale);
            await Context.SaveChangesAsync();

            Decimal currentValue = 0;
            for (int i = 0; i < contract.NumberOfRates; i++)
            {
                decimal value = price / contract.NumberOfRates;
                decimal roundedValue = Math.Floor(value * 100) / 100;
                if (i + 1 == contract.NumberOfRates)
                {
                    roundedValue = price - currentValue;
                }
                currentValue += roundedValue;
                Payment payment = new Payment()
                {
                    Date = null,
                    IdSale = singleSale.IdSale,
                    Value = roundedValue
                };

                await Context.Payments.AddAsync(payment);
                await Context.SaveChangesAsync();
            }
            
            scope.Complete();
        }
    }

    public async Task<object?> GetContract(int idClient)
    {
        return await Context.SingleSales.Where(e => e.IdClient == idClient)
            .Select(e => new
            {
                e.IdSale,
                e.IdClient,
                e.IdSoftware,
                e.SoftwareVersion,
                e.CreatedAt,
                e.ExpireDate,
                e.IsSigned,
                e.Description,
                e.Price,
                e.ExtraYearsForUpdates,
                e.UpdatesInfo,
                e.NumberOfRates,
                e.EndOfSoftware
            }).ToListAsync();
    }
}