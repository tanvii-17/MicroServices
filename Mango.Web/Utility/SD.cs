namespace Mango.Web.Utility
{
    public class SD
    {
        //contains the base Url for Services
        public static string CouponAPIBase {  get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";

        //to store key-value pair for Cookie
        public const string TokenCookie = "JwtToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
