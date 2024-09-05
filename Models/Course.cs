using System.Collections.Generic;

namespace CourseEquivalencyDesktop.Models;

public class Course : BaseModel
{
    #region Fields
    private int id;
    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private string name;
    private string courseId;
    private University university;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private int universityId;
    private string? url;
    private string? description;
    #endregion

    #region Properties
    public int Id
    {
        get => id;
        set => SetField(ref id, value);
    }

    public string Name
    {
        get => name;
        set => SetField(ref name, value);
    }

    public string CourseId
    {
        get => courseId;
        set => SetField(ref courseId, value);
    }

    public int UniversityId
    {
        get => universityId;
        set => SetField(ref universityId, value);
    }

    public University University
    {
        get => university;
        set => SetField(ref university, value);
    }

    public string? Url
    {
        get => url;
        set => SetField(ref url, value);
    }

    public string? Description
    {
        get => description;
        set => SetField(ref description, value);
    }

    // Handled by EF Core
    // ReSharper disable UnassignedGetOnlyAutoProperty
    // ReSharper disable CollectionNeverUpdated.Global
    public ICollection<Course> Equivalencies { get; } = [];

    public ICollection<Course> EquivalenciesOf { get; } = [];

    public ICollection<StudyPlan> HomeUniversityCoursesStudyPlans { get; } = [];

    public ICollection<StudyPlan> DestinationUniversityCoursesStudyPlans { get; } = [];
    // ReSharper restore CollectionNeverUpdated.Global
    // ReSharper restore UnassignedGetOnlyAutoProperty
    #endregion

    #region Object
    public override string ToString()
    {
        return $"{CourseId} - {Name}";
    }
    #endregion
}
