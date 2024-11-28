using Microsoft.EntityFrameworkCore;
using IRepository;
using Entity;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.Data;
using Entity.CourseFolder;

namespace Repository
{
    public class ClassFileRepository : IClassFileRepository
    {
        private readonly AppDbContext _context;

        public ClassFileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassFileDto>> GetAllByClassIdAsync(Guid classId)
        {
            return await _context.ClassFiles
                .Where(cf => cf.ClassId == classId)
                .Select(cf => new ClassFileDto
                {
                    FilePath = cf.FilePath,
                    ClassId = cf.ClassId,
                    Topic = cf.Topic,
                    Description = cf.Description,
                    UploadDate = cf.UploadDate
                })
                .ToListAsync();
        }

        public async Task<ClassFileDto> GetByIdAsync(Guid id)
        {
            var classFile = await _context.ClassFiles.FindAsync(id);
            if (classFile == null) return null;

            return new ClassFileDto
            {
                FilePath = classFile.FilePath,
                ClassId = classFile.ClassId,
                Topic = classFile.Topic,
                Description = classFile.Description,
                UploadDate = classFile.UploadDate
            };
        }

        public async Task CreateAsync(ClassFile classFile)
        {
            await _context.ClassFiles.AddAsync(classFile);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var classFile = await _context.ClassFiles.FindAsync(id);
            if (classFile == null) return false;

            _context.ClassFiles.Remove(classFile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ClassFileDto> UpdateAsync(Guid id, ClassFileDto updatedClassFile)
        {
            var classFile = await _context.ClassFiles.FindAsync(id);
            if (classFile == null) return null;

            classFile.FilePath = updatedClassFile.FilePath;
            classFile.Topic = updatedClassFile.Topic;
            classFile.Description = updatedClassFile.Description;
            classFile.UploadDate = updatedClassFile.UploadDate;

            await _context.SaveChangesAsync();

            return new ClassFileDto
            {
                FilePath = classFile.FilePath,
                ClassId = classFile.ClassId,
                Topic = classFile.Topic,
                Description = classFile.Description,
                UploadDate = classFile.UploadDate
            };
        }
    }
}
