namespace PetAdoption_WebApi.Models
{
    public interface IAuditable
    {
        string? CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        string? UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}
