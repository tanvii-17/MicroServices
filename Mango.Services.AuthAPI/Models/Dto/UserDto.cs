namespace Mango.Services.AuthAPI.Models.Dto
{
    public class UserDto
    {
        public string Id { get; set; }          //default Guid setup for Id in identity
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
