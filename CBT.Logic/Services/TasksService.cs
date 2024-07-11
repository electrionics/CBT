using CBT.Domain;
using CBT.Domain.Entities;

namespace CBT.Logic.Services
{
    public class TasksService
    {
        private readonly CBTDataContext _dbContext;

        public TasksService(CBTDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<TaskEntity>> GetAllTasks()
        {
            throw new NotImplementedException();
        }
    }
}
