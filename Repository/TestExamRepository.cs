using Entity;
using Entity.Data;
using Entity.Test;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Test;

namespace Repository
{
    public class TestExamRepository : ITestExamRepository
    {
        private readonly AppDbContext _context;

        public TestExamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestModel> AddTestAsync(TestModel model)
        {
            var newTest = new TestExam
            {
                Id = Guid.NewGuid(),
                TestName = model.TestName,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                CreateBy = model.CreateBy,
            };
            _context.TestExam.Add(newTest);

            foreach (var classId in model.ClassIds)
            {
                var classRelation = new ClassRelationShip
                {
                    Id = Guid.NewGuid(), 
                    TestId = newTest.Id, 
                    ClassId = classId   
                };

                _context.ClassRelationShip.Add(classRelation); 
            }

            await _context.SaveChangesAsync();

            model.Id = newTest.Id;
            return model;
        }

        public async Task<IEnumerable<TestExam>> GetAllTestsAsync(Guid userId)
        {
            return await _context.TestExam
                .Where(test => test.CreateBy == userId) 
                .ToListAsync();
        }
    }
}
