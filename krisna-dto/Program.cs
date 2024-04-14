using krisna_dto.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BookData>();
builder.Services.AddScoped<UserData>();

//builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors(builder =>
//{
    //builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

    //     builder.WithOrigins("https://localhost:3000", "https://localhost:3001").WithMethods("GET", "POST").AllowAnyHeader();
//});

app.UseAuthorization();

app.MapControllers();

app.Run();

