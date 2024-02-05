using System.ComponentModel.DataAnnotations;

namespace VehicleTracking.CsAPI.Vehicles;

public record VehicleInput(
    [Required]
    Guid Id,
    
    [Required]
    [GraphQLDescription("Label for easier identification of vehicle")]
    string Label)
{
    
}