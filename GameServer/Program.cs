using GameServer.Data;
using GameServer.Entities;
using GameServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// OpenAPI(Swagger) 문서 — 개발 환경에서 API를 브라우저로 테스트할 수 있게 해줌.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "CasualRPG Game API";
        document.Info.Version = "0.0.1";
        document.Info.Description = "캐주얼 RPG 서버 REST API";
        //비동기 작업을 위한 람다이지만, 우리가 지금한건 동기작업이라 그냥 완료된 빈태스크를 리턴.
        return Task.CompletedTask;
    });
});

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 46)));
});

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CasualRpgServer");
    });
}

app.UseHttpsRedirection();

// 서버 동작 확인용 최소 엔드포인트.
app.MapGet("/", () => "CasualRpgServer is running.");

app.MapControllers();

app.Run();
