using System.Security.Cryptography;
using System.Text;
using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Middleware;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

string jwtSecret = builder.Configuration["Jwt:Secret"];

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<LIMSContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Logging (Optional)
builder.Logging.AddConsole();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LIMS API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}' (without quotes) into the field below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {} // No specific scopes required
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(option =>
{
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});




// Register AuthService with a parameter from _configuration
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<LoggedInUserProvider>();


builder.Services.AddScoped<IAuthService>(provider =>
{
    var userRepository = provider.GetRequiredService<IUserRepository>();
    var logger = provider.GetRequiredService<ILogger<AuthService>>();
    var configuration = provider.GetRequiredService<IConfiguration>();
    var emailService = provider.GetRequiredService<EmailService>();
    return new AuthService(userRepository, logger, jwtSecret, configuration,emailService);
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ISiteActivityRepository, SiteActivityRepository>();
builder.Services.AddScoped<ISiteErrorRepository, SiteErrorRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IFileUploadRepository, FileUploadRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ISpecimenTypeRepository, SpecimenTypeRepository>();
builder.Services.AddScoped<ITestMasterRepository, TestMasterRepository>();
builder.Services.AddScoped<ITestMethodRepository, TestMethodRepository>();
builder.Services.AddScoped<ITestMethodStandardRepository, TestMethodStandardRepository>();
builder.Services.AddScoped<ITestGroupRepository, TestGroupRepository>();
builder.Services.AddScoped<ICompanyCategoryRepository, CompanyCategoryRepository>();
builder.Services.AddScoped<IItemMasterRepository, ItemMasterRepository>();
builder.Services.AddScoped<IDispatchModeRepository, DispatchModeRepository>();
builder.Services.AddScoped<IRemarkRepository, RemarkRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IBankRepository,BankRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISubContractorRepository, SubContractorRepository>();
builder.Services.AddScoped<ITPIMasterRepository, TPIMasterRepository>();
builder.Services.AddScoped<ILabScopeRepository, LabScopeRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IDisciplineRepository, DisciplineRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ISubGroupRepository, SubGroupRepository>();
builder.Services.AddScoped<IOEMRepository, OEMRepository>();
builder.Services.AddScoped<ICalibrationAgencyRepository, CalibrationAgencyRepository>();
builder.Services.AddScoped<ITestMethodRepository, TestMethodRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDimensionalFactorRepository, DimensionalFactorRepository>();
builder.Services.AddScoped<IHeatTreatmentRepository, HeatTreatmentRepository>();
builder.Services.AddScoped<IParameterRepository, ParameterRepository>();
builder.Services.AddScoped<IProductConditionRepository, ProductConditionRepository>();
builder.Services.AddScoped<ISpecimenOrientationRepository, SpecimenOrientationRepository>();
builder.Services.AddScoped<IParameterUnitRepository, ParameterUnitRepository>();
builder.Services.AddScoped<IProductSpecificationRepository, ProductSpecificationRepository>();



// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ISiteActivityService, SiteActivityService>();
builder.Services.AddScoped<ISiteErrorService, SiteErrorService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISpecimenTypeService, SpecimenTypeService>();
builder.Services.AddScoped<ITestGroupService, TestGroupService>();
builder.Services.AddScoped<ICompanyCategoryService, CompanyCategoryService>();
builder.Services.AddScoped<IItemMasterService, ItemMasterService>();
builder.Services.AddScoped<IDispatchModeService,DispatchModeService>();
builder.Services.AddScoped<IRemarkService,RemarkService>();
builder.Services.AddScoped<ITaxService,TaxService>();
builder.Services.AddScoped<IBankService,BankService>();
builder.Services.AddScoped<ICourierService,CourierService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISubContractorService, SubContractorService>();
builder.Services.AddScoped<ITPIMasterService, TPIMasterService>();
builder.Services.AddScoped<ILabScopeService, LabScopeService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IDisciplineService, DisciplineService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ISubGroupService, SubGroupService>();
builder.Services.AddScoped<IOEMService, OEMService>();
builder.Services.AddScoped<ICalibrationAgencyService, CalibrationAgencyService>();
builder.Services.AddScoped<ITestMethodService, TestMethodService>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IDimensionalFactorService,DimensionalFactorService>();
builder.Services.AddScoped<IHeatTreatmentService, HeatTreatmentService>();
builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IProductConditionService, ProductConditionService>();
builder.Services.AddScoped<ISpecimenOrientationService, SpecimenOrientationService>();
builder.Services.AddScoped<IParameterUnitService, ParameterUnitService>();
builder.Services.AddScoped<IProductSpecificationService, ProductSpecificationService>();



// Third party services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SMSService>();
builder.Services.AddScoped<WhatsAppService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseMiddleware<GeneralizedExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
