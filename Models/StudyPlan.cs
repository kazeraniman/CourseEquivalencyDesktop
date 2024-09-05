using System;
using System.Collections.Generic;

namespace CourseEquivalencyDesktop.Models;

// Outside the class due to nesting issues in the axaml
public enum StudyPlanStatus
{
    Pending,
    Cancelled,
    Complete
}

public class StudyPlan : BaseModel
{
    public enum SeasonalTerm
    {
        Fall,
        Winter,
        Spring
    }

    public enum AcademicTerm
    {
        A1,
        B1,
        A2,
        B2,
        A3,
        B3,
        A4,
        B4
    }

    #region Fields
    private int id;

    // TODO: Should these be DateTimeOffset? Do we care with a local DB?
    private DateTime updatedAt;
    private DateTime? dueDate;

    private int studentId;
    private int destinationUniversityId;
    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Student student;
    private University destinationUniversity;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private StudyPlanStatus status;
    private string? notes;
    private int year;
    private SeasonalTerm seasonalTerm;
    private AcademicTerm academicTerm;
    #endregion

    #region Properties
    public int Id
    {
        get => id;
        set => SetField(ref id, value);
    }

    public DateTime UpdatedAt
    {
        get => updatedAt;
        set => SetField(ref updatedAt, value);
    }

    public DateTime? DueDate
    {
        get => dueDate;
        set => SetField(ref dueDate, value);
    }

    public int StudentId
    {
        get => studentId;
        set => SetField(ref studentId, value);
    }

    public Student Student
    {
        get => student;
        set => SetField(ref student, value);
    }

    public int DestinationUniversityId
    {
        get => destinationUniversityId;
        set => SetField(ref destinationUniversityId, value);
    }

    public University DestinationUniversity
    {
        get => destinationUniversity;
        set => SetField(ref destinationUniversity, value);
    }

    public StudyPlanStatus Status
    {
        get => status;
        set => SetField(ref status, value);
    }

    public string? Notes
    {
        get => notes;
        set => SetField(ref notes, value);
    }

    public int Year
    {
        get => year;
        set => SetField(ref year, value);
    }

    public SeasonalTerm Seasonal
    {
        get => seasonalTerm;
        set => SetField(ref seasonalTerm, value);
    }

    public AcademicTerm Academic
    {
        get => academicTerm;
        set => SetField(ref academicTerm, value);
    }

    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable UnassignedGetOnlyAutoProperty
    // ReSharper disable CollectionNeverUpdated.Global
    public ICollection<Course> HomeUniversityCourses { get; }
    public ICollection<Course> DestinationUniversityCourses { get; }
    // ReSharper restore CollectionNeverUpdated.Global
    // ReSharper restore UnassignedGetOnlyAutoProperty
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion
}
