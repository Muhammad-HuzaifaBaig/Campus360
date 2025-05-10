using Campus360.Data.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

public class CoursesBySemesterViewModel
{
    public Semester? SelectedSemester { get; set; }
    public List<SelectListItem> SemesterOptions { get; set; }
    public List<Course> Courses { get; set; } = new();
}
