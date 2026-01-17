namespace EShopAPI.Requiests
{
    public class LoginRequestDTO
    {
        public string ?Username { get; set; }
        public string ?Password { get; set; }
    }
    public class RegisterDTO
    {
        public string ?Username { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }
        public string ?FullName { get; set; }
    }

}
