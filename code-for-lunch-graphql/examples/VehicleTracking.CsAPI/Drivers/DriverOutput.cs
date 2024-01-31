using VehicleTracking.Core;

namespace VehicleTracking.CsAPI.Drivers;

public record DriverOutput
{
    public DriverOutput(Guid id, string name, string surname)
    {
        Id = id;
        Name = name;
        Surname = surname;
    }

    [GraphQLDescription("Driver identifier")]
    public Guid Id { get; init; }

    [GraphQLDescription("Driver name")]
    public string Name { get; init; }

    [GraphQLDescription("Driver surname")]
    public string Surname { get; init; }


    //TODO vehicles

    public static DriverOutput OfDriver(Domain.Driver driver)
    {
        return new DriverOutput(driver.Id, driver.Name, driver.Surname);
    }
}