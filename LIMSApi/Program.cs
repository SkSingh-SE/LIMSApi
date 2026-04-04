using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Hangfire;
using Hangfire.Dashboard;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

// Load secrets from non-committed file (overrides appsettings.json values)
builder.Configuration.AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

string jwtSecret = builder.Configuration["Jwt:Secret"];

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Allow all origins (ngrok URLs change frequently)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.

// Global upload size limit — safety net for all multipart form endpoints
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddControllers(options =>
    {
        // Require authentication on ALL controllers by default.
        // Controllers that need anonymous access must use [AllowAnonymous].
        var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<LIMSContext>(opt => opt
    .UseSqlServer(connectionString)
    .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Phase 9: Memory cache for workflow definitions
builder.Services.AddMemoryCache();

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

    option.Events = new JwtBearerEvents
    {
        // SignalR sends JWT via query string (?access_token=xxx) because
        // WebSockets cannot send custom HTTP headers. This extracts it.
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new
            {
                message = "Token expired or invalid. Please login again."
            });
        }
    };
});
builder.Services.AddAuthorization();

// ===== Rate Limiting =====
// Global rate limit: 100 requests per minute per IP (protects all endpoints)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("GlobalLimit", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Too many requests. Please try again later."
        }, cancellationToken);
    };
});

// Login rate limiter (custom — supports reset on successful login)
builder.Services.AddSingleton<LoginRateLimiter>();

// Register AuthService with a parameter from _configuration
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<LoggedInUserProvider>();


builder.Services.AddScoped<IAuthService, AuthService>();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
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
builder.Services.AddScoped<IEquipmentReferenceMaterialRepository, EquipmentReferenceMaterialRepository>();
builder.Services.AddScoped<IEquipmentTypeRepository, EquipmentTypeRepository>();
builder.Services.AddScoped<IFileUploadRepository, FileUploadRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IHeatTreatmentRepository, HeatTreatmentRepository>();
builder.Services.AddScoped<IHeatTreatmentCategoryRepository, HeatTreatmentCategoryRepository>();
builder.Services.AddScoped<ICoolingMediumRepository, CoolingMediumRepository>();
builder.Services.AddScoped<IParameterCategoryRepository, ParameterCategoryRepository>();
builder.Services.AddScoped<IProductFormRepository, ProductFormRepository>();
builder.Services.AddScoped<ISpecimenOrientationCategoryRepository, SpecimenOrientationCategoryRepository>();
builder.Services.AddScoped<IProductConditionCategoryRepository, ProductConditionCategoryRepository>();
builder.Services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
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
builder.Services.AddScoped<IProductTestGroupRepository, ProductTestGroupRepository>();
builder.Services.AddScoped<IProductSpecificationGradeRepository, ProductSpecificationGradeRepository>();
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
builder.Services.AddScoped<ICuttingPriceMasterRepository, CuttingPriceMasterRepository>();
builder.Services.AddScoped<ISamplePreparationMasterRepository, SamplePreparationMasterRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<ISampleInwardRepository, SampleInwardRepository>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ISampleStatusRepository, SampleStatusRepository>();
builder.Services.AddScoped<ICuttingRepository, CuttingRepository>();
builder.Services.AddScoped<IProformaInvoiceRepository, ProformaInvoiceRepository>();
builder.Services.AddScoped<INablRepository, NablRepository>();


// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<ICalibrationAgencyService, CalibrationAgencyService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICompanyCategoryService, CompanyCategoryService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICourierService, CourierService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IDimensionalFactorService, DimensionalFactorService>();
builder.Services.AddScoped<IDisciplineService, DisciplineService>();
builder.Services.AddScoped<IDispatchModeService, DispatchModeService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEquipmentReferenceMaterialService, EquipmentReferenceMaterialService>();
builder.Services.AddScoped<IEquipmentTypeService, EquipmentTypeService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IHeatTreatmentService, HeatTreatmentService>();
builder.Services.AddScoped<IHeatTreatmentCategoryService, HeatTreatmentCategoryService>();
builder.Services.AddScoped<ICoolingMediumService, CoolingMediumService>();
builder.Services.AddScoped<IParameterCategoryService, ParameterCategoryService>();
builder.Services.AddScoped<IProductFormService, ProductFormService>();
builder.Services.AddScoped<ISpecimenOrientationCategoryService, SpecimenOrientationCategoryService>();
builder.Services.AddScoped<IProductConditionCategoryService, ProductConditionCategoryService>();
builder.Services.AddScoped<IPropertyTypeService, PropertyTypeService>();
builder.Services.AddScoped<IItemMasterService, ItemMasterService>();
builder.Services.AddScoped<ILaboratoryTestService, LaboratoryTestService>();
builder.Services.AddScoped<ITestMasterService, TestMasterService>();
builder.Services.AddScoped<ILabScopeService, LabScopeService>();
builder.Services.AddScoped<IMetalClassificationService, MetalClassificationService>();
builder.Services.AddScoped<IOEMService, OEMService>();
builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IParameterUnitService, ParameterUnitService>();
builder.Services.AddScoped<IProductConditionService, ProductConditionService>();
builder.Services.AddScoped<IProductSpecificationService, ProductSpecificationService>();
builder.Services.AddScoped<IProductTestGroupService, ProductTestGroupService>();
builder.Services.AddScoped<IProductSpecificationGradeService, ProductSpecificationGradeService>();
builder.Services.AddScoped<IRemarkService, RemarkService>();
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
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<ITestGroupService, TestGroupService>();
builder.Services.AddScoped<ITestMethodSpecificationService, TestMethodSpecificationService>();
builder.Services.AddScoped<ITestMethodStandardService, TestMethodStandardService>();
builder.Services.AddScoped<ITPIMasterService, TPIMasterService>();
builder.Services.AddScoped<IUniversalCodeTypeService, UniversalCodeTypeService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IInvoiceCaseConfigurationService, InvoiceCaseConfigurationService>();
builder.Services.AddScoped<IInvoiceCaseService, InvoiceCaseService>();
builder.Services.AddScoped<ICuttingPriceMasterService, CuttingPriceMasterService>();
builder.Services.AddScoped<ISamplePreparationMasterService, SamplePreparationMasterService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<ISampleInwardService, SampleInwardService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
builder.Services.AddScoped<ISampleStatusService, SampleStatusService>();
builder.Services.AddScoped<ICuttingService, CuttingService>();
builder.Services.AddScoped<IPricingEngine, PricingEngine>();
builder.Services.AddScoped<IPriceCalculationService, PriceCalculationService>();
builder.Services.AddScoped<IFinancialYearService, FinancialYearService>();
builder.Services.AddScoped<INablService, NablService>();
builder.Services.AddScoped<INablAuditService, NablAuditService>();
builder.Services.AddScoped<IPriceDimensionTypeService, PriceDimensionTypeService>();
builder.Services.AddScoped<IPriceDimensionTypeRepository, PriceDimensionTypeRepository>();
builder.Services.AddScoped<ICustomerPurchaseOrderService, CustomerPurchaseOrderService>();
builder.Services.AddScoped<ICustomerPurchaseOrderRepository, CustomerPurchaseOrderRepository>();

//Service without Repo

builder.Services.AddScoped<ITestResultService, TestResultService>();
builder.Services.AddScoped<ITestPriceCalculationService, TestPriceCalculationService>();
builder.Services.AddScoped<FormulaEvaluator>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportTemplateService, ReportTemplateService>();
builder.Services.AddScoped<IReportBlockGenerator, ReportBlockGenerator>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerAmendmentService, CustomerAmendmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentGatingService, PaymentGatingService>();
builder.Services.AddScoped<ICustomerLedgerService, CustomerLedgerService>();
builder.Services.AddScoped<IMachineIntegrationService, MachineIntegrationService>();
builder.Services.AddScoped<INablScopeValidationService, NablScopeValidationService>();
builder.Services.AddScoped<DispatchService>();
builder.Services.AddScoped<CaseClosureService>();
builder.Services.AddScoped<BillingSettlementService>();
builder.Services.AddScoped<ISettingsService,SettingsService>();

// Register Dashboard Service
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Register Test Auto-Suggest Service
builder.Services.AddScoped<ITestAutoSuggestService, TestAutoSuggestService>();

// Register TPI Inspection Service
builder.Services.AddScoped<ITpiService, TpiService>();

// Third party services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SMSService>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.AddScoped<InvoicePdfService>();
builder.Services.AddScoped<TemplateService>();

//builder.Services.AddSingleton<IConverter>(
//    new SynchronizedConverter(new PdfTools()));

// QuestPDF license (free Community)
QuestPDF.Settings.License = LicenseType.Community;

// your other services�


builder.Services.AddSignalR();

// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

// --------------------
// Seed essential data on first deployment
// --------------------
await LIMSApi.DataSeeder.SeedAsync(app);

// --------------------
// Infrastructure (no auth needed)
// --------------------
app.UseStaticFiles();
app.UseHttpsRedirection();

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    await next();
});

// --------------------
// Routing
// --------------------
app.UseRouting();

// --------------------
// Security (CRITICAL ORDER)
// --------------------
app.UseCors("AllowAngular");

app.UseAuthentication();   // MUST exist
app.UseAuthorization();    // MUST follow authentication
// app.UseRateLimiter();   // DISABLED during testing — re-enable for production

// --------------------
// Global Exception Handling (AFTER auth)
// --------------------
app.UseMiddleware<GeneralizedExceptionHandlingMiddleware>();

// --------------------
// Swagger (safe after auth)
// --------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = ""; // root
});

// --------------------
// Hangfire
// --------------------
app.UseHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
{
    Authorization = new[] { new HangfireAuthFilter(app.Environment) }
});

// Jobs
RecurringJob.AddOrUpdate<ReminderJob>(
    "ReminderJob",
    x => x.Execute(),
    "0 */12 * * *");

RecurringJob.AddOrUpdate<MonthlyBillingJob>(
    "monthly-billing",
    job => job.ExecuteMonthly(),
    "0 9 1 * *"); // 1st of every month at 9 AM

RecurringJob.AddOrUpdate<MonthlyBillingJob>(
    "weekly-billing",
    job => job.ExecuteWeekly(),
    "0 9 * * MON"); // Every Monday at 9 AM

RecurringJob.AddOrUpdate<CustomIntervalBillingJob>(
    "custom-interval-billing",
    job => job.Execute(),
    "0 2 * * *"); // Daily at 2 AM — checks BillingEvery customers

RecurringJob.AddOrUpdate<PaymentOverdueJob>(
    "payment-overdue-check",
    job => job.Execute(),
    "0 8 * * *"); // Daily at 8 AM — notifies Accounts of overdue invoices

RecurringJob.AddOrUpdate<POExpiryJob>(
    "po-expiry-check",
    job => job.Execute(),
    "0 8 * * *"); // Daily at 8 AM — notifies about POs expiring within 7 days

RecurringJob.AddOrUpdate<EnvironmentMonitoringJob>(
    "environment-monitoring-daily",
    job => job.Execute(),
    "0 8 * * *"); // Daily at 8 AM — auto-generate daily environment records

RecurringJob.AddOrUpdate<TestUsageStatsJob>(
    "test-usage-stats",
    x => x.Execute(),
    "0 2 * * *"); // Daily at 2 AM

RecurringJob.AddOrUpdate<VersionReviewReminderJob>(
    "version-review-reminder",
    x => x.Execute(),
    "0 9 * * MON"); // Every Monday at 9 AM

// --------------------
// Endpoints
// --------------------
app.MapControllers().RequireRateLimiting("GlobalLimit");
app.MapHub<NotificationHub>("/hubs/notifications")
    .RequireAuthorization();

app.Run();

// Simple Hangfire auth filter — Dev: allow all, Prod: Admin only
public class HangfireAuthFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _env;
    public HangfireAuthFilter(IWebHostEnvironment env) => _env = env;

    public bool Authorize(Hangfire.Dashboard.DashboardContext dashboardContext)
    {
        if (_env.IsDevelopment()) return true;

        var httpContext = dashboardContext.GetHttpContext();
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true) return false;

        var role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
    }
}

