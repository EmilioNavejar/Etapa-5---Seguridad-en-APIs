using System.Text;
using BankAPI.Data;
using BankAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//DBContext
builder.Services.AddSqlServer<BankContext>(builder.Configuration.GetConnectionString("BankConnection"));

//Service layer
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountTypeService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
AddJwtBearer(options=>{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey=true,
    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    ValidateIssuer = false,
    ValidateAudience = false
  };
});


builder.Services.AddAuthorization(options=>{
  options.AddPolicy("SuperAdmin", policy=>policy.RequireClaim("AdminType","Super"));
  options.AddPolicy("IsClient", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Client"));
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
