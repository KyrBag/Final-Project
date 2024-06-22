using AutoMapper;

namespace TaskManagerApp.Services
{
	public class ApplicationService : IApplicationService
	{
		private readonly IMapper _mapper;
		private readonly ILogger<UserService>? _logger;

		public ApplicationService(ILogger<UserService>? logger, IMapper mapper)
		{
			_mapper = mapper;
			_logger = logger;
		}

		public UserService UserService => new(_logger, _mapper);
		
	}
}
