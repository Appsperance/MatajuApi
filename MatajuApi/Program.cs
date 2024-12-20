using MatajuApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using MatajuApi.Helpers;
using MatajuApi.Models;
using MatajuApi.Repositories;
using Microsoft.EntityFrameworkCore;

/*******************
 * Web Host Builder
 *******************/
var builder = WebApplication.CreateBuilder(args);


/*****************   DI Container   ******************/
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// DbContext 등록
string? connectionStringForMySql = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionStringForMySql, ServerVersion.AutoDetect(connectionStringForMySql))); //MySQL ADO Provider

// JWT 인증 설정
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                                                                                        {
                                                                                          options.TokenValidationParameters = new TokenValidationParameters
                                                                                                                              {
                                                                                                                                ValidateIssuer = true, ValidateAudience = true,
                                                                                                                                ValidateLifetime = true,
                                                                                                                                ValidateIssuerSigningKey = true,
                                                                                                                                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                                                                                                                ValidAudience =
                                                                                                                                  builder.Configuration["Jwt:Audience"],
                                                                                                                                IssuerSigningKey =
                                                                                                                                  JwtHelper.GetPublicKey(builder.Configuration),
                                                                                                                                RoleClaimType =
                                                                                                                                  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                                                                                                                              };

                                                                                          // 쿠키에서 토큰을 읽는 이벤트 설정
                                                                                          options.Events = new JwtBearerEvents
                                                                                                           {
                                                                                                             OnMessageReceived = context =>
                                                                                                                                 {
                                                                                                                                   var token =
                                                                                                                                     context.HttpContext.Request.Cookies["token"];
                                                                                                                                   if (!string.IsNullOrEmpty(token))
                                                                                                                                   {
                                                                                                                                     // 현재 token값이 "Bearer {토큰}" 형태로 저장되었다면 "Bearer " 제거
                                                                                                                                     if (token.StartsWith("Bearer "))
                                                                                                                                     {
                                                                                                                                       token = token.Substring("Bearer ".Length)
                                                                                                                                                    .Trim();
                                                                                                                                     }

                                                                                                                                     context.Token = token;
                                                                                                                                   }

                                                                                                                                   return Task.CompletedTask;
                                                                                                                                 }
                                                                                                           };
                                                                                        });

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
                                                          {
                                                            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                                                          });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Swagger에 JWT 인증 스키마 추가
builder.Services.AddSwaggerGen(options =>
                               {
                                 options.SwaggerDoc("v1", new OpenApiInfo { Title = "MatajuApi", Version = "v1" });

                                 // Bearer 인증 스키마 정의
                                 options.AddSecurityDefinition("Bearer",
                                                               new OpenApiSecurityScheme
                                                               {
                                                                 In = ParameterLocation.Header, Name = "Authorization", Type = SecuritySchemeType.ApiKey, Scheme = "Bearer",
                                                                 BearerFormat = "JWT", Description = "Bearer schem을 사용한 JWT Authorization header. 예제: \"Bearer {token}\""
                                                               });

                                 // Bearer 인증 스키마를 기본 인증 방식으로 추가
                                 options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                                {
                                                                  {
                                                                    new OpenApiSecurityScheme
                                                                    {
                                                                      Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                                                                    },
                                                                    Array.Empty<string>()
                                                                  }
                                                                });
                                 // 앱의 환경 변수 설정으로 서버 베이스 경로 변경가능하게. 예: 호스트뒤에 "/dev"를 api 베이스로 설정가능
                                 var swaggerBasePath = Environment.GetEnvironmentVariable("SwaggerBasePath") ?? string.Empty;
                                 if (!string.IsNullOrEmpty(swaggerBasePath))
                                 {
                                   options.AddServer(new OpenApiServer { Url = swaggerBasePath, Description = "Base URL for the API" });
                                 }
                               });

//환경에 따른 테이블 레포지토리 선택 
if (builder.Environment.IsEnvironment("Local"))
{
  builder.Services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();
  builder.Services.AddSingleton<IRepository<House>, InMemoryRepository<House>>();
  builder.Services.AddSingleton<IRepository<Unit>, InMemoryRepository<Unit>>();
  builder.Services.AddSingleton<IRepository<Booking>, InMemoryRepository<Booking>>();
} else
{
  builder.Services.AddScoped<IRepository<User>, DbEfCoreRepository<User>>();
  builder.Services.AddScoped<IRepository<House>, DbEfCoreRepository<House>>();
  builder.Services.AddScoped<IRepository<Unit>, DbEfCoreRepository<Unit>>();
  builder.Services.AddScoped<IRepository<Booking>, DbEfCoreRepository<Booking>>();
}


//IHttpClientFactory && 현재 요청 컨텍스트 접근 설정(호스트 정보)
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


/********************************************
// * HTTP request 파이프라인에 미들웨어 추가
// ******************************************/
if (app.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Local"))
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseStaticFiles(); //wwwroot 정적리소스 폴더 지원 추가
app.UseHttpsRedirection();


// 인증 및 권한 미들웨어 추가
app.UseAuthentication();
app.UseAuthorization();

// MVC 라우트 추가
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
// 데이터 시딩
using (IServiceScope? scope = app.Services.CreateScope())
{
  IServiceProvider? scopedServices = scope.ServiceProvider;
  IRepository<House>? houseRepo = scopedServices.GetRequiredService<IRepository<House>>();
  IRepository<Unit>? unitRepo = scopedServices.GetRequiredService<IRepository<Unit>>();
  IRepository<User>? userRepo = scopedServices.GetRequiredService<IRepository<User>>();

  DataSeeder.SeedData(houseRepo, unitRepo, userRepo);
}

/***************
 * Run the host
 **************/
app.Run();