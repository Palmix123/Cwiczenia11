using Microsoft.EntityFrameworkCore;
using SemestralProject.Data;
using SemestralProject.Services.ExchangeRate;

namespace SemestralProject.Services;

public class IncomeService(ProjectContext context) : IIncomeService
{
    private ProjectContext Context { get; set; } = context;
    private ExchangeRateService ExchangeRateService;

    public async Task<Decimal> CalculateCurrentIncome()
    {
        var currentIncome = await Context.SingleSales
            .Where(e => e.IsSigned == "Y")
            .SumAsync(e => e.Price);

        return currentIncome;
    }

    public async Task<decimal> CalculatePossibleIncome()
    {
        DateTime now = DateTime.Now;
        var currentIncome = await Context.SingleSales
            .Where(e => e.IsSigned == "Y" || (e.IsSigned == "N" && e.ExpireDate > now))
            .SumAsync(e => e.Price);

        return currentIncome;
    }

    public async Task<decimal> CalculateIncomeFromCurrentPayments()
    {
        DateTime now = DateTime.Now;

        var currentIncome = await CalculateCurrentIncome();
        var consumerPayments = await Context.Payments
            .Include(e => e.IdSaleNavigation)
            .Where(e => e.IdSaleNavigation.IsSigned == "N" 
                        && e.IdSaleNavigation.ExpireDate > now
                        && e.Date != null)
            .SumAsync(e => e.Value);

        return currentIncome + consumerPayments;
    }

    public async Task<decimal> CalculateSoftwareIncome(int idSoftware)
    {
        var currentIncome = await Context.SingleSales
            .Where(e => e.IsSigned == "Y" && e.IdSoftware == idSoftware)
            .SumAsync(e => e.Price);

        return currentIncome;
    }
}