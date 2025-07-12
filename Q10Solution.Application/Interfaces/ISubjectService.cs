using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q10Solution.Application.DTOs;

namespace Q10Solution.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(Guid id);
        Task CreateAsync(SubjectDto subject);
        Task UpdateAsync(SubjectDto subject);
        Task DeleteAsync(Guid id);
    }
}
