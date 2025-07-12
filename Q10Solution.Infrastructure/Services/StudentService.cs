using Microsoft.EntityFrameworkCore;
using Q10Solution.Application.DTOs;
using Q10Solution.Application.Interfaces;
using Q10Solution.Domain.Entities;
using Q10Solution.Infrastructure.Data;
using Q10Solution.Infrastructure.Persistence;

namespace Q10Solution.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;

    public StudentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        var students = await _context.Students
            .Include(s => s.Subjects)
            .ToListAsync();

        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Document = s.Document,
            Email = s.Email,
            SubjectIds = s.Subjects.Select(sub => sub.Id).ToList(),
            Subjects = s.Subjects.Select(sub => new SubjectDto
            {
                Id = sub.Id,
                Name = sub.Name,
                Code = sub.Code,
                Credits = sub.Credits
            }).ToList()
        }).ToList();

    }

    public async Task<StudentDto?> GetByIdAsync(Guid id)
    {
        var student = await _context.Students
            .Include(s => s.Subjects)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return null;

        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Document = student.Document,
            Email = student.Email,
            SubjectIds = student.Subjects.Select(sub => sub.Id).ToList()
        };
    }

    public async Task CreateAsync(StudentDto dto)
    {
        var subjects = await _context.Subjects
            .Where(s => dto.SubjectIds.Contains(s.Id))
            .ToListAsync();

        ValidateCreditLimit(subjects);

        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Document = dto.Document,
            Email = dto.Email,
            Subjects = subjects
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudentDto dto)
    {
        var student = await _context.Students
            .Include(s => s.Subjects)
            .FirstOrDefaultAsync(s => s.Id == dto.Id);

        if (student == null) return;

        var subjects = await _context.Subjects
            .Where(s => dto.SubjectIds.Contains(s.Id))
            .ToListAsync();

        ValidateCreditLimit(subjects);

        student.Name = dto.Name;
        student.Document = dto.Document;
        student.Email = dto.Email;
        student.Subjects = subjects;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
    }

    private void ValidateCreditLimit(List<Subject> subjects)
    {
        int highCreditCount = subjects.Count(s => s.Credits > 4);

        if (highCreditCount > 3)
        {
            throw new InvalidOperationException("No se pueden inscribir más de 3 materias con más de 4 créditos.");
        }
    }

    public async Task AssignSubjectsAsync(Guid studentId, List<Guid> subjectIds)
    {
        var student = await _context.Students
            .Include(s => s.Subjects)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            throw new InvalidOperationException("Estudiante no encontrado.");
                
        var newSubjects = await _context.Subjects
            .Where(s => subjectIds.Contains(s.Id))
            .ToListAsync();
                
        ValidateCreditLimit(newSubjects);
                
        student.Subjects = newSubjects;

        await _context.SaveChangesAsync();
    }


}
