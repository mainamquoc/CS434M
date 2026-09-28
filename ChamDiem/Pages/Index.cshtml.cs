using ChamDiem.Models;
using ChamDiem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChamDiem.Pages;

public sealed class IndexModel(StudentService studentService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? StudentId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Status { get; set; } = "all";

    [BindProperty]
    public GradingInput Input { get; set; } = new();

    public IReadOnlyList<Student> Students { get; private set; } = [];
    public Student? SelectedStudent { get; private set; }
    public int TotalStudents { get; private set; }
    public int GradedStudents { get; private set; }
    public int PendingStudents { get; private set; }
    public decimal? AverageScore { get; private set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        LoadPageData(useBoundInput: false);
    }

    public IActionResult OnPostSaveDraft()
    {
        StudentId = Input.StudentId;
        ValidateScorePrecision();

        if (!ModelState.IsValid)
        {
            LoadPageData(useBoundInput: true);
            ErrorMessage = "Không thể lưu nháp. Vui lòng kiểm tra các điểm đã nhập.";
            return Page();
        }

        var result = studentService.SaveDraft(Input);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            LoadPageData(useBoundInput: true);
            return Page();
        }

        SuccessMessage = "Đã lưu bản nháp. Sinh viên vẫn ở trạng thái Chờ chấm.";
        return RedirectToPage(new { studentId = Input.StudentId, search = Search, status = Status });
    }

    public IActionResult OnPostConfirmScore()
    {
        StudentId = Input.StudentId;
        ValidateScorePrecision();
        ValidateConfirmationRequirements();

        if (!ModelState.IsValid)
        {
            LoadPageData(useBoundInput: true);
            ErrorMessage = "Chưa thể xác nhận điểm. Vui lòng hoàn thiện các trường được báo lỗi.";
            return Page();
        }

        var result = studentService.Confirm(Input);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            LoadPageData(useBoundInput: true);
            return Page();
        }

        SuccessMessage = "Đã xác nhận điểm thành công. Phiếu chấm đã được khóa.";
        return RedirectToPage(new { studentId = Input.StudentId, search = Search, status = Status });
    }

    public string Classification(decimal? score) => StudentService.GetClassification(score);

    private void LoadPageData(bool useBoundInput)
    {
        var allStudents = studentService.GetStudents();
        TotalStudents = allStudents.Count;
        GradedStudents = allStudents.Count(student => student.Status == GradingStatus.Graded);
        PendingStudents = TotalStudents - GradedStudents;

        var gradedScores = allStudents
            .Where(student => student.FinalScore.HasValue)
            .Select(student => student.FinalScore!.Value)
            .ToList();
        AverageScore = gradedScores.Count > 0
            ? Math.Round(gradedScores.Average(), 2, MidpointRounding.AwayFromZero)
            : null;

        IEnumerable<Student> filtered = allStudents;
        filtered = Status.ToLowerInvariant() switch
        {
            "pending" => filtered.Where(student => student.Status == GradingStatus.Pending),
            "graded" => filtered.Where(student => student.Status == GradingStatus.Graded),
            _ => filtered
        };

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search!.Trim();
            filtered = filtered.Where(student =>
                student.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                student.StudentCode.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        Students = filtered.ToList();
        SelectedStudent = allStudents.FirstOrDefault(student => student.Id == StudentId)
            ?? Students.FirstOrDefault();
        StudentId = SelectedStudent?.Id;

        if (!useBoundInput && SelectedStudent is not null)
        {
            Input = new GradingInput
            {
                StudentId = SelectedStudent.Id,
                AttendanceScore = SelectedStudent.AttendanceScore,
                ReportScore = SelectedStudent.ReportScore,
                DefenseScore = SelectedStudent.DefenseScore,
                LecturerFeedback = SelectedStudent.LecturerFeedback
            };
        }
    }

    private void ValidateScorePrecision()
    {
        ValidateOneDecimal(Input.AttendanceScore, nameof(Input.AttendanceScore));
        ValidateOneDecimal(Input.ReportScore, nameof(Input.ReportScore));
        ValidateOneDecimal(Input.DefenseScore, nameof(Input.DefenseScore));
    }

    private void ValidateOneDecimal(decimal? score, string propertyName)
    {
        if (score.HasValue && decimal.Round(score.Value, 1) != score.Value)
        {
            ModelState.AddModelError($"Input.{propertyName}", "Chỉ được nhập tối đa một chữ số thập phân.");
        }
    }

    private void ValidateConfirmationRequirements()
    {
        if (!Input.AttendanceScore.HasValue)
        {
            ModelState.AddModelError("Input.AttendanceScore", "Vui lòng nhập điểm chuyên cần.");
        }

        if (!Input.ReportScore.HasValue)
        {
            ModelState.AddModelError("Input.ReportScore", "Vui lòng nhập điểm báo cáo.");
        }

        if (!Input.DefenseScore.HasValue)
        {
            ModelState.AddModelError("Input.DefenseScore", "Vui lòng nhập điểm bảo vệ.");
        }

        var feedback = Input.LecturerFeedback?.Trim() ?? string.Empty;
        if (feedback.Length == 0)
        {
            ModelState.AddModelError("Input.LecturerFeedback", "Vui lòng nhập nhận xét của giảng viên.");
        }
        else if (feedback.Length < 30)
        {
            ModelState.AddModelError("Input.LecturerFeedback", "Nhận xét phải có tối thiểu 30 ký tự.");
        }
    }
}
