using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.ServiceWORepo
{
    public class SettingsService : ISettingsService
    {
        private readonly LIMSContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IFileUploadService _fileUploadService;
        private readonly IFinancialYearService _fyService;

        public SettingsService(LIMSContext db, IWebHostEnvironment env, IFileUploadService fileUploadService, IFinancialYearService fyService)
        {
            _db = db;
            _env = env;
            _fileUploadService = fileUploadService;
            _fyService = fyService;
        }

        public async Task<(Organization, NablAccreditation?, NumberingConfig[], GstConfig?, FinancialYear?, AuthorizedSignatory[])> GetAllAsync(long organizationId = 0, CancellationToken cancellationToken = default)
        {
            // if organizationId not provided, use the first active organization in the system
            if (organizationId == 0)
            {
                var firstOrg = await _db.Organizations.FirstOrDefaultAsync(cancellationToken);
                if (firstOrg == null)
                {
                    // Auto-create a default organization for first-time setup
                    firstOrg = new Organization { LabName = "Default Lab", LabCode = "LAB001" };
                    _db.Organizations.Add(firstOrg);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                organizationId = firstOrg.Id;
            }

            var org = await _db.Organizations.FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken)
                ?? throw new InvalidOperationException("Organization not found");
            var nabl = await _db.NablAccreditations.FirstOrDefaultAsync(x => x.OrganizationId == organizationId, cancellationToken);
            var numbering = await _db.NumberingConfigs.Where(x => x.OrganizationId == organizationId).ToArrayAsync(cancellationToken);
            var gst = await _db.GstConfigs.FirstOrDefaultAsync(x => x.OrganizationId == organizationId, cancellationToken);
            var year = await _db.FinancialYears.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.IsCurrent, cancellationToken);
            var signatories = await _db.AuthorizedSignatories.Where(x => x.OrganizationId == organizationId).ToArrayAsync(cancellationToken);
            return (org, nabl, numbering, gst, year, signatories);
        }

        public async Task<Organization> SaveOrganizationAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(organization.LabName))
                throw new ArgumentException("LabName is required");
            if (organization.Id == 0)
                _db.Organizations.Add(organization);
            else
                _db.Organizations.Update(organization);
            await _db.SaveChangesAsync(cancellationToken);
            return organization;
        }

        public async Task<NablAccreditation> SaveNablAsync(NablAccreditation nabl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nabl.CertificateNumber))
                throw new ArgumentException("CertificateNumber is required");
            if (nabl.Id == 0)
                _db.NablAccreditations.Add(nabl);
            else
                _db.NablAccreditations.Update(nabl);
            await _db.SaveChangesAsync(cancellationToken);
            return nabl;
        }

        public async Task<NumberingConfig> SaveNumberingAsync(NumberingConfig numbering, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(numbering.ModuleName))
                throw new ArgumentException("ModuleName is required");
            if (numbering.Id == 0)
                _db.NumberingConfigs.Add(numbering);
            else
                _db.NumberingConfigs.Update(numbering);
            await _db.SaveChangesAsync(cancellationToken);
            return numbering;
        }

        public async Task<GstConfig> SaveGstAsync(GstConfig gst, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(gst.GstNumber))
                throw new ArgumentException("GstNumber is required");
            if (gst.Id == 0)
                _db.GstConfigs.Add(gst);
            else
                _db.GstConfigs.Update(gst);
            await _db.SaveChangesAsync(cancellationToken);
            return gst;
        }

        public async Task<FinancialYear> SaveFinancialYearAsync(FinancialYear year, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(year.Year))
                throw new ArgumentException("Year is required");
            if (year.Id == 0)
                _db.FinancialYears.Add(year);
            else
                _db.FinancialYears.Update(year);
            await _db.SaveChangesAsync(cancellationToken);

            // Set as current with audit trail if IsCurrent flag is set
            if (year.IsCurrent)
            {
                var currentUserId = LIMSApi.Helpers.LoggedInUserProvider.CurrentUser?.EmployeeID ?? 0;
                await _fyService.SetCurrentFinancialYearAsync(year.Id, currentUserId, "Changed via Settings");
            }

            return year;
        }

        public async Task<AuthorizedSignatory> SaveSignatoryAsync(AuthorizedSignatory signatory, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(signatory.Name))
                throw new ArgumentException("Name is required");
            if (signatory.Id == 0)
                _db.AuthorizedSignatories.Add(signatory);
            else
                _db.AuthorizedSignatories.Update(signatory);
            await _db.SaveChangesAsync(cancellationToken);
            return signatory;
        }

        public async Task SaveAllAsync(
            Organization organization,
            NablAccreditation? nabl,
            NumberingConfig[] numbering,
            GstConfig? gst,
            FinancialYear? year,
            AuthorizedSignatory[] signatories,
            CancellationToken cancellationToken = default)
        {
            // Transaction support structure
            using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await SaveOrganizationAsync(organization, cancellationToken);
                if (nabl != null) await SaveNablAsync(nabl, cancellationToken);
                foreach (var num in numbering) await SaveNumberingAsync(num, cancellationToken);
                if (gst != null) await SaveGstAsync(gst, cancellationToken);
                if (year != null) await SaveFinancialYearAsync(year, cancellationToken);
                foreach (var sign in signatories) await SaveSignatoryAsync(sign, cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<string> UploadOrganizationLogoAsync(long organizationId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Organization, null, organizationId.ToString());
            var relativePath = uploaded.FilePath;

            var org = await _db.Organizations.FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);
            if (org != null)
            {
                org.OrganizationLogo = relativePath;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return relativePath;
        }

        public async Task<string> UploadOrganizationLogoAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            // find default organization (first) and reuse existing logic
            var org = await _db.Organizations.FirstOrDefaultAsync(cancellationToken);
            if (org == null) throw new InvalidOperationException("Organization not found");
            return await UploadOrganizationLogoAsync(org.Id, file, env, cancellationToken);
        }

        public async Task<string> UploadNablCertificateAsync(long nablId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Nabl, null, nablId.ToString());
            var relativePath = uploaded.FilePath;

            var nabl = await _db.NablAccreditations.FirstOrDefaultAsync(x => x.Id == nablId, cancellationToken);
            if (nabl != null)
            {
                nabl.CertificatePath = relativePath;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return relativePath;
        }

        public async Task<string> UploadNablCertificateAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            // ensure a nabl record exists for default organization
            var org = await _db.Organizations.FirstOrDefaultAsync(cancellationToken);
            if (org == null) throw new InvalidOperationException("Organization not found");
            var nabl = await _db.NablAccreditations.FirstOrDefaultAsync(x => x.OrganizationId == org.Id, cancellationToken);
            if (nabl == null)
            {
                nabl = new NablAccreditation { CertificateNumber = string.Empty, IssueDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddYears(1), OrganizationId = org.Id };
                _db.NablAccreditations.Add(nabl);
                await _db.SaveChangesAsync(cancellationToken);
            }
            return await UploadNablCertificateAsync(nabl.Id, file, env, cancellationToken);
        }

        public async Task<string> UploadSignatureAsync(long signatoryId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, FileType.Signatory, null, signatoryId.ToString());
            var relativePath = uploaded.FilePath;

            var sign = await _db.AuthorizedSignatories.FirstOrDefaultAsync(x => x.Id == signatoryId, cancellationToken);
            if (sign != null)
            {
                sign.SignaturePath = relativePath;
                await _db.SaveChangesAsync(cancellationToken);
            }
            return relativePath;
        }

        public async Task<string> UploadSignatureAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default)
        {
            // store signature file and return url without binding to DB (frontend uploads signature before signatory exists)
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var uploaded = await _fileUploadService.UploadFileAsync(file, Dtos.FileType.Signatory, null, string.Empty);
            var relativePath = uploaded.FilePath;
            return relativePath;
        }

        // ----------------------------
        // DTO-based thin-controller helpers
        // ----------------------------
        public async Task<SettingsSaveAllDto> GetAllDtoAsync(long organizationId = 0, CancellationToken cancellationToken = default)
        {
            var (org, nabl, numbering, gst, year, signatories) = await GetAllAsync(organizationId, cancellationToken);

            var orgDto = new OrganizationDto
            {
                Id = org.Id,
                LabName = org.LabName,
                LabCode = org.LabCode,
                LabAddress = org.LabAddress,
                ContactEmail = org.ContactEmail,
                ContactPhone = org.ContactPhone,
                OrganizationLogo = org.OrganizationLogo
            };

            var nablDto = new NablDto
            {
                NablEnabled = nabl != null,
                NablTcNumber = nabl?.CertificateNumber,
                NablCertificate = nabl?.CertificatePath,
                NablLogo = null
            };

            var numberingDto = new NumberingDto();
            var tc = numbering.FirstOrDefault(x => x.ModuleName == "TC");
            if (tc != null)
            {
                numberingDto.TcBaseNumber = tc.Prefix;
                numberingDto.YearCode = DateTime.UtcNow.Year.ToString().Substring(2);
                numberingDto.RunningCounter = tc.CurrentNumber;
            }
            var report = numbering.FirstOrDefault(x => x.ModuleName == "REPORT");
            if (report != null)
            {
                numberingDto.ReportNumberPrefix = report.Prefix;
            }

            var gstDto = new GstDto
            {
                GstApplicable = gst != null,
                Gstin = gst?.GstNumber,
                PanNumber = null,
                StateCode = gst?.State,
                DefaultGstRate = gst?.DefaultGstRate ?? 18,
                PIGstApplicable = gst?.PIGstApplicable ?? true
            };

            var fyDto = new FinancialYearDto();
            if (year != null)
            {
                fyDto.StartDate = DateOnly.FromDateTime(year.StartDate);
                fyDto.EndDate = DateOnly.FromDateTime(year.EndDate);
            }

            var signDtos = signatories.Select(s => new SignatoryDto
            {
                Id = s.Id,
                SignatoryName = s.Name,
                Designation = s.Designation,
                SignatureImage = s.SignaturePath,
                Status = true,
                ApplicableFor = true
            }).ToArray();

            return new SettingsSaveAllDto
            {
                OrganizationInfo = orgDto,
                NablAccreditation = nablDto,
                Numbering = numberingDto,
                GstConfig = gstDto,
                FinancialYear = fyDto,
                Signatories = signDtos.ToList()
            };
        }

        public async Task<Organization> SaveOrganizationAsync(OrganizationDto organizationDto, CancellationToken cancellationToken = default)
        {
            var (existingOrg, _, _, _, _, _) = await GetAllAsync(0, cancellationToken);
            var org = new Organization
            {
                Id = existingOrg?.Id ?? 0,
                LabName = organizationDto.LabName,
                LabCode = organizationDto.LabCode,
                LabAddress = organizationDto.LabAddress,
                ContactEmail = organizationDto.ContactEmail,
                ContactPhone = organizationDto.ContactPhone,
                OrganizationLogo = organizationDto.OrganizationLogo
            };
            return await SaveOrganizationAsync(org, cancellationToken);
        }

        public async Task<NablAccreditation> SaveNablAsync(NablDto nablDto, CancellationToken cancellationToken = default)
        {
            var (org, existingNabl, _, _, _, _) = await GetAllAsync(0, cancellationToken);
            var model = existingNabl ?? new NablAccreditation { OrganizationId = org.Id };
            model.CertificateNumber = nablDto.NablTcNumber ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nablDto.NablCertificate)) model.CertificatePath = nablDto.NablCertificate;
            return await SaveNablAsync(model, cancellationToken);
        }

        public async Task<object> SaveNumberingAsync(NumberingDto numberingDto, CancellationToken cancellationToken = default)
        {
            var (org, _, existingNumbering, _, _, _) = await GetAllAsync(0, cancellationToken);
            var tc = existingNumbering.FirstOrDefault(x => x.ModuleName == "TC") ?? new NumberingConfig { ModuleName = "TC", OrganizationId = org.Id };
            tc.Prefix = numberingDto.TcBaseNumber ?? string.Empty;

            var report = existingNumbering.FirstOrDefault(x => x.ModuleName == "REPORT") ?? new NumberingConfig { ModuleName = "REPORT", OrganizationId = org.Id };
            report.Prefix = numberingDto.ReportNumberPrefix ?? string.Empty;

            var savedTc = await SaveNumberingAsync(tc, cancellationToken);
            var savedReport = await SaveNumberingAsync(report, cancellationToken);
            var result = new
            {
                TcConfig = savedTc,
                ReportConfig = savedReport
            };
            return result;
        }

        public async Task<GstConfig> SaveGstAsync(GstDto gstDto, CancellationToken cancellationToken = default)
        {
            var (org, _, _, existingGst, _, _) = await GetAllAsync(0, cancellationToken);
            var model = existingGst ?? new GstConfig { OrganizationId = org.Id };
            model.GstNumber = gstDto.Gstin ?? string.Empty;
            model.State = gstDto.StateCode ?? string.Empty;
            model.Address = null;
            model.DefaultGstRate = gstDto.DefaultGstRate;
            model.PIGstApplicable = gstDto.PIGstApplicable;
            return await SaveGstAsync(model, cancellationToken);
        }

        public async Task<FinancialYear> SaveFinancialYearAsync(FinancialYearDto yearDto, CancellationToken cancellationToken = default)
        {
            var (org, _, _, _, existingYear, _) = await GetAllAsync(0, cancellationToken);
            var model = new FinancialYear
            {
                Id = existingYear?.Id ?? 0,
                OrganizationId = org.Id,
                StartDate = yearDto.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = yearDto.EndDate.ToDateTime(TimeOnly.MinValue),
                Year = $"{yearDto.StartDate.Year}-{yearDto.EndDate.Year}",
                IsCurrent = true
            };
            return await SaveFinancialYearAsync(model, cancellationToken);
        }

        public async Task<AuthorizedSignatory[]> SaveSignatoriesAsync(SignatoryDto[] signatoryDtos, CancellationToken cancellationToken = default)
        {
            var (org, _, _, _, _, _) = await GetAllAsync(0, cancellationToken);
            var results = new List<AuthorizedSignatory>();
            foreach (var signDto in signatoryDtos)
            {
                var sign = new AuthorizedSignatory
                {
                    Id = signDto.Id ?? 0,
                    Name = signDto.SignatoryName,
                    Designation = signDto.Designation,
                    SignaturePath = signDto.SignatureImage,
                    OrganizationId = org.Id
                };
                results.Add(await SaveSignatoryAsync(sign, cancellationToken));
            }
            return results.ToArray();
        }

        public async Task SaveAllAsync(SettingsSaveAllDto payload, CancellationToken cancellationToken = default)
        {
            var dto = payload;
            var (existingOrg, existingNabl, existingNumbering, existingGst, existingFy, _) = await GetAllAsync(0, cancellationToken);

            var org = new Organization
            {
                Id = existingOrg?.Id ?? 0,
                LabName = dto.OrganizationInfo.LabName,
                LabCode = dto.OrganizationInfo.LabCode,
                LabAddress = dto.OrganizationInfo.LabAddress,
                ContactEmail = dto.OrganizationInfo.ContactEmail,
                ContactPhone = dto.OrganizationInfo.ContactPhone,
                OrganizationLogo = dto.OrganizationInfo.OrganizationLogo
            };

            NablAccreditation? nabl = null;
            if (dto.NablAccreditation.NablEnabled)
            {
                nabl = existingNabl ?? new NablAccreditation { OrganizationId = org.Id };
                nabl.CertificateNumber = dto.NablAccreditation.NablTcNumber ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(dto.NablAccreditation.NablCertificate)) nabl.CertificatePath = dto.NablAccreditation.NablCertificate;
            }

            var numberingList = new List<NumberingConfig>();
            var tc = existingNumbering.FirstOrDefault(x => x.ModuleName == "TC") ?? new NumberingConfig { ModuleName = "TC", OrganizationId = org.Id };
            tc.Prefix = dto.Numbering.TcBaseNumber ?? string.Empty;
            var report = existingNumbering.FirstOrDefault(x => x.ModuleName == "REPORT") ?? new NumberingConfig { ModuleName = "REPORT", OrganizationId = org.Id };
            report.Prefix = dto.Numbering.ReportNumberPrefix ?? string.Empty;
            numberingList.Add(tc);
            numberingList.Add(report);

            GstConfig? gst = null;
            if (dto.GstConfig.GstApplicable)
            {
                gst = existingGst ?? new GstConfig { OrganizationId = org.Id };
                gst.GstNumber = dto.GstConfig.Gstin ?? string.Empty;
                gst.State = dto.GstConfig.StateCode ?? string.Empty;
                gst.DefaultGstRate = dto.GstConfig.DefaultGstRate;
                gst.PIGstApplicable = dto.GstConfig.PIGstApplicable;
            }

            FinancialYear? fy = null;
            if (dto.FinancialYear.StartDate != default && dto.FinancialYear.EndDate != default)
            {
                fy = new FinancialYear
                {
                    Id = existingFy?.Id ?? 0,
                    OrganizationId = org.Id,
                    StartDate = dto.FinancialYear.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = dto.FinancialYear.EndDate.ToDateTime(TimeOnly.MinValue),
                    Year = $"{dto.FinancialYear.StartDate.Year}-{dto.FinancialYear.EndDate.Year}",
                    IsCurrent = true
                };
            }

            var signatories = dto.Signatories.Select(s => new AuthorizedSignatory
            {
                Id = s.Id ?? 0,
                Name = s.SignatoryName,
                Designation = s.Designation,
                SignaturePath = s.SignatureImage,
                OrganizationId = org.Id
            }).ToArray();

            await SaveAllAsync(org, nabl, numberingList.ToArray(), gst, fy, signatories, cancellationToken);
        }

        public async Task<string> UploadOrganizationLogoAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await UploadOrganizationLogoAsync(file, _env, cancellationToken);
        }

        public async Task<string> UploadNablCertificateAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await UploadNablCertificateAsync(file, _env, cancellationToken);
        }

        public async Task<string> UploadSignatureAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            return await UploadSignatureAsync(file, _env, cancellationToken);
        }

        public async Task DeleteSignatoryAsync(long signatoryId, CancellationToken cancellationToken = default)
        {
            var sign = await _db.AuthorizedSignatories.FirstOrDefaultAsync(x => x.Id == signatoryId, cancellationToken)
                ?? throw new InvalidOperationException("Signatory not found");
            _db.AuthorizedSignatories.Remove(sign);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
