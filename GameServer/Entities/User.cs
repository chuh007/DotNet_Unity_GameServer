using System.ComponentModel.DataAnnotations;

namespace GameServer.Entities;

public class User
{
    /// <summary>기본키(PK). 자동 증가. DB 내부 식별용.</summary>
    public int Id { get; set; }

    /// <summary>로그인 계정 아이디. 고유(unique)해야 함.</summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    /// <summary>화면에 표시되는 이름(닉네임).</summary>
    [Required]
    [MaxLength(50)]
    public string Nickname { get; set; } = null!;

    /// <summary>해시된 비밀번호. 원문은 절대 저장하지 않는다.</summary>
    [Required]
    public string PasswordHash { get; set; } = null!;

    /// <summary>보유 골드. (계정 공용 재화 — 캐릭터가 아니라 계정에 둔다)</summary>
    public long Gold { get; set; }

    /// <summary>계정 생성 시각(UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
