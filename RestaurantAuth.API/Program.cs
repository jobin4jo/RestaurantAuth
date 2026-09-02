using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAuth.Application.Repositories;
using RestaurantAuth.Application.Service;
using RestaurantAuth.Domain.Common.Password;
using RestaurantAuth.Domain.JWT;
using RestaurantAuth.Infrastructure;
using RestaurantAuth.Infrastructure.IService;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RestaurantDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();  
builder.Services.AddScoped<IServiceRepo,ServiceRepo>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher> ();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
