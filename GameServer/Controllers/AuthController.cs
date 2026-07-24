using GameServer.Dtos;
using GameServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            UserResponse? result = await _authService.RegisterAsync(request);

            if (result is null)
            {
                //409에러헤더에 메시지 객체 Json으로 반환.
                return Conflict(new { message = "이미 사용중인 아이디입니다." });
            }

            //Rest API 규격은 반환시, Location 헤더에 새로 생성된 리소스의 URI를 포함해야 합니다. 첫번째가 Location, 뒤에가 반환응답.
            return Created($"/api/users/{result.Id}", result);
        }

    }
}
