using Together.Models;

namespace Together.Helpers
{
    public static class EnumExtensions
    {
        public static string ToFriendlyName(this OrganizationType type)
        {
            return type
                switch
            {
                OrganizationType.Corporate => "Doanh nghiệp",
                OrganizationType.School => "Trường học",
                OrganizationType.Community => "Cộng đồng",
                OrganizationType.Government => "Chính phủ",
                OrganizationType.NGO => "Phi chính phủ",
                _ => type.ToString()
            };
        }

        public static string ToFriendlyName(this OrganzationStatus status)
        {
            return status
                switch
            {
                OrganzationStatus.Pending => "Đang chờ duyệt",
                OrganzationStatus.Active => "Đang hoạt động",
                OrganzationStatus.Unactive => "Không hoạt động",
                OrganzationStatus.Rejected => "Bị từ chối",
                _ => status.ToString()
            };
        }

        public static string ToFriendlyName(this AccountRole role)
        {
            return role
                switch
            {
                AccountRole.Volunteer => "Tình nguyện viên",
                AccountRole.Staff => "Nhân viên tổ chức",
                AccountRole.Admin => "Quản trị viên",
                _ => role.ToString()
            };
        }

        public static string ToFriendlyName(this AccountStatus status)
        {
            return status
                switch
            {
                AccountStatus.Active => "Đang hoạt động",
                AccountStatus.Inactive => "Ngừng hoạt động",
                _ => status.ToString()
            };
        }

        public static string ToFriendlyName(this ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "Đang chờ duyệt",
                ApplicationStatus.Approved => "Đã được phê duyệt",
                ApplicationStatus.Rejected => "Bị từ chối",
                ApplicationStatus.Withdrawn => "Đã rút đơn",
                ApplicationStatus.Completed => "Hoàn thành",
                _ => status.ToString()
            };
        }

        public static string ToFriendlyName(this StaffRole role)
        {
            return role switch
            {
                StaffRole.Manager => "Quản lý",
                StaffRole.Reviewer => "Người đánh giá",
                StaffRole.Employee => "Nhân viên",
                _ => role.ToString()
            };
        }

        public static string ToFriendlyName(this ProjectStatus status)
        {
            return status switch
            {
                ProjectStatus.Draft => "Bản nháp",
                ProjectStatus.Planning => "Đang lên kế hoạch",
                ProjectStatus.Recruiting => "Đang tuyển dụng",
                ProjectStatus.Active => "Đang triển khai",
                ProjectStatus.Completed => "Hoàn thành",
                ProjectStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }

        public static string ToFriendlyName(this ProjectType type)
        {
            return type switch
            {
                ProjectType.Teaching => "Giảng dạy",
                ProjectType.MedicalSupport => "Hỗ trợ y tế",
                ProjectType.Infrastructure => "Cơ sở hạ tầng",
                ProjectType.Event => "Sự kiện",
                ProjectType.Training => "Đào tạo",
                ProjectType.Counseling => "Tư vấn",
                ProjectType.Other => "Khác",
                _ => type.ToString()
            };
        }

        public static string ToFriendlyName(this BlogStatus status)
        {
            return status switch
            {
                BlogStatus.Draft => "Bản nháp",
                BlogStatus.Pending => "Đang chờ duyệt",
                BlogStatus.Published => "Đang xuất bản",
                BlogStatus.Archived => "Lưu trữ",
                _ => status.ToString()
            };
        }
    }
}