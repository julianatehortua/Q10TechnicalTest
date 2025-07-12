using Microsoft.EntityFrameworkCore;
using Q10Solution.Application.DTOs;
using Q10Solution.Domain.Entities;
using Q10Solution.Infrastructure.Persistence;
using Q10Solution.Infrastructure.Services;
using Xunit;

public class SubjectServiceTests
{    
    private readonly AppDbContext _context; 
    private readonly SubjectService _service;

    // Este constructor se ejecuta antes de cada prueba    
    public SubjectServiceTests()
    {
        // Usamos una base de datos en memoria para que no afecte datos reales
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // le damos un nombre único a cada BD
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated(); 

        _service = new SubjectService(_context); // el servicio que vamos a probar
    }

    [Fact]
    public async Task CreateSubject_ShouldAddNewSubject()
    {
        // Creamos el DTO con la info de la materia
        var dto = new SubjectDto
        {
            Name = "Física",
            Code = "PHY101",
            Credits = 3
        };

        // Ejecutamos el método de crear
        await _service.CreateAsync(dto);

        // Validamos en la BD la creaciónn
        var subject = await _context.Subjects.FirstOrDefaultAsync();

        // Y confirmamos que todo esté bien
        Assert.NotNull(subject);
        Assert.Equal("Física", subject.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSubjects()
    {
        // Agregamos un par de materias directamente a la BD
        _context.Subjects.AddRange(
            new Subject { Id = Guid.NewGuid(), Name = "Química", Code = "CHEM", Credits = 4 },
            new Subject { Id = Guid.NewGuid(), Name = "Inglés", Code = "ENG", Credits = 2 }
        );
        await _context.SaveChangesAsync();

        // Probamos el método que trae todo
        var result = await _service.GetAllAsync();

        // Debe haber 2 materias
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateSubject_ShouldUpdateCorrectly()
    {
        // Creamos una materia y la guardamos en la base de datos
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = "Biología",
            Code = "BIO",
            Credits = 3
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Creamos un DTO con los cambios que queremos hacer
        var dto = new SubjectDto
        {
            Id = subject.Id,
            Name = "Biología Avanzada",
            Code = "BIO2",
            Credits = 4
        };

        // Mandamos a actualizar
        await _service.UpdateAsync(dto);

        // Verificamos que sí se haya actualizado
        var updated = await _context.Subjects.FindAsync(subject.Id);
        Assert.NotNull(updated);
        Assert.Equal("Biología Avanzada", updated.Name);
        Assert.Equal("BIO2", updated.Code);
    }

    [Fact]
    public async Task DeleteSubject_ShouldRemoveFromDb()
    {
        // Creamos una materia y la guardamos
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = "Arte",
            Code = "ART",
            Credits = 2
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // La borramos usando el servicio
        await _service.DeleteAsync(subject.Id);

        // Y confirmamos que ya no esté
        var deleted = await _context.Subjects.FindAsync(subject.Id);
        Assert.Null(deleted);
    }
}
