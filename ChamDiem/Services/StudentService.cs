using ChamDiem.Models;

namespace ChamDiem.Services;

public sealed class StudentService
{
    private readonly object _syncRoot = new();
    private readonly List<Student> _students = CreateSeedData();

    public IReadOnlyList<Student> GetStudents()
    {
        lock (_syncRoot)
        {
            return _students.ToList();
        }
    }

    public Student? GetById(int id)
    {
        lock (_syncRoot)
        {
            return _students.FirstOrDefault(student => student.Id == id);
        }
    }

    public SaveResult SaveDraft(GradingInput input)
    {
        lock (_syncRoot)
        {
            var student = _students.FirstOrDefault(item => item.Id == input.StudentId);
            if (student is null)
            {
                return SaveResult.Failure("Không tìm thấy sinh viên cần chấm điểm.");
            }

            if (student.IsConfirmed || student.Status == GradingStatus.Graded)
            {
                return SaveResult.Failure("Điểm của sinh viên đã được xác nhận và không thể chỉnh sửa.");
            }

            student.AttendanceScore = input.AttendanceScore;
            student.ReportScore = input.ReportScore;
            student.DefenseScore = input.DefenseScore;
            student.LecturerFeedback = input.LecturerFeedback?.Trim() ?? string.Empty;
            student.FinalScore = CalculateFinalScore(input.AttendanceScore, input.ReportScore, input.DefenseScore);
            return SaveResult.Success();
        }
    }

    public SaveResult Confirm(GradingInput input)
    {
        lock (_syncRoot)
        {
            var student = _students.FirstOrDefault(item => item.Id == input.StudentId);
            if (student is null)
            {
                return SaveResult.Failure("Không tìm thấy sinh viên cần chấm điểm.");
            }

            if (student.IsConfirmed || student.Status == GradingStatus.Graded)
            {
                return SaveResult.Failure("Điểm của sinh viên đã được xác nhận và không thể chỉnh sửa.");
            }

            var finalScore = CalculateFinalScore(input.AttendanceScore, input.ReportScore, input.DefenseScore);
            if (finalScore is null)
            {
                return SaveResult.Failure("Vui lòng nhập đầy đủ ba đầu điểm trước khi xác nhận.");
            }

            student.AttendanceScore = input.AttendanceScore;
            student.ReportScore = input.ReportScore;
            student.DefenseScore = input.DefenseScore;
            student.FinalScore = finalScore;
            student.LecturerFeedback = input.LecturerFeedback?.Trim() ?? string.Empty;
            student.IsConfirmed = true;
            student.Status = GradingStatus.Graded;
            return SaveResult.Success();
        }
    }

    public static decimal? CalculateFinalScore(decimal? attendance, decimal? report, decimal? defense)
    {
        if (!attendance.HasValue || !report.HasValue || !defense.HasValue)
        {
            return null;
        }

        return Math.Round(
            attendance.Value * 0.2m + report.Value * 0.3m + defense.Value * 0.5m,
            2,
            MidpointRounding.AwayFromZero);
    }

    public static string GetClassification(decimal? score) => score switch
    {
        >= 8.5m => "A - Giỏi",
        >= 7.0m => "B - Khá",
        >= 5.5m => "C - Trung bình",
        >= 4.0m => "D - Yếu",
        not null => "F - Kém",
        _ => "Chưa đủ điểm"
    };

    private static List<Student> CreateSeedData() =>
    [
        new Student
        {
            Id = 1,
            StudentCode = "29211145181",
            FullName = "Mai Nam Quốc",
            ClassName = "CS434M",
            Major = "Kỹ thuật Phần mềm",
            InternshipTerm = "Đợt 1 (2024-2025)",
            CompanyName = "FPT Software Đà Nẵng",
            InternshipPosition = "Backend Intern",
            CompanySupervisor = "Nguyễn Hoàng Nam (Tech Lead)",
            SubmissionDate = new DateTime(2024, 10, 15),
            Status = GradingStatus.Pending,
            Report = new InternshipReport
            {
                Topic = "Xây dựng hệ thống phân tán vi dịch vụ quản trị thực tập và xác thực tập trung OAuth2/JWT dựa trên Spring Cloud và Kubernetes.",
                ReportFileName = "BaoCao_ThucTap_MaiNamQuoc.pdf",
                SourceFileName = "SourceCode_Project_UC11.zip",
                SubmittedOnTime = true,
                PlagiarismRate = 1.2m
            },
            EnterpriseEvaluation = new EnterpriseEvaluation
            {
                OverallScore = 9.2m,
                DisciplineScore = 9.5m,
                ProfessionalScore = 9.0m,
                TeamworkScore = 9.0m,
                Comment = "Sinh viên nắm vững kiến thức nền tảng, hoàn thành tốt các module sprint được giao đúng hạn. Có tinh thần chủ động tìm hiểu công nghệ mới và hòa nhập nhanh với văn hóa đội ngũ phát triển.",
                IsDigitallySigned = true
            }
        },
        new Student
        {
            Id = 2,
            StudentCode = "29211164915",
            FullName = "Trương Nhật Trường",
            ClassName = "CS434M",
            Major = "Kỹ thuật Phần mềm",
            InternshipTerm = "Đợt 1 (2024-2025)",
            CompanyName = "FPT Telecom",
            InternshipPosition = "Frontend Intern",
            CompanySupervisor = "Trần Minh Tâm (Project Manager)",
            SubmissionDate = new DateTime(2024, 10, 14),
            Status = GradingStatus.Graded,
            AttendanceScore = 8.5m,
            ReportScore = 8.4m,
            DefenseScore = 8.6m,
            FinalScore = 8.52m,
            LecturerFeedback = "Sinh viên có ý thức tốt, báo cáo rõ ràng và trình bày sản phẩm tự tin. Cần bổ sung thêm kiểm thử tự động cho dự án.",
            IsConfirmed = true,
            Report = new InternshipReport
            {
                Topic = "Phát triển cổng chăm sóc khách hàng đa nền tảng cho dịch vụ viễn thông.",
                ReportFileName = "BaoCao_ThucTap_TruongNhatTruong.pdf",
                SourceFileName = "CustomerPortal_Source.zip",
                SubmittedOnTime = true,
                PlagiarismRate = 2.0m
            },
            EnterpriseEvaluation = new EnterpriseEvaluation
            {
                OverallScore = 8.7m,
                DisciplineScore = 9.0m,
                ProfessionalScore = 8.5m,
                TeamworkScore = 8.6m,
                Comment = "Thực hiện nhiệm vụ nghiêm túc, giao tiếp tốt và có khả năng tiếp nhận phản hồi nhanh.",
                IsDigitallySigned = true
            }
        },
        new Student
        {
            Id = 3,
            StudentCode = "21IT02",
            FullName = "Mai Văn Chung",
            ClassName = "CS430",
            Major = "Công nghệ Thông tin",
            InternshipTerm = "Đợt 1 (2024-2025)",
            CompanyName = "VNG Corporation",
            InternshipPosition = "Data Engineer Intern",
            CompanySupervisor = "Lê Hoàng Anh (Engineering Lead)",
            SubmissionDate = new DateTime(2024, 10, 12),
            Status = GradingStatus.Graded,
            AttendanceScore = 9.0m,
            ReportScore = 9.0m,
            DefenseScore = 9.0m,
            FinalScore = 9.0m,
            LecturerFeedback = "Sinh viên hoàn thành xuất sắc mục tiêu thực tập, có tư duy hệ thống tốt và làm chủ được giải pháp đã triển khai.",
            IsConfirmed = true,
            Report = new InternshipReport
            {
                Topic = "Xây dựng pipeline xử lý dữ liệu thời gian thực phục vụ phân tích hành vi người dùng.",
                ReportFileName = "BaoCao_ThucTap_MaiVanChung.pdf",
                SourceFileName = "RealtimePipeline_Source.zip",
                SubmittedOnTime = true,
                PlagiarismRate = 0.8m
            },
            EnterpriseEvaluation = new EnterpriseEvaluation
            {
                OverallScore = 9.3m,
                DisciplineScore = 9.2m,
                ProfessionalScore = 9.5m,
                TeamworkScore = 9.1m,
                Comment = "Có năng lực chuyên môn tốt, chủ động tối ưu hiệu năng và hỗ trợ tích cực cho các thành viên trong nhóm.",
                IsDigitallySigned = true
            }
        },
        new Student
        {
            Id = 4,
            StudentCode = "21KT03",
            FullName = "Lê Thị Hoa",
            ClassName = "AC412",
            Major = "Hệ thống Thông tin Quản lý",
            InternshipTerm = "Đợt 1 (2024-2025)",
            CompanyName = "KPMG Vietnam",
            InternshipPosition = "IT Audit Intern",
            CompanySupervisor = "Phạm Thu Hà (Senior Consultant)",
            SubmissionDate = new DateTime(2024, 10, 18),
            Status = GradingStatus.Pending,
            Report = new InternshipReport
            {
                Topic = "Đánh giá kiểm soát công nghệ thông tin trong quy trình kiểm toán doanh nghiệp.",
                ReportFileName = "BaoCao_ThucTap_LeThiHoa.pdf",
                SourceFileName = "Audit_Checklist.zip",
                SubmittedOnTime = true,
                PlagiarismRate = 1.5m
            },
            EnterpriseEvaluation = new EnterpriseEvaluation
            {
                OverallScore = 8.8m,
                DisciplineScore = 9.2m,
                ProfessionalScore = 8.6m,
                TeamworkScore = 8.7m,
                Comment = "Cẩn thận, trách nhiệm và tuân thủ tốt quy trình bảo mật dữ liệu của doanh nghiệp.",
                IsDigitallySigned = true
            }
        }
    ];
}

public sealed record SaveResult(bool IsSuccess, string? ErrorMessage)
{
    public static SaveResult Success() => new(true, null);
    public static SaveResult Failure(string message) => new(false, message);
}
