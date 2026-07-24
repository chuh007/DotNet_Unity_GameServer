using System.ComponentModel.DataAnnotations;

namespace GameServer.Dtos;

/// <summary>
/// 회원가입 요청 형태. 클라이언트(Unity)가 JSON으로 보내면 자동으로 파싱되서 여기에 들어온다. 
/// Required같은 Validation헤더에 값을 넣으면 자동으로 검증해주고, 실패하면 400 BadRequest를 반환한다. 이때 ErrorMessage를 클라이언트에게 전달한다.
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "아이디는 필수입니다")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "아이디는 3자 이상 50자 이하로 입력해야 합니다")]
    public string Username { get; set; } = null!;
    //!는 널 용서 연산자. null 셋팅에 대한 워닝을 표기하지마. 내가 처리한다.. 라는 뜻

    [Required(ErrorMessage = "닉네임은 필수입니다")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "닉네임은 2자 이상 50자 이하로 입력해야 합니다")]
    public string Nickname { get; set; } = null!;

    [Required(ErrorMessage = "비밀번호는 필수입니다")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "비밀번호는 6자 이상 100자 이하로 입력해야 합니다")]
    public string Password { get; set; } = null!;
}
