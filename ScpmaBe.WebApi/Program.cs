using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories;
using ScpmaBe.Repositories.Entities;
using ScpmaBe.Repositories.Interfaces;
using ScpmaBe.Repositories.Repo;
using ScpmaBe.Services;
using ScpmaBe.Services.Common;
using ScpmaBe.Services.Interfaces;
using ScpmaBe.Services.Models;
using ScpmaBe.WebApi.Converters;
using ScpmaBe.WebApi.Helpers;
using ScpmaBe.WebApi.Jobs;
using ScpmBe.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHostedService<ContractJob>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy/MM/dd HH:mm"));
        //options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter("HH:mm"));
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Start DI
builder.Services.AddSingleton<IHashHelper, HashHelper>();
// Fix for CS7036 and CS1526: Provide the required parameter 'httpContextAccessor' when creating an instance of Utils.
builder.Services.AddSingleton<Utils>(provider => new Utils(provider.GetRequiredService<IHttpContextAccessor>()));

// AppSettings
var appSettings = new AppSettings
{
    ApplicationUrl = builder.Configuration["AppSettings:ApplicationUrl"],
    HashSecretKey = builder.Configuration["AppSettings:HashSecretKey"],
    PaymentBaseUrl = builder.Configuration["AppSettings:PaymentBaseUrl"],
    TmnCode = builder.Configuration["AppSettings:TmnCode"],
};

builder.Services.AddSingleton(appSettings);

var cnnString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SCPMContext>((optionBuilder) =>
{
    optionBuilder.UseSqlServer(cnnString);
});

builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IFeedBackRepository, FeedBackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ITaskEachRepository,TaskEachRepository>();
builder.Services.AddScoped<ITaskEachService, TaskEachService>();
builder.Services.AddScoped<IAssignedTaskRepository, AssignedTaskRepository>();
builder.Services.AddScoped<IAssignedTaskService, AssignedTaskService>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IFloorService, FloorService>();
builder.Services.AddScoped<IParkingSpaceRepository, ParkingSpaceRepository>();
builder.Services.AddScoped<IParkingSpaceService, ParkingSpaceService>();
builder.Services.AddScoped<IParkingLotRepository, ParkingLotRepository>();
builder.Services.AddScoped<IParkingLotService, ParkingLotService>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IEntryExitLogRepository, EntryExitLogRepository>();
builder.Services.AddScoped<IEntryExitLogService, EntryExitLogService>();
builder.Services.AddScoped<IPaymentContractRepository, PaymentContractRepository>();
//builder.Services.AddScoped<IPaymentContractService, PaymentContractService>();
builder.Services.AddScoped<IParkingStatusSensorRepository, ParkingStatusSensorRepository>();
builder.Services.AddScoped<IParkingStatusSensorService, ParkingStatusSensorService>();

builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

await app.RunAsync();
