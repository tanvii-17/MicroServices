namespace Mango.Services.CouponAPI.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }        //an individual object or a list
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
