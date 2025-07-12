using Q10Solution.Domain.Entities;
using Q10Solution.Infrastructure.Persistence;

namespace Q10Solution.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();        

        // Evitar duplicar datos si ya existen
        if (context.Subjects.Any() || context.Students.Any())
            return;

        // Se crean materias
        var subjects = new List<Subject>
        {
           new Subject { Id = Guid.NewGuid(), Name = "Literatura", Code = "LIT201", Credits = 5 },
           new Subject { Id = Guid.NewGuid(), Name = "Historia", Code = "HIS202", Credits = 6 },
           new Subject { Id = Guid.NewGuid(), Name = "Biología", Code = "BIO301", Credits = 7 },
           new Subject { Id = Guid.NewGuid(), Name = "Programación I", Code = "PROG302", Credits = 3 },
           new Subject { Id = Guid.NewGuid(), Name = "Economía", Code = "ECO401", Credits = 4 },
           new Subject { Id = Guid.NewGuid(), Name = "Derecho Constitucional", Code = "DER402", Credits = 8 },
           new Subject { Id = Guid.NewGuid(), Name = "Arte y Diseño", Code = "ART501", Credits = 2 },
           new Subject { Id = Guid.NewGuid(), Name = "Filosofía", Code = "FIL502", Credits = 3 },
           new Subject { Id = Guid.NewGuid(), Name = "Gastronomia", Code = "GAS501", Credits = 6 },
           new Subject { Id = Guid.NewGuid(), Name = "Bases de datos", Code = "BAB502", Credits = 5 }
        };

        context.Subjects.AddRange(subjects);

        // Se crean estudiantes con materias inscritas
        var students = new List<Student>
        {
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Derain Julian Atehortua Montes",
                Document = "123456789",
                Email = "derain@q10.com",
                Subjects = new List<Subject> { subjects[0], subjects[1], subjects[2] }
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Yeris Paola Vergara Díaz",
                Document = "987654321",
                Email = "yvergara@q10.com",
                Subjects = new List<Subject> { subjects[3], subjects[6] }
            }
            ,
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Jesús Ortiz Paz",
                Document = "777777777",
                Email = "jortiz@q10.com",
                Subjects = new List<Subject> { subjects[4], subjects[5], subjects[7] }
            }
             ,
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Benito Martinez",
                Document = "2222222222",
                Email = "Benito@q10.com",
                Subjects = new List<Subject> { subjects[8], subjects[9]}
            }
            , new Student
            {
                Id = Guid.NewGuid(),
                Name = "Daniela Atehortua",
                Document = "1111111111",
                Email = "Daniela@q10.com",
                Subjects = new List<Subject> { subjects[7]}
            }
        };

        context.Students.AddRange(students);

        context.SaveChanges();
    }
}
