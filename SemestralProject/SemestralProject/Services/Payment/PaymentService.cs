using System.Transactions;
using Microsoft.EntityFrameworkCore;
using SemestralProject.Data;

namespace SemestralProject.Services;

public class PaymentService(ProjectContext context) : IPaymentService
{
    private ProjectContext Context { get; set; } = context;
    public async Task<object?> GetAllClientPayments(int idClient, int ?idSale)
    {
        if (idSale == null)
        {
            return await Context.Payments.Include(e => e.IdSaleNavigation)
                .Where(e => e.IdSaleNavigation.IdClient == idClient)
                .Select(e => new
                {
                    e.IdSale,
                    e.Date,
                    e.Value
                }).ToListAsync();
        }
        return await Context.Payments.Include(e => e.IdSaleNavigation)
            .Where(e => e.IdSaleNavigation.IdClient == idClient && e.IdSale == idSale)
            .Select(e => new
            {
                e.IdSale,
                e.Date,
                e.Value
            }).ToListAsync();
    }

    public async Task<bool> DoesSaleExist(int idSale)
    {
        return await Context.SingleSales.FindAsync(idSale) != null;
    }

    public async Task<bool> IsSaleSigned(int idSale)
    {
        return await Context.SingleSales
            .FirstOrDefaultAsync(e => e.IdSale == idSale && e.IsSigned == "Y") != null;
    }

    public async Task<bool> DoesAmountCorrelatesToSomePayment(int idSale, decimal amount)
    {
        return await Context.Payments
            .FirstOrDefaultAsync(e => e.IdSale == idSale && amount == e.Value && e.Date == null) != null;
    }

    public async Task<bool> IsSaleExpired(int idSale)
    {
        return await Context.SingleSales
            .FirstOrDefaultAsync(e => e.IdSale == idSale && e.ExpireDate < DateTime.Now) != null;
    }

    public async Task<object?> GetUnrealizedClientPayments(int idClient, int ?idSale)
    {
        if (idSale == null)
        {
            return await Context.Payments.Include(e => e.IdSaleNavigation)
                .Where(e => e.IdSaleNavigation.IdClient == idClient && e.Date == null)
                .Select(e => new
                {
                    e.IdSale,
                    e.Date,
                    e.Value
                }).ToListAsync();
        }
        return await Context.Payments.Include(e => e.IdSaleNavigation)
            .Where(e => e.IdSaleNavigation.IdClient == idClient && e.Date == null && e.IdSale == idSale)
            .Select(e => new
            {
                e.IdSale,
                e.Date,
                e.Value
            }).ToListAsync();
    }
    
    public async Task AddPaymentAndApplyPaymentLogic(int idSale, decimal amount)
    {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var payment = await Context.Payments
                .FirstOrDefaultAsync(e => e.IdSale == idSale && e.Date == null && e.Value == amount);

            if (payment == null)
            {
                throw new Exception("Payment not found or already processed.");
            }
            
            payment.Date = DateTime.Now;
            Context.Payments.Update(payment);
            await Context.SaveChangesAsync();

            var payments = await Context.Payments.Where(e => e.IdSale == idSale && e.Date == null).ToListAsync();

            if (payments.Count == 0)
            {
                var sale = await Context.SingleSales.FirstOrDefaultAsync(e => e.IdSale == idSale);
                if (sale == null)
                {
                    throw new Exception("Sale not found.");
                }
                sale.IsSigned = "Y";
                Context.SingleSales.Update(sale);
                await Context.SaveChangesAsync();

                int idClient = sale.IdClient;

                var client = await Context.Clients
                    .Include(e => e.IdDiscountNavigation)
                    .FirstOrDefaultAsync(e => e.IdClient == idClient && e.IdDiscount != null);

                if (client != null)
                {
                    var discount = await Context.ClientDiscounts
                        .FirstOrDefaultAsync(e => e.Value == (decimal)Enums.CurrentDiscountForActiveClients / 100.0m);

                    if (discount != null)
                    {
                        client.IdDiscount = discount.IdDiscount;
                        Context.Clients.Update(client);
                        await Context.SaveChangesAsync();
                    }
                }
            }
            
            scope.Complete();
        }
    }
}