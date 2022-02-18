using API.Data;
using API.Middleware;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("Store") ?? "Data Source=store.db";
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<DbInitializer>();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlite(connectionString);
});
builder.Services.AddCors();

var app = builder.Build();

var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

using(var scope = scopedFactory.CreateScope()){
    var service = scope.ServiceProvider.GetService<DbInitializer>();
    service.Initialize();
}

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   // app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(opt =>{
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
});
app.UseAuthorization();

app.MapControllers();

app.Run();
