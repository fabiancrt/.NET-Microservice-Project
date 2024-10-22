using AutoMapper;
using CommandService.Dtos;
using CommandsService.Data;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _reposiroty;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repository, IMapper mapper)
        {
          _reposiroty = repository;
          _mapper = mapper;  
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("Getting Platforms from Command Service");
            var platformItems = _reposiroty.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }
        [HttpPost]
        public ActionResult TestInboundConnection(){
            Console.WriteLine("Inbound POST # Command Service");
            return Ok("Inbound test of from Platforms Controller");
        }
    }
}