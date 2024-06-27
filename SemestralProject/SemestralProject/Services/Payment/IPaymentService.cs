namespace SemestralProject.Services;

public interface IPaymentService
{
    Task<object?> GetUnrealizedClientPayments(int idClient, int ?idSale);
    Task<object?> GetAllClientPayments(int idClient, int ?idSale);
    Task<bool> DoesSaleExist(int idSale);
    Task<bool> IsSaleSigned(int idSale);
    Task<bool> DoesAmountCorrelatesToSomePayment(int idSale, decimal amount);
    Task<bool> IsSaleExpired(int idSale);
    Task AddPaymentAndApplyPaymentLogic(int idSale, decimal amount);
}