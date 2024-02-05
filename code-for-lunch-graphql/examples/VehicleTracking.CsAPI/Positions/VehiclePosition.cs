namespace VehicleTracking.CsAPI.Positions;

public class VehiclePosition
{
    public VehiclePosition(Guid vehicleId, double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        VehicleId = vehicleId;
    }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public Guid VehicleId { get; init; }
}