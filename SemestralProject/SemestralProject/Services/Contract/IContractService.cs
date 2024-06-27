using SemestralProject.DTOs.ContractDTOs;

namespace SemestralProject.Services;

public interface IContractService
{
    Task<decimal> CalculatePrice(ContractAddDto contract);
    Task<bool> DoesSoftwareExist(int contractIdSoftware);
    Task<object> GetSoftwareVersion(int contractIdSoftware);
    Task<bool> HasActiveSubscriptionOrContract(int contractIdClient, int contractIdSoftware);
    Task AddSaleAndObligatoryPayments(ContractAddDto contract);
    Task<object?> GetContract(int idClient);
}