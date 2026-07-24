using GameServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>users 테이블. LINQ 쿼리의 시작점.</summary>
    public DbSet<User> Users => Set<User>();

    //이건 스키마 설계도야. 마이그레이션 시점에서 읽어서 설계도대로 만들어. 근데 마이그레이션을 읽을때 설계도가 변경되었다면 마이그레이션을 바꿔
    //뿐만 아니라 런타임에서 Linq를 SQL로 번역할때도 여기서 넘어온 설계도를 참조해
    // OnModelCreating은 EF Core가 모델을 구성할 때 호출되는 메서드로, 엔티티의 속성, 관계, 인덱스 등을 정의할 수 있다.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Username은 로그인 아이디이므로 중복 불가(고유 인덱스).
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}
