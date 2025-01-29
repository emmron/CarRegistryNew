var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAzureStaticApp",
        policy =>
        {
            policy.WithOrigins("https://*.azurestaticapps.net")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAzureStaticApp");

// In-memory car list for demo
var cars = new List<Car>
{
    new() { Id = Guid.NewGuid().ToString(), Make = "BMW", Model = "M3", Year = 2023, RegistrationStatus = "Pending" },
    new() { Id = Guid.NewGuid().ToString(), Make = "Mercedes", Model = "AMG GT", Year = 2023, RegistrationStatus = "Registered" },
    new() { Id = Guid.NewGuid().ToString(), Make = "Porsche", Model = "911", Year = 2022, RegistrationStatus = "Pending" }
};

// Endpoints
app.MapGet("/api/cars", () => cars)
    .WithName("GetCars")
    .WithOpenApi();

app.MapPost("/api/cars", (Car car) =>
{
    car.Id = Guid.NewGuid().ToString();
    cars.Add(car);
    return Results.Created($"/api/cars/{car.Id}", car);
})
.WithName("CreateCar")
.WithOpenApi();

app.MapPut("/api/cars/{id}/status", (string id, string status) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    if (car == null) return Results.NotFound();
    
    car.RegistrationStatus = status;
    return Results.Ok(car);
})
.WithName("UpdateCarStatus")
.WithOpenApi();

app.Run();

record Car
{
    public required string Id { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required int Year { get; set; }
    public required string RegistrationStatus { get; set; }
}
