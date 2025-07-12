using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q10Solution.Application.DTOs;

namespace Q10Solution.Application.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentDto>> GetAllAsync();
        Task<StudentDto?> GetByIdAsync(Guid id);
        Task CreateAsync(StudentDto student);
        Task UpdateAsync(StudentDto student);
        Task DeleteAsync(Guid id);
        
        Task AssignSubjectsAsync(Guid id, List<Guid> subjectIds);

    }
}
