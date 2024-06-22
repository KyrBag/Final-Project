using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApp.DTOs;
using TaskManagerApp.Models;
using TaskManagerApp.Services;

namespace TaskManagerApp.Controllers
{

	[Authorize]
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TaskItemsController : ControllerBase
	{
		private readonly ITaskItemService _taskItemService;
		private readonly IMapper _mapper;
		private readonly ILogger<TaskItemsController> _logger;
		private readonly UserManager<User> _userManager;
		public TaskItemsController(ITaskItemService taskItemService, IMapper mapper,
			ILogger<TaskItemsController> logger, UserManager<User> userManager)
		{
			_taskItemService = taskItemService;
			_mapper = mapper;
			_logger = logger;
			_userManager = userManager;
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTaskItems()
		{
			_logger.LogInformation("GetTaskItems called");
			var taskItems = await _taskItemService.GetAllTaskItemsAsync();
			return Ok(_mapper.Map<IEnumerable<TaskItemDTO>>(taskItems));
		}


		
		[HttpPost]
		public async Task<IActionResult> CreateTaskItem(CreateTaskItemDTO createTaskItemDTO)
		{
			_logger.LogInformation("CreateTaskItem called");

			try
			{
				// Check if the username is provided
				if (string.IsNullOrEmpty(createTaskItemDTO.Username))
				{
					_logger.LogWarning("Username is null or empty.");
					return BadRequest(new { isSuccess = false, message = "Username cannot be null or empty." });
					
				}
				// Retrieve the user by their username
				_logger.LogInformation($"Retrieving user with username '{createTaskItemDTO.Username}'...");
				var user = await _userManager.FindByNameAsync(createTaskItemDTO.Username!);

				// Check if the user exists
				if (user == null)
				{
					_logger.LogWarning($"User with username '{createTaskItemDTO.Username}' not found.");
					return NotFound(new { isSuccess = false, message = $"User with username '{createTaskItemDTO.Username}' not found." });
					
				}

				_logger.LogInformation($"User {user.UserName} is found.");

				// Map data from DTO to TaskItem entity
				_logger.LogInformation("Mapping CreateTaskItemDTO to TaskItem entity...");
				var taskItem = _mapper.Map<TaskItem>(createTaskItemDTO);

				// Set the user ID of the task item
				taskItem.UserId = user.Id;

				// Create the task item using a service method
				_logger.LogInformation("Creating task item...");
				var result = await _taskItemService.CreateTaskItemAsync(taskItem);

				// Check if the task item was created successfully
				if (!result)
				{
					_logger.LogError("A problem occurred while handling the request.");
					return StatusCode(500, new { isSuccess = false, message = "A problem occurred while handling your request." });
					
				}

				_logger.LogInformation("Task item created successfully.");

				// Return an Ok response with the created task item
				return Ok(new { isSuccess = true, message = "Task created successfully" });
				//return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while creating the task item.");
				return StatusCode(500, new { isSuccess = false, message = "An error occurred while creating the task item.", details = ex.Message });
				;
			}
			
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTaskItemById(int id)
		{
			var taskItem = await _taskItemService.GetTaskItemByIdAsync(id);
			if (taskItem == null)
			{
				return NotFound();
			}
			var taskItemDTO = _mapper.Map<TaskItemDTO>(taskItem);
			return Ok(taskItemDTO);
		}


		

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTaskItem(int id, [FromBody] TaskItemDTO taskItemDTO)
		{
			_logger.LogInformation("UpdateTaskItem called with ID: {Id}", id);

			if (id != taskItemDTO.Id)
			{
				_logger.LogWarning("ID in URL does not match ID in the provided DTO. URL ID: {UrlId}, DTO ID: {DtoId}", id, taskItemDTO.Id);
				return BadRequest("ID in URL does not match ID in the provided DTO.");
			}

			try
			{
				// Fetch the existing task to retain user association
				var existingTask = await _taskItemService.GetTaskItemByIdAsync(id);
				if (existingTask == null)
				{
					_logger.LogWarning("TaskItem with ID: {Id} not found.", id);
					return NotFound("Task item not found.");
				}

				// Map the updated fields from the DTO to the existing task
				_mapper.Map(taskItemDTO, existingTask);
				_logger.LogInformation("TaskItemDTO successfully mapped to TaskItem.");

				// Ensure the user association is retained
				existingTask.UserId = existingTask.UserId;

				await _taskItemService.UpdateTaskItemAsync(existingTask);
				_logger.LogInformation("TaskItem with ID: {Id} updated successfully.", id);

				return NoContent();
			}
			catch (AutoMapperMappingException ex)
			{
				_logger.LogError(ex, "Error occurred while mapping TaskItemDTO to TaskItem.");
				return StatusCode(500, "Internal server error while processing your request.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while updating the task item.");
				return StatusCode(500, "A problem occurred while handling your request.");
			}
		}


	

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTaskItem(int id)
		{
			try
			{
				await _taskItemService.DeleteTaskItemAsync(id);
				_logger.LogInformation($"Task item with ID {id} deleted successfully.");
				return NoContent(); // 204 No Content response
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error deleting task item with ID {id}: {ex.Message}");
				return StatusCode(500, "Internal server error"); // Return 500 Internal Server Error with a message
			}
		}
	}
}
