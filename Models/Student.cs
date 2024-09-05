using System.Collections.Generic;

namespace CourseEquivalencyDesktop.Models;

public class Student : BaseModel
{
    public enum ProgramType
    {
        Computer,
        Electrical
    }

    public enum StreamType
    {
        Four,
        Eight
    }

    #region Fields
    private int id;
    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private string name;
    private string studentId;
    private University university;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private int universityId;
    private string? email;
    private string? notes;
    private ProgramType program;
    private StreamType stream;
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

    public string StudentId
    {
        get => studentId;
        set => SetField(ref studentId, value);
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

    public string? Email
    {
        get => email;
        set => SetField(ref email, value);
    }

    public string? Notes
    {
        get => notes;
        set => SetField(ref notes, value);
    }

    public ProgramType Program
    {
        get => program;
        set => SetField(ref program, value);
    }

    public StreamType Stream
    {
        get => stream;
        set => SetField(ref stream, value);
    }

    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // ReSharper disable UnassignedGetOnlyAutoProperty
    // ReSharper disable CollectionNeverUpdated.Global
    public ICollection<StudyPlan> StudyPlans { get; }
    // ReSharper restore CollectionNeverUpdated.Global
    // ReSharper restore UnassignedGetOnlyAutoProperty
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion
}
