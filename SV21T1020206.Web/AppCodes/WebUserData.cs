using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020206.Web
{
    /// <summary>
    /// Những thông tin cần dùng để mô tả danh tính người dùng
    /// </summary>
    public class WebUserData
    {
        public string UserID { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Role {  get; set; }


        /// <summary>
        /// Tạo chứng nhận để ghi nhận danh tính người dùng
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            // Tạo danh sách các claim, mỗi claim lưu trữ 1 thông tin danh tính của người dùng 
            List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserID), UserID),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
                new Claim(nameof(Photo), Photo),
            };
            if(Role != null)
            {
                foreach(var role in Role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
