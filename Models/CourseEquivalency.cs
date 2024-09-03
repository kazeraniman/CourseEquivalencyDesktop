namespace CourseEquivalencyDesktop.Models;

public class CourseEquivalency : BaseModel
{
    #region Fields
    // Handled by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Course course;
    private Course equivalentCourse;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    #endregion

    #region Properties
    public Course Course
    {
        get => course;
        set => SetField(ref course, value);
    }

    public Course EquivalentCourse
    {
        get => equivalentCourse;
        set => SetField(ref equivalentCourse, value);
    }
    #endregion
}
