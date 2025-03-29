using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class TaskEachService : ITaskEachService
    {
        private readonly ITaskEachRepository _taskEachRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IHashHelper _hashHelper;

        public TaskEachService(ITaskEachRepository taskEachRepository, IOwnerRepository ownerRepository, IHashHelper hashHelper)
        {
            _taskEachRepository = taskEachRepository;
            _ownerRepository = ownerRepository;
            _hashHelper = hashHelper;
        }

        public async Task<List<TaskEach>> GetPaging(int pageIndex, int pageSize)
        {
            return await _taskEachRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<TaskEach>> GetTaskEachOfOwnerAsync(int ownerId)
        {
            return await _taskEachRepository.GetAll()
                                             .Where(x => x.OwnerId == ownerId)
                                             .ToListAsync();
        }

        public async Task<TaskEach> GetById(int id)
        {
            var task = await _taskEachRepository.GetById(id);

            if (task == null) throw AppExceptions.NotFoundId();

            return task;
        }

        public async Task<List<TaskEach>> SearchTaskEachAsync(SearchTaskRequest request)
        {
            var query = _taskEachRepository.GetAll();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x =>
                                !string.IsNullOrEmpty(x.Description) &&
                                x.Description.Contains(request.Keyword));
            }

            var tasks = await query.Select(x => new TaskEach
            {
                TaskEachId = x.TaskEachId,
                OwnerId = x.OwnerId,
                Description = x.Description
            }).ToListAsync();

            return tasks;
        }

        public async Task<TaskEach> AddTaskEachAsync(AddTaskEachRequest request)
        {
            var existingOwner = await _ownerRepository.ExistsByIdAsync(request.OwnerId);

            if (!existingOwner) throw AppExceptions.NotFoundId();

            var newTask = new TaskEach()
            {
                OwnerId = request.OwnerId,
                Description = request.Description
            };

            return await _taskEachRepository.Insert(newTask);
        }

        public async Task<TaskEach> UpdateTaskEachAsync(UpdateTaskEachRequest request)
        {
            var updateTask = await _taskEachRepository.GetById(request.TaskEachId);

            if (updateTask == null) throw AppExceptions.NotFoundId();

            updateTask.Description = request.Description;

            await _taskEachRepository.Update(updateTask);

            return updateTask;
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
    }
}
