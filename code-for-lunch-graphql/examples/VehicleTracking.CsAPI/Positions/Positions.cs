namespace VehicleTracking.CsAPI.Positions;

public class Positions
{
    private const double MinLat = 44.8948358;
    private const double MaxLat = 52.5841061;
    private const double MinLon = 4.9568123;
    private const double MaxLon = 28.1767924;
    
    private readonly Random random = new Random();

    public VehiclePosition NextPosition(Guid vehicleId)
    {
        return new VehiclePosition(
            vehicleId,
            random.NextDouble() * (MaxLat - MinLat) + MinLat,
            random.NextDouble() * (MaxLon - MinLon) + MinLon
        );
    }
}