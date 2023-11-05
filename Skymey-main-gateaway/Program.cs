using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using Skymey_main_Gateway.Actions.JWT;
using Skymey_main_Gateway.Interfaces.JWT;
using Skymey_main_Gateway;
using AnSkymey_main_Gatewayevo.Data;
using Microsoft.EntityFrameworkCore;
using Skymey_main_gateaway.Data;

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyMethod()
           .AllowAnyHeader()
           .AllowAnyOrigin();
}));

var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions
{
    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
    ClientCertificateMode = ClientCertificateMode.AllowCertificate,
    ServerCertificate = new X509Certificate2("certificate_cert_out.pfx", "1234")

};
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
    options.HttpsPort = 5001;
});
builder.WebHost.UseUrls("http://192.168.1.85:5005;https://192.168.1.85:5007;");
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "SkymeyAPI", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            }, new string[]{}
        }
    });
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Parse("192.168.1.85"), 5005);
    serverOptions.Listen(IPAddress.Parse("192.168.1.85"), 5007, listenOptions =>
    {
        listenOptions.UseHttps("certificate_cert_out.pfx", "1234");
    });
});

#region Auth
builder.Services.AddOptions();
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings")); // Сопоставление JWTSettings с файлом конфигурации appsettings.json
builder.Services.Configure<UserSettings>(builder.Configuration.GetSection("ServiceSettings:UserAuth")); // Сопоставление JWTSettings с файлом конфигурации appsettings.json
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("ServiceSettings:MongoDb")); // Сопоставление JWTSettings с файлом конфигурации appsettings.json

var secretKey = builder.Configuration.GetSection("JWTSettings:SecretKey").Value; // Секретный код из appsettings.json
var issuer = builder.Configuration.GetSection("JWTSettings:Issuer").Value; // Издатель токена. Можно указать любое название
var audience = builder.Configuration.GetSection("JWTSettings:Audience").Value; // Пользователь токена. Можно указать любое название

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));


builder.Services.AddAuthentication(option => { // Указываем аутентификацию с помощью токенов
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters
    { // Задаем параметры валидации токена. Нужно проверять: Издатель, потребитель, ключ, срок действия
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        IssuerSigningKey = signingKey,
        ValidateIssuerSigningKey = true,
        LifetimeValidator = CustomLifetime.CustomLifetimeValidator
    };
});
#endregion
builder.Services.AddTransient<ITokenService, CreateJWTToken>();
builder.Services.AddTransient<UserSettings>();
builder.Services.AddTransient<MongoDbSettings>();
var app = builder.Build();

app.UseHttpsRedirection().UseCertificateForwarding().UseCors().UseHsts();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
