using System;

namespace Networking.Dtos
{
    // 서버의 요청/응답 DTO와 1:1 대응. Newtonsoft가 역직렬화 시 대소문자를 무시하고 매칭하며,
    // 직렬화는 ApiClient.JsonSettings(camelCase)를 따른다.
    
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
    }
    
    
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public long Gold { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}