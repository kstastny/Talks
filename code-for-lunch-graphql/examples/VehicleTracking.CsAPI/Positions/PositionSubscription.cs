using System.Net.Sockets;
using System.Runtime.CompilerServices;
using HotChocolate.Subscriptions;
using VehicleTracking.Core;
using VehicleTracking.CsAPI.RootObjects;

namespace VehicleTracking.CsAPI.Positions;

[ExtendObjectType<RootSubscription>]
public class PositionSubscription : IDisposable, IAsyncDisposable
{
    private readonly Storage.Storage storage;
    private readonly ITopicEventSender topicEventSender;
    private readonly Timer timer;

    public PositionSubscription(
        [Service] Storage.Storage storage,
        [Service] ITopicEventSender topicEventSender)
    {
        this.storage = storage;
        this.topicEventSender = topicEventSender;

        timer = new Timer(
            SendPosition,
            null,
            TimeSpan.FromSeconds(5.0), 
            TimeSpan.FromSeconds(5.0));
    }


    private const string TopicName = "vehiclePositions";

    private readonly Positions positions = new Positions();


    private void SendPosition(object obj)
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        SendPositionAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private async Task SendPositionAsync()
    {
        var vehicles = await storage.GetVehicles(CancellationToken.None);
        var vehicle = vehicles.Head;

        await topicEventSender.SendAsync(TopicName, positions.NextPosition(vehicle.Id), CancellationToken.None);
    }

    public async IAsyncEnumerable<VehiclePosition> SubscribeToPositionStream(
        [Service] ITopicEventReceiver topicEventReceiver,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var vehicles = await storage.GetVehicles(cancellationToken);
        var vehicle = vehicles.Head;

        yield return positions.NextPosition(vehicle.Id);

        var stream =
            await topicEventReceiver.SubscribeAsync<VehiclePosition>(TopicName, cancellationToken);

        await foreach (var position in stream.ReadEventsAsync().WithCancellation(cancellationToken))
        {
            yield return position;
        }
    }

    [Subscribe(With = "SubscribeToPositionStream")]
    public VehiclePosition OnPositionMessage([EventMessage] VehiclePosition message)
    {
        return message;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (timer != null) await timer.DisposeAsync();
    }
}