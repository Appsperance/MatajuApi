using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;

/*******************
 * Web Host Builder
 *******************/
var builder = WebApplication.CreateBuilder(args);

// JWT Public Key 로드
string publicKey = builder.Configuration["Jwt:PublicKey"];
var rsa = RSA.Create();
rsa.ImportFromPem(publicKey.ToCharArray());
var signingKey = new RsaSecurityKey(rsa);

/* DI Container   ******************/
// JWT 인증 설정
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
                     {
                         options.TokenValidationParameters = new TokenValidationParameters
                                                             {
                                                                 ValidateIssuer = true,
                                                                 ValidateAudience = true,
                                                                 ValidateLifetime = true,
                                                                 ValidateIssuerSigningKey = true,
                                                                 ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                                                 ValidAudience = builder.Configuration["Jwt:Audience"],
                                                                 IssuerSigningKey = signingKey
                                                             };
                     });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Swagger에 JWT 인증 스키마 추가
builder.Services.AddSwaggerGen(c =>
                               {
                                   c.SwaggerDoc("v1", new OpenApiInfo { Title = "MatajuApi", Version = "v1" });

                                   // Bearer 인증 스키마 정의
                                   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                                     {
                                                                         In = ParameterLocation.Header,
                                                                         Name = "Authorization",
                                                                         Type = SecuritySchemeType.ApiKey,
                                                                         Scheme = "Bearer",
                                                                         BearerFormat = "JWT",
                                                                         Description =
                                                                             "Bearer schem을 사용한 JWT Authorization header. 예제: \"Bearer {token}\""
                                                                     });

                                   // Bearer 인증 스키마를 기본 인증 방식으로 추가
                                   c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                            {
                                                                {
                                                                    new OpenApiSecurityScheme
                                                                    {
                                                                        Reference = new OpenApiReference
                                                                                    {
                                                                                        Type = ReferenceType.SecurityScheme,
                                                                                        Id = "Bearer"
                                                                                    }
                                                                    },
                                                                    Array.Empty<string>()
                                                                }
                                                            });
                               });

var app = builder.Build();


/********************************************
// * HTTP request 파이프라인에 미들웨어 추가
// ******************************************/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// 인증 및 권한 미들웨어 추가
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

/***************
 * Run the host
 **************/
app.Run();