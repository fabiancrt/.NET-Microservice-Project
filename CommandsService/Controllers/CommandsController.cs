using System.Windows.Input;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using CommandsService.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;
        public CommandController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"Hit GetCommandsForPlatform: {platformId}");
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var commands = _repository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }
        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"Hit GetCommandForPlatform: {platformId} / {commandId}");
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var command = _repository.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(command));
        }
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"Hit CreateCommandForPlatform: {platformId}");
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }
            var command = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}