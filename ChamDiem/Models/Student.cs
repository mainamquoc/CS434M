namespace ChamDiem.Models;

public enum GradingStatus
{
    Pending,
    Graded
}

public sealed class Student
{
    public int Id { get; init; }
    public string StudentCode { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string ClassName { get; init; } = string.Empty;
    public string Major { get; init; } = string.Empty;
    public string InternshipTerm { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string InternshipPosition { get; init; } = string.Empty;
    public string CompanySupervisor { get; init; } = string.Empty;
    public DateTime SubmissionDate { get; init; }
    public GradingStatus Status { get; set; }
    public decimal? AttendanceScore { get; set; }
    public decimal? ReportScore { get; set; }
    public decimal? DefenseScore { get; set; }
    public decimal? FinalScore { get; set; }
    public string LecturerFeedback { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public InternshipReport Report { get; init; } = new();
    public EnterpriseEvaluation EnterpriseEvaluation { get; init; } = new();

    public string Initials => string.Join(string.Empty,
        FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .TakeLast(2)
            .Select(part => char.ToUpperInvariant(part[0])));
}

public sealed class InternshipReport
{
    public string Topic { get; init; } = string.Empty;
    public string ReportFileName { get; init; } = string.Empty;
    public string SourceFileName { get; init; } = string.Empty;
    public bool SubmittedOnTime { get; init; }
    public decimal? PlagiarismRate { get; init; }
}

public sealed class EnterpriseEvaluation
{
    public decimal OverallScore { get; init; }
    public decimal DisciplineScore { get; init; }
    public decimal ProfessionalScore { get; init; }
    public decimal TeamworkScore { get; init; }
    public string Comment { get; init; } = string.Empty;
    public bool IsDigitallySigned { get; init; }
}
