using GameServer.Data;
using GameServer.Dtos;
using GameServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher; //User타입에 대한 해셔를 등록

    public AuthService(AppDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// 회원가입. 성공하면 생성된 유저 정보를, 아이디가 이미 있으면 null을 반환한다.
    /// </summary>
    public async Task<UserResponse?> RegisterAsync(RegisterRequest request)
    {
        // 1) 아이디 중복검사.
        bool exists = await _db.Users.AnyAsync(u => u.Username == request.Username);
        if (exists)
            return null;

        // 2) User 엔티티 생성
        User user = new User
        {
            Username = request.Username,
            Nickname = request.Nickname,
            //골드와 CreateAt은 엔티티 기본값으로 사용.
        };

        // 3) 비밀번호 해싱
        //인자인 user는 구분을 위한 용도이고 실제로 Hash함수에서 user를 사용하진 않아.
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        // 4) DB에 저장
        _db.Users.Add(user); //Entity를 넣으면 EfCore가 자동으로 SQL을 만들어서 Insert해줌.
        await _db.SaveChangesAsync(); //Insert SQL 실행

        // 5) UserResponse로 변환 후 반환
        return UserResponse.FromEntity(user);

    }
}

