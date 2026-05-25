namespace AppCore.Dto;

public class DeduplicationConfigDto
{
    public double SimilarityThreshold { get; set; } = 0.85;
    public List<string> PropertiesToCompare { get; set; } = new()
    {
        "FirstName", "LastName", "Email", "Phone"
    };
}