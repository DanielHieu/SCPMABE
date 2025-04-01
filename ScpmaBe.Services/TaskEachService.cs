using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Enum;
using ScpmaBe.Services.Helpers;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class TaskEachService : ITaskEachService
    {
        private readonly ITaskEachRepository _taskEachRepository;
        private readonly IStaffRepository _staffRepository;

        public TaskEachService(ITaskEachRepository taskEachRepository, IStaffRepository staffRepository)
        {
            _taskEachRepository = taskEachRepository;
            _staffRepository = staffRepository;
        }

        public async Task<TaskEachResponse> GetById(int id)
        {
            var task = await _taskEachRepository.GetAll().FirstOrDefaultAsync(x=> x.TaskEachId == id);

            if (task == null) throw AppExceptions.NotFoundId();

            return new TaskEachResponse
            {
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority.ToString(),
                Status = ((TaskEachStatus)task.Status).ToString(),
                StartDate = task.StartDate.ToString("dd/MM/yyyy"),
                EndDate = task.EndDate.ToString("dd/MM/yyyy"),
                AssignedToId = task.AssignedToId,
                AssigneeName = $"{task.AssignedTo?.FirstName} {task.AssignedTo?.LastName}",
            };
        }

        public async Task<List<TaskEachResponse>> SearchTaskEachAsync(SearchTaskRequest request)
        {
            var query = _taskEachRepository.GetAll().Include(x=>x.AssignedTo).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x =>
                                !string.IsNullOrEmpty(x.Description) &&
                                x.Description.Contains(request.Keyword));
            }

            var tasks = await query.ToListAsync();

            return tasks.Select(x => new TaskEachResponse
            {
                TaskEachId = x.TaskEachId,
                Title = x.Title,
                AssignedToId = x.AssignedToId,
                AssigneeName = $"{x.AssignedTo?.FirstName} {x.AssignedTo?.LastName}",
                StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                Priority = ((TaskEachPriority) x.Priority).ToString(),
                Status = ((TaskEachStatus)x.Status).ToString(),
                Description = x.Description
            }).ToList();
        }

        public async Task<TaskEachResponse> AddTaskEachAsync(AddTaskEachRequest request)
        {
            var existingOwner = await _staffRepository.StaffIdExsistAsync(request.AssignedToId);

            if (!existingOwner) throw AppExceptions.NotFoundId();

            var task = new TaskEach
            {
                Title = request.Title,
                StartDate = request.StartDate.ToVNTime(),
                EndDate = request.EndDate.ToVNTime(),
                Priority = TransformPriority(request.Priority),
                AssignedToId = request.AssignedToId,
                Description = request.Description,
                Status = 1
            };

            var newTask = await _taskEachRepository.Insert(task);

            return new TaskEachResponse
            {
                Title = newTask.Title,
                Description = newTask.Description,
                Priority = newTask.Priority.ToString(),
                Status = ((TaskEachStatus)newTask.Status).ToString(),
                StartDate = newTask.StartDate.ToString("dd/MM/yyyy"),
                EndDate = newTask.EndDate.ToString("dd/MM/yyyy"),
                AssignedToId = newTask.AssignedToId,
                AssigneeName = $"{newTask.AssignedTo?.FirstName} {newTask.AssignedTo?.LastName}",
            };
        }
        
        private int TransformPriority(string priority)
        {
            return priority switch
            {
                "High" => 3,
                "Medium" => 2,
                _ => 1
            };
        }

        public async Task<TaskEachResponse> UpdateTaskEachAsync(UpdateTaskEachRequest request)
        {
            var updateTask = await _taskEachRepository.GetById(request.TaskEachId);

            if (updateTask == null) throw AppExceptions.NotFoundId();

            updateTask.StartDate = request.StartDate;
            updateTask.EndDate = request.EndDate;
            updateTask.Priority = TransformPriority(request.Priority);
            updateTask.Title = request.Title;
            updateTask.AssignedToId = request.AssignedToId;
            updateTask.Description = request.Description;

            var newTask = await _taskEachRepository.Update(updateTask);

            return new TaskEachResponse
            {
                Title = newTask.Title,
                Description = newTask.Description,
                Priority = newTask.Priority.ToString(),
                Status = ((TaskEachStatus)newTask.Status).ToString(),
                StartDate = newTask.StartDate.ToString("dd/MM/yyyy"),
                EndDate = newTask.EndDate.ToString("dd/MM/yyyy"),
                AssignedToId = newTask.AssignedToId,
                AssigneeName = $"{newTask.AssignedTo?.FirstName} {newTask.AssignedTo?.LastName}",
            };
        }

        public async Task<bool> DeleteTaskEachAsync(int id)
        {
            try
            {
                var task = await _taskEachRepository.GetById(id);
                if (task == null) throw AppExceptions.NotFoundId();

                await _taskEachRepository.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CompleteAsync(int id)
        {
            try
            {
                var task = await _taskEachRepository.GetById(id);

                if (task == null) throw AppExceptions.NotFoundId();

                task.Status = (int)TaskEachStatus.Completed;

                await _taskEachRepository.Update(task);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
