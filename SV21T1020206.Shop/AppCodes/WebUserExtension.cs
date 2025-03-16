using System.Security.Claims;

namespace SV21T1020206.Shop
{
    public static class WebUserExtension
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return null;

                var userData = new WebUserData();

                userData.UserID = principal.FindFirstValue(nameof(userData.UserID)) ?? "";
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName)) ?? "";
                userData.DisplayName = principal.FindFirstValue(nameof(userData.DisplayName)) ?? "";
                userData.Photo = principal.FindFirstValue(nameof(userData.Photo)) ?? "";

                userData.Role = new List<string>();
                foreach (var role in principal.FindAll(ClaimTypes.Role))
                {
                    userData.Role.Add(role.Value);
                }
                return userData;
            }
            catch
            {
                return null;
            }
        }
    }
}
