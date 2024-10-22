using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        Console.WriteLine($"Calling gRPC Service {_configuration["GrpcPlatform"]}");
        var grpcPlatformAddress = _configuration["GrpcPlatform"];
        if (string.IsNullOrEmpty(grpcPlatformAddress))
        {
            throw new ArgumentNullException("GrpcPlatform address is not configured.");
        }
        var channel = GrpcChannel.ForAddress(grpcPlatformAddress);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not call the gRPC Server. {ex.Message}");
            return Enumerable.Empty<Platform>();
        }
    }

}
}
    