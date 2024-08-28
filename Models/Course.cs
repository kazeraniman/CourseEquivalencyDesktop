﻿namespace CourseEquivalencyDesktop.Models;

public class Course : ModelBase
{
    #region Fields
    private int id;
    private string name;
    private string courseId;
    private int universityId;
    private University university;
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
    #endregion
}
