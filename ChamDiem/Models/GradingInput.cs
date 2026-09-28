using System.ComponentModel.DataAnnotations;

namespace ChamDiem.Models;

public sealed class GradingInput
{
    public int StudentId { get; set; }

    [Display(Name = "Chuyên cần và ý thức")]
    [Range(typeof(decimal), "0", "10", ErrorMessage = "Điểm phải nằm trong khoảng 0 đến 10.")]
    public decimal? AttendanceScore { get; set; }

    [Display(Name = "Chất lượng báo cáo")]
    [Range(typeof(decimal), "0", "10", ErrorMessage = "Điểm phải nằm trong khoảng 0 đến 10.")]
    public decimal? ReportScore { get; set; }

    [Display(Name = "Kết quả và bảo vệ")]
    [Range(typeof(decimal), "0", "10", ErrorMessage = "Điểm phải nằm trong khoảng 0 đến 10.")]
    public decimal? DefenseScore { get; set; }

    [Display(Name = "Nhận xét của giảng viên")]
    public string? LecturerFeedback { get; set; }
}
