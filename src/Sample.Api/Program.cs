using Microsoft.AspNetCore.Mvc;
using Sample.Ado;
using Sample.Application;
using Sample.Domain;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("ConnectionStrings:Default").Value;

builder.Services.AddTransient<IUsersRepository>(repo => new UsersRepository(connectionString));
builder.Services.AddTransient<IUsersApplicationService, UsersApplicationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var migrator = new Migrations(connectionString);
migrator.Dispose();

await using (var scope = app.Services.CreateAsyncScope())
{
    var service = scope.ServiceProvider.GetService<IUsersApplicationService>();

    app.MapPost("/users", async (CreateUpdateUserDto input) =>
    {
        var dto = await service!.CreateAsync(input);

        return dto;
    }).WithName("CreateUser");
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var service = scope.ServiceProvider.GetService<IUsersApplicationService>();

    app.MapPut("/users/{id}", async (Guid id, CreateUpdateUserDto input) =>
    {
        var dto = await service!.UpdateAsync(id, input);

        return dto;
    }).WithName("UpdateUser");
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var service = scope.ServiceProvider.GetService<IUsersApplicationService>();

    app.MapGet("/users/{id}", async (Guid id) =>
    {
        var dto = await service!.GetAsync(id);

        return dto;
    }).WithName("GetUser");
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var service = scope.ServiceProvider.GetService<IUsersApplicationService>();

    app.MapGet("/users", async () =>
    {
        var dto = await service!.GetAllAsync();

        return dto;
    }).WithName("GetUsers");
}

app.Run();