using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Helpers;

public static class ContactSimilarityHelper
{
    public static bool IsDuplicate(
        this Person existing, 
        CreatePersonDto newContact, 
        DeduplicationConfigDto config)
    {
        if (config.PropertiesToCompare.Contains("FirstName") && config.PropertiesToCompare.Contains("LastName"))
        {
            var fullNameSimilar = JaroWinklerDistance.AreSimilar(
                $"{existing.FirstName} {existing.LastName}",
                $"{newContact.FirstName} {newContact.LastName}",
                config.SimilarityThreshold);
            
            if (fullNameSimilar) return true;
        }

        foreach (var prop in config.PropertiesToCompare)
        {
            switch (prop)
            {
                case "FirstName":
                    if (JaroWinklerDistance.AreSimilar(existing.FirstName, newContact.FirstName, config.SimilarityThreshold))
                        return true;
                    break;
                case "LastName":
                    if (JaroWinklerDistance.AreSimilar(existing.LastName, newContact.LastName, config.SimilarityThreshold))
                        return true;
                    break;
                case "Email":
                    if (string.Equals(existing.Email, newContact.Email, StringComparison.OrdinalIgnoreCase))
                        return true;
                    break;
                case "Phone":
                    if (string.Equals(existing.Phone, newContact.Phone, StringComparison.OrdinalIgnoreCase))
                        return true;
                    break;
            }
        }
        return false;
    }

    public static bool IsExactDuplicate(this Person existing, CreatePersonDto newContact)
    {
        return string.Equals(existing.FirstName, newContact.FirstName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existing.LastName, newContact.LastName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existing.Email, newContact.Email, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existing.Phone, newContact.Phone, StringComparison.OrdinalIgnoreCase);
    }
}