using AutoMapper;
using TaskManagerApp.DTOs;
using TaskManagerApp.Models;

namespace TaskManagerApp.Configuration
{
	public class MapperConfig : Profile
	{
		public MapperConfig() 
		{
			CreateMap<CreateTaskItemDTO, TaskItem>();
			CreateMap<TaskItem, TaskItemDTO>();
			CreateMap<TaskItemDTO, TaskItem>();


			
		}
	}
}
