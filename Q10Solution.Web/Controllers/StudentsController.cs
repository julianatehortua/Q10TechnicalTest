using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Q10Solution.Application.DTOs;
using Q10Solution.Application.Interfaces;

namespace Q10Solution.Web.Controllers;

public class StudentsController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ISubjectService _subjectService;

    public StudentsController(IStudentService studentService, ISubjectService subjectService)
    {
        _studentService = studentService;
        _subjectService = subjectService;
    }

    //Metodo Index
    public async Task<IActionResult> Index()
    {
        var students = await _studentService.GetAllAsync();
        return View(students);
    }

    //Metodo GET para crear
    public async Task<IActionResult> Create()
    {
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        return View();
    }

    //Metodo POST para crear
    [HttpPost]
    public async Task<IActionResult> Create(StudentDto student)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = await _subjectService.GetAllAsync();
            return View(student);
        }

        await _studentService.CreateAsync(student);
        return RedirectToAction(nameof(Index));
    }

    //Metodo GET para editar
    public async Task<IActionResult> Edit(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();

        ViewBag.Subjects = await _subjectService.GetAllAsync();
        return View(student);
    }

    //Metodo POST para editar
    [HttpPost]
    public async Task<IActionResult> Edit(StudentDto student)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = await _subjectService.GetAllAsync();
            return View(student);
        }

        await _studentService.UpdateAsync(student);
        return RedirectToAction(nameof(Index));
    }

    //Metodo GET para eliminar
    public async Task<IActionResult> Delete(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    //Metodo POST para editar
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _studentService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    //Metodo GET para asignar materias
    [HttpGet]
    public async Task<IActionResult> AssignSubjects(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();

        var allSubjects = await _subjectService.GetAllAsync();
        ViewBag.Subjects = allSubjects.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = $"{s.Name} ({s.Credits} créditos)"
        }).ToList();


        return View(student);
    }

    //Metodo POST para asignar materias
    [HttpPost]
    public async Task<IActionResult> AssignSubjects(StudentDto dto)
    {
        try
        {
            var student = await _studentService.GetByIdAsync(dto.Id);
            if (student == null) return NotFound();

            await _studentService.AssignSubjectsAsync(dto.Id, dto.SubjectIds);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var allSubjects = await _subjectService.GetAllAsync();
        ViewBag.Subjects = allSubjects.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = $"{s.Name} ({s.Credits} créditos)"
        }).ToList();

        return View(dto);
    }


}
