namespace SemestralProject.DTOs.ContractDTOs;

public class ContractAddDto
{
    public int IdClient { get; set; }

    public int IdSoftware { get; set; }
    
    public DateTime ExpireDate { get; set; }
    
    public string Description { get; set; } = null!;

    public int ExtraYearsForUpdates { get; set; }

    public string UpdatesInfo { get; set; } = null!;

    public int NumberOfRates { get; set; }
}