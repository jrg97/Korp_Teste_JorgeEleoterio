using Microsoft.EntityFrameworkCore;
using Faturamento.API.Data;
using Microsoft.Extensions.Http.Resilience;
using Faturamento.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FaturamentoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient("EstoqueApi", client =>{ client.BaseAddress = new Uri("http://localhost:5144");}).AddStandardResilienceHandler();
builder.Services.AddCors(options =>{options.AddPolicy("PermitirAngular", policy =>{policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();});});
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("PermitirAngular");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
