namespace SemestralProject.Services;

public interface IIncomeService
{
    Task<Decimal> CalculateCurrentIncome();
    Task<Decimal> CalculatePossibleIncome();
    Task<Decimal> CalculateIncomeFromCurrentPayments();
    Task<Decimal> CalculateSoftwareIncome(int idSoftware);
}