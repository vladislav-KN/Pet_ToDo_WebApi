using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pet_ToDo_WebApi.Data;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<ToDoContext>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



using (var scope = app.Services.CreateScope())
{
    ToDoContext context = scope.ServiceProvider.GetRequiredService<ToDoContext>();
    context.Database.EnsureCreated();
}
app.UseHttpsRedirection();
  

app.Run();

 
