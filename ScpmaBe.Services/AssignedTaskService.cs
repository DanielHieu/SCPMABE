using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmBe.Services.Exceptions;

namespace ScpmaBe.Services
{
    public class AssignedTaskService : IAssignedTaskService
    {
        private readonly IAssignedTaskRepository _assignedTaskRepository;
        private readonly ITaskEachRepository _tasksEachRepository;
        private readonly IStaffRepository _staffRepository;

        public AssignedTaskService(IAssignedTaskRepository assignedTaskRepository, ITaskEachRepository tasksEachRepository,
            IStaffRepository staffRepository)
        {
            _assignedTaskRepository = assignedTaskRepository;
            _tasksEachRepository = tasksEachRepository;
            _staffRepository = staffRepository;
        }

        public async Task<List<AssignedTask>> GetPaging(int pageIndex, int pageSize)
        {
            return await _assignedTaskRepository.GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<AssignedTask>> GetAssignedTasksOfStaffAsync(int staffId)
        {
            return await _assignedTaskRepository.GetAll()
                                                .Where(x => x.StaffId == staffId)
                                                .ToListAsync();
        }

        public async Task<AssignedTask> GetById(int id)
        {
            var assTask = await _assignedTaskRepository.GetById(id);
            if (assTask == null) throw AppExceptions.NotFoundId();

            return assTask;
        }

        public async Task<AssignedTask> AddAssignedTaskAsync(AddAssignedTaskRequest request)
        {
            var existStaff = await _staffRepository.StaffIdExsistAsync(request.StaffId);
            if (!existStaff) throw AppExceptions.NotFoundId();

            var existTasksEach = await _tasksEachRepository.TaskEachIdExsistAsync(request.TaskEachId);
            if (!existTasksEach) throw AppExceptions.NotFoundId();

            var newAssTask = new AssignedTask
            {
                TaskEachId = request.TaskEachId,
                StaffId = request.StaffId,
                ShiftDate = request.ShiftDate,
                ShiftTime = request.ShiftTime,
                Address = request.Address,
                Note = request.Note,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            return await _assignedTaskRepository.Insert(newAssTask);
        }

        public async Task<AssignedTask> UpdateAssignedTaskAsync(UpdateAssignedTaskRequest request)
        {
            var existStaff = await _staffRepository.StaffIdExsistAsync(request.StaffId);
            if (!existStaff) throw AppExceptions.NotFoundId();

            var existTasksEach = await _tasksEachRepository.TaskEachIdExsistAsync(request.TaskEachId);
            if (!existTasksEach) throw AppExceptions.NotFoundId();

            var updateAssTask = await _assignedTaskRepository.GetById(request.AssignedTaskId);
            if (updateAssTask == null) throw AppExceptions.NotFoundId();

            updateAssTask.AssignedTaskId = request.AssignedTaskId;
            updateAssTask.StaffId = request.StaffId;
            updateAssTask.TaskEachId = request.TaskEachId;
            updateAssTask.ShiftDate = request.ShiftDate;
            updateAssTask.ShiftTime = request.ShiftTime;
            updateAssTask.Address = request.Address;
            updateAssTask.Note = request.Note;
            updateAssTask.CreatedDate = request.CreatedDate;
            updateAssTask.UpdatedDate = DateTime.Now;

            await _assignedTaskRepository.Update(updateAssTask);
            return updateAssTask;
        }

        public async Task<bool> DeleteAssignedTaskAsync(int id)
        {
            try
            {
                var assTask = await _assignedTaskRepository.GetById(id);
                if (assTask == null) throw AppExceptions.NotFoundId();

                await _assignedTaskRepository.Delete(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
