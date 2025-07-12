using System;
using Microsoft.EntityFrameworkCore;
using Q10Solution.Application.DTOs;
using Q10Solution.Application.Interfaces;
using Q10Solution.Domain.Entities;
using Q10Solution.Infrastructure.Persistence;
using Q10Solution.Infrastructure.Services;
using Xunit;

public class StudentServiceTests
{
    private readonly AppDbContext _context;
    private readonly StudentService _service;

    // Este constructor se ejecuta antes de cada prueba    
    public StudentServiceTests()
    {
        // Usamos una base de datos en memoria para que no afecte datos reales
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // le damos un nombre único a cada BD
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _service = new StudentService(_context); // el servicio que vamos a probar
    }

    [Fact]
    public async Task CreateStudent_WithValidSubjects_ShouldSucceed()
    {
        // Creamos dos materias normales con créditos permitidos
        var s1 = new Subject { Id = Guid.NewGuid(), Name = "Religion", Credits = 3 };
        var s2 = new Subject { Id = Guid.NewGuid(), Name = "Artistica", Credits = 2 };
        _context.Subjects.AddRange(s1, s2);
        await _context.SaveChangesAsync();

        // Armamos el DTO del estudiante que queremos guardar
        var studentDto = new StudentDto
        {
            Name = "Pepito",
            Document = "123457",
            Email = "pepito@gmail.com",
            SubjectIds = new List<Guid> { s1.Id, s2.Id }
        };

        // Llamamos al servicio para guardar el estudiante
        await _service.CreateAsync(studentDto);

        // Verificamos que sí se guardó y que tiene las materias que esperábamos
        var student = await _context.Students.Include(s => s.Subjects).FirstOrDefaultAsync();
        Assert.NotNull(student);
        Assert.Equal(2, student.Subjects.Count);
    }

    [Fact]
    public async Task CreateStudent_WithTooManyHighCreditSubjects_ShouldThrow()
    {
        // Creamos 4 materias con más de 4 créditos (lo cual no está permitido)
        var subjects = new List<Subject>
        {
            new Subject { Id = Guid.NewGuid(), Name = "Calculo", Credits = 5 },
            new Subject { Id = Guid.NewGuid(), Name = "Estadistica", Credits = 6 },
            new Subject { Id = Guid.NewGuid(), Name = "Geometria", Credits = 7 },
            new Subject { Id = Guid.NewGuid(), Name = "Finanzas", Credits = 8 }
        };

        _context.Subjects.AddRange(subjects);
        await _context.SaveChangesAsync();

        // Preparamos un estudiante con esas materias
        var studentDto = new StudentDto
        {
            Name = "Daniela Atehortua",
            Document = "4567812",
            Email = "dani45@gmail.com",
            SubjectIds = subjects.Select(s => s.Id).ToList()
        };

        // Esperamos que falle, porque se están pasando del límite de materias con muchos créditos
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(studentDto));
    }

    [Fact]
    public async Task UpdateStudent_ShouldUpdateSubjectsCorrectly()
    {
        // Creamos un estudiante sin materias y preparamos dos materias nuevas
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Carlos",
            Document = "111",
            Email = "carlos@mail.com",
            Subjects = new List<Subject>()
        };
        var s1 = new Subject { Id = Guid.NewGuid(), Name = "Bio", Credits = 3 };
        var s2 = new Subject { Id = Guid.NewGuid(), Name = "Art", Credits = 2 };

        _context.Subjects.AddRange(s1, s2);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Armamos los datos nuevos que le queremos poner a Carlos
        var dto = new StudentDto
        {
            Id = student.Id,
            Name = "Carlos A.",
            Document = "111",
            Email = "carlos.a@mail.com",
            SubjectIds = new List<Guid> { s1.Id, s2.Id }
        };

        // Ejecutamos la actualización
        await _service.UpdateAsync(dto);

        // Revisamos que el nombre haya cambiado y que tenga las materias nuevas
        var updated = await _context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == student.Id);
        Assert.NotNull(updated);
        Assert.Equal("Carlos A.", updated.Name);
        Assert.Equal(2, updated.Subjects.Count);
    }

    [Fact]
    public async Task DeleteStudent_ShouldRemoveStudentFromDb()
    {
        // Creamos un estudiante sencillo
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Luis Montes",
            Document = "222222",
            Email = "luis@gmail.com"
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Ahora lo borramos
        await _service.DeleteAsync(student.Id);

        // Verificamos que ya no exista en la BD
        var result = await _context.Students.FindAsync(student.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStudentsWithSubjects()
    {
        // Creamos una materia y un estudiante con esa materia
        var s1 = new Subject { Id = Guid.NewGuid(), Name = "Math", Credits = 4 };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Adrian Velez",
            Document = "3333333",
            Email = "Adrian@gmail.com",
            Subjects = new List<Subject> { s1 }
        };

        _context.Subjects.Add(s1);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Obtenemos los estudiantes desde el servicio
        var students = await _service.GetAllAsync();

        // Verificamos que venga Daniela y que tenga la materia asociada
        Assert.Single(students);
        Assert.Equal("Daniela", students[0].Name);
        Assert.Single(students[0].SubjectIds);
    }

    [Fact]
    public async Task AssignSubjectsAsync_ShouldThrow_WhenMoreThan3HighCreditSubjects()
    {
        // Creamos un estudiante y 4 materias que se pasan del límite de créditos
        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Diego Torres",
            Document = "9876543",
            Email = "torres@gmail.com"
        };

        var subjects = Enumerable.Range(1, 4).Select(i =>new Subject { Id = Guid.NewGuid(), Name = $"Materia {i}", Credits = 5 }).ToList();

        _context.Students.Add(student);
        _context.Subjects.AddRange(subjects);
        await _context.SaveChangesAsync();

        var ids = subjects.Select(s => s.Id).ToList();

        // Se intenta guardar la asignacion de materias, debería lanzar error
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AssignSubjectsAsync(student.Id, ids));
    }
}
