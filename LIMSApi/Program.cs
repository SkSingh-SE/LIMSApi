using System.Security.Cryptography;
using System.Text;
using Hangfire;
using LIMSApi.Data;
using LIMSApi.Helpers;
using LIMSApi.Jobs;
using LIMSApi.Middleware;
using LIMSApi.Repositories;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services;
using LIMSApi.Services.Interface;
using LIMSApi.ServiceWORepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

string jwtSecret = builder.Configuration["Jwt:Secret"];

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // exact Angular dev origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // allow cookies/signalr creds
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<LIMSContext>(opt => opt.UseSqlServer(connectionString));

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
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IBankRepository,BankRepository>();
builder.Services.AddScoped<ICalibrationAgencyRepository, CalibrationAgencyRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICompanyCategoryRepository, CompanyCategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IDimensionalFactorRepository, DimensionalFactorRepository>();
builder.Services.AddScoped<IDisciplineRepository, DisciplineRepository>();
builder.Services.AddScoped<IDispatchModeRepository, DispatchModeRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEquipmentTypeRepository, EquipmentTypeRepository>();
builder.Services.AddScoped<IFileUploadRepository, FileUploadRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IHeatTreatmentRepository, HeatTreatmentRepository>();
builder.Services.AddScoped<IItemMasterRepository, ItemMasterRepository>();
builder.Services.AddScoped<ILaboratoryTestRepository, LaboratoryTestRepository>();
builder.Services.AddScoped<ILaboratoryTestRepository, LaboratoryTestRepository>();
builder.Services.AddScoped<ILabScopeRepository, LabScopeRepository>();
builder.Services.AddScoped<IMetalClassificationRepository, MetalClassificationRepository>();
builder.Services.AddScoped<IOEMRepository, OEMRepository>();
builder.Services.AddScoped<IParameterRepository, ParameterRepository>();
builder.Services.AddScoped<IParameterUnitRepository, ParameterUnitRepository>();
builder.Services.AddScoped<IProductConditionRepository, ProductConditionRepository>();
builder.Services.AddScoped<IProductSpecificationRepository, ProductSpecificationRepository>();
builder.Services.AddScoped<IRemarkRepository, RemarkRepository>();
builder.Services.AddScoped<ISiteActivityRepository, SiteActivityRepository>();
builder.Services.AddScoped<ISiteErrorRepository, SiteErrorRepository>();
builder.Services.AddScoped<ISpecificationHeaderRepository, SpecificationHeaderRepository>();
builder.Services.AddScoped<ISpecimenOrientationRepository, SpecimenOrientationRepository>();
builder.Services.AddScoped<ISpecimenTypeRepository, SpecimenTypeRepository>();
builder.Services.AddScoped<IStandardOrganizationRepository, StandardOrganizationRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ISubContractorRepository, SubContractorRepository>();
builder.Services.AddScoped<ISubGroupRepository, SubGroupRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<ITestGroupRepository, TestGroupRepository>();
builder.Services.AddScoped<ITestMasterRepository, TestMasterRepository>();
builder.Services.AddScoped<ITestMethodSpecificationRepository, TestMethodSpecificationRepository>();
builder.Services.AddScoped<ITestMethodStandardRepository, TestMethodStandardRepository>();
builder.Services.AddScoped<ITPIMasterRepository, TPIMasterRepository>();
builder.Services.AddScoped<IUniversalCodeTypeRepository, UniversalCodeTypeRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IInvoiceCaseConfigurationRepository, InvoiceCaseConfigurationRepository>();
builder.Services.AddScoped<IInvoiceCaseRepository, InvoiceCaseRepository>();
builder.Services.AddScoped<ICuttingPriceMasterRepository,CuttingPriceMasterRepository>();
builder.Services.AddScoped<IConfigurationRepository,ConfigurationRepository>();
builder.Services.AddScoped<IMenuRepository,MenuRepository>();
builder.Services.AddScoped<IRoleRepository,RoleRepository>();
builder.Services.AddScoped<IUserPermissionRepository,UserPermissionRepository>();
builder.Services.AddScoped<ISampleInwardRepository, SampleInwardRepository>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMaterialTestMappingRepository, MaterialTestMappingRepository>();
builder.Services.AddScoped<ISampleStatusRepository, SampleStatusRepository>();
builder.Services.AddScoped<ICuttingRepository, CuttingRepository>();
builder.Services.AddScoped<IProformaInvoiceRepository, ProformaInvoiceRepository>();


// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IBankService,BankService>();
builder.Services.AddScoped<ICalibrationAgencyService, CalibrationAgencyService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICompanyCategoryService, CompanyCategoryService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICourierService,CourierService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IDimensionalFactorService,DimensionalFactorService>();
builder.Services.AddScoped<IDisciplineService, DisciplineService>();
builder.Services.AddScoped<IDispatchModeService,DispatchModeService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEquipmentTypeService, EquipmentTypeService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IHeatTreatmentService, HeatTreatmentService>();
builder.Services.AddScoped<IItemMasterService, ItemMasterService>();
builder.Services.AddScoped<ILaboratoryTestService, LaboratoryTestService>();
builder.Services.AddScoped<ILabScopeService, LabScopeService>();
builder.Services.AddScoped<IMetalClassificationService, MetalClassificationService>();
builder.Services.AddScoped<IOEMService, OEMService>();
builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IParameterUnitService, ParameterUnitService>();
builder.Services.AddScoped<IProductConditionService, ProductConditionService>();
builder.Services.AddScoped<IProductSpecificationService, ProductSpecificationService>();
builder.Services.AddScoped<IRemarkService,RemarkService>();
builder.Services.AddScoped<ISiteActivityService, SiteActivityService>();
builder.Services.AddScoped<ISiteErrorService, SiteErrorService>();
builder.Services.AddScoped<ISpecificationHeaderService, SpecificationHeaderService>();
builder.Services.AddScoped<ISpecimenOrientationService, SpecimenOrientationService>();
builder.Services.AddScoped<ISpecimenTypeService, SpecimenTypeService>();
builder.Services.AddScoped<IStandardOrganizationService, StandardOrganizationService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ISubContractorService, SubContractorService>();
builder.Services.AddScoped<ISubGroupService, SubGroupService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ITaxService,TaxService>();
builder.Services.AddScoped<ITestGroupService, TestGroupService>();
builder.Services.AddScoped<ITestMethodSpecificationService, TestMethodSpecificationService>();
builder.Services.AddScoped<ITPIMasterService, TPIMasterService>();
builder.Services.AddScoped<IUniversalCodeTypeService, UniversalCodeTypeService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IInvoiceCaseConfigurationService, InvoiceCaseConfigurationService>();
builder.Services.AddScoped<IInvoiceCaseService, InvoiceCaseService>();
builder.Services.AddScoped<ICuttingPriceMasterService, CuttingPriceMasterService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<ISampleInwardService, SampleInwardService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
builder.Services.AddScoped<IMaterialTestMappingService, MaterialTestMappingService>();
builder.Services.AddScoped<ISampleStatusService, SampleStatusService>();
builder.Services.AddScoped<ICuttingService, CuttingService>();



//Service without Repo

builder.Services.AddScoped<ITestResultService, TestResultService>();
builder.Services.AddScoped<FormulaEvaluator>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportTemplateService, ReportTemplateService>();
builder.Services.AddScoped<IReportBlockGenerator, ReportBlockGenerator>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerAmendmentService, CustomerAmendmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Third party services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SMSService>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.AddScoped<InvoicePdfService>();

//builder.Services.AddSingleton<IConverter>(
//    new SynchronizedConverter(new PdfTools()));

// QuestPDF license (free Community)
QuestPDF.Settings.License = LicenseType.Community;

// your other services…


builder.Services.AddSignalR();

// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire");
// Schedule jobs directly
RecurringJob.AddOrUpdate<ReminderJob>("ReminderJob", x => x.Execute(), "0 0 9 * * *");

app.UseMiddleware<GeneralizedExceptionHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = ""; // ? Make Swagger the root (http://lims.com/)
    });
}
app.UseCors("AllowAngular");
app.MapHub<NotificationHub>("/hubs/notifications");



app.UseStaticFiles();
app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
