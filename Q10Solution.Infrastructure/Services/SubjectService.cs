using Microsoft.EntityFrameworkCore;
using Q10Solution.Application.DTOs;
using Q10Solution.Application.Interfaces;
using Q10Solution.Domain.Entities;
using Q10Solution.Infrastructure.Data;
using Q10Solution.Infrastructure.Persistence;

namespace Q10Solution.Infrastructure.Services;

public class SubjectService : ISubjectService
{
    private readonly AppDbContext _context;

    public SubjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> GetAllAsync()
    {
        var subjects = await _context.Subjects.ToListAsync();

        return subjects.Select(s => new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            Credits = s.Credits
        }).ToList();
    }

    public async Task<SubjectDto?> GetByIdAsync(Guid id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return null;

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Credits = subject.Credits
        };
    }

    public async Task CreateAsync(SubjectDto dto)
    {
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Credits = dto.Credits
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SubjectDto dto)
    {
        var subject = await _context.Subjects.FindAsync(dto.Id);
        if (subject == null) return;

        subject.Name = dto.Name;
        subject.Code = dto.Code;
        subject.Credits = dto.Credits;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return;

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
    }
}
