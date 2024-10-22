using System.Text.Json;
using AutoMapper;
using CommandService.Dtos;
using CommandService.EventProcessing;
using CommandService.Models;
using CommandsService.Data;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, AutoMapper.IMapper mapper)
        {
            _scopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    addPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            if (eventType == null)
            {
                Console.WriteLine("Event type is null");
                return EventType.Undetermined;
            }

            switch (eventType.Event)
            {
                case "PlatformPublished":
                    Console.WriteLine("Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                    Console.WriteLine("Platform added!");
                }
                else
                {
                    Console.WriteLine("Platform already exists...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add Platform to DB: {ex.Message}");
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}