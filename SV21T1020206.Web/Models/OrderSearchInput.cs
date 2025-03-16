namespace SV21T1020206.Web.Models
{
    public class OrderSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Trạng thái của đơn hàng cần tìm (mặc định = 0)
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// Chuỗi chứa khoảng thời gian cần tìm (dd/MM/yyyy - dd/MM/yyyy)
        /// </summary>
        public string TimeRange { get; set; } = "";

        /// <summary>
        /// Lấy thời điểm bắt đầu dựa vào TimeRange
        /// </summary>
        public DateTime? FromTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;

                string[] times = TimeRange.Split('-');
                if (times.Length != 2)
                    return null;

                return times[0].Trim().ToDateTime();
            }
        }

        /// <summary>
        /// Lấy thời điểm kết thúc dựa vào TimeRange (cuối ngày)
        /// </summary>
        public DateTime? ToTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TimeRange))
                    return null;

                string[] times = TimeRange.Split('-');
                if (times.Length != 2)
                    return null;

                DateTime? value = times[1].Trim().ToDateTime();
                if (value.HasValue)
                {
                    // Lấy cuối ngày (23:59:59.999)
                    value = value.Value.Date.AddDays(1).AddTicks(-1);
                }
                return value;
            }
        }
    }

    /// <summary>
    /// Lớp mở rộng để hỗ trợ chuyển đổi chuỗi sang DateTime
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Chuyển đổi chuỗi sang DateTime (định dạng dd/MM/yyyy)
        /// </summary>
        public static DateTime? ToDateTime(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }
    }
}
