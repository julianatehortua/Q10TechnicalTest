using Microsoft.AspNetCore.Mvc;
using Q10Solution.Application.DTOs;
using Q10Solution.Application.Interfaces;

namespace Q10Solution.Web.Controllers;

public class SubjectsController : Controller
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    //Metodo Index
    public async Task<IActionResult> Index()
    {
        var subjects = await _subjectService.GetAllAsync();
        return View(subjects);
    }

    //Metodo GET para crear
    public IActionResult Create()
    {
        return View();
    }

    //Metodo POST para crear
    [HttpPost]
    public async Task<IActionResult> Create(SubjectDto subject)
    {
        if (!ModelState.IsValid)
            return View(subject);

        await _subjectService.CreateAsync(subject);
        return RedirectToAction(nameof(Index));
    }

    //Metodo GET para editar
    public async Task<IActionResult> Edit(Guid id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        if (subject == null) return NotFound();
        return View(subject);
    }

    //Metodo POST para editar
    [HttpPost]
    public async Task<IActionResult> Edit(SubjectDto subject)
    {
        if (!ModelState.IsValid)
            return View(subject);

        await _subjectService.UpdateAsync(subject);
        return RedirectToAction(nameof(Index));
    }

    //Metodo GET para eliminar
    public async Task<IActionResult> Delete(Guid id)
    {
        var subject = await _subjectService.GetByIdAsync(id);
        if (subject == null) return NotFound();
        return View(subject);
    }

    //Metodo POST para crear
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _subjectService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
