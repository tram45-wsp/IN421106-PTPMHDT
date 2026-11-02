using EduHub.Application.Clubs;
using EduHub.Domain.Interfaces;
using EduHub.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Đăng ký services (Dependency Injection) ──────────────────────────────
// TODO 6a: đăng ký IClubRepository → InMemoryClubRepository (AddScoped)
builder.Services.AddScoped<IClubRepository, InMemoryClubRepository>();
// TODO 6b: đăng ký IMemberRepository → InMemoryMemberRepository (AddScoped)
builder.Services.AddScoped<IMemberRepository, InMemoryMemberRepository>();
// TODO 6c: đăng ký 4 handler: GetClubsQueryHandler, GetClubByIdQueryHandler,
//          CreateClubCommandHandler, DeleteClubCommandHandler (AddScoped)
builder.Services.AddScoped<GetClubsQueryHandler>();
builder.Services.AddScoped<GetClubByIdQueryHandler>();
builder.Services.AddScoped<CreateClubCommandHandler>();
builder.Services.AddScoped<DeleteClubCommandHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// TODO 7: cấu hình Swagger — SwaggerDoc "v1" với Title = "EduHub API"
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EduHub API", Version = "v1" });
});

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

// Giải thích:
// TODO 6a & 6b: Đăng ký ánh xạ giữa Interface ở tầng Domain và lớp cài đặt thực tế ở tầng Infrastructure với vòng đời `AddScoped` (mỗi HTTP request nhận một bản thể mới).
// TODO 6c: Đăng ký các lớp Handler của tầng Application vào DI container để Controller có thể tự động nhận chúng thông qua cơ chế constructor injection.
// TODO 7: Cấu hình `AddSwaggerGen` để thiết lập tiêu đề (`Title = "EduHub API"`) và phiên bản cho tài liệu API hiển thị trên trang giao diện Swagger UI.
