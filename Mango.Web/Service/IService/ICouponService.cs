using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> CreateCouponsAsync(CouponDto coupon);
        Task<ResponseDto?> UpdateCouponsAsync(CouponDto coupon);
        Task<ResponseDto?> DeleteCouponsAsync(int id);
    }
}
