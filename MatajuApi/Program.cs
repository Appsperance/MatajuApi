using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

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
builder.Services.AddSwaggerGen();

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