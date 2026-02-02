using LIMSApi.Dtos;
using LIMSApi.Models;

namespace LIMSApi.ServiceWORepo
{
    public interface ISettingsService
    {
        // organizationId is optional; pass 0 to use the default/first organization in system
        Task<(Organization, NablAccreditation?, NumberingConfig[], GstConfig?, FinancialYear?, AuthorizedSignatory[])> GetAllAsync(long organizationId = 0, CancellationToken cancellationToken = default);
        Task<Organization> SaveOrganizationAsync(Organization organization, CancellationToken cancellationToken = default);
        Task<NablAccreditation> SaveNablAsync(NablAccreditation nabl, CancellationToken cancellationToken = default);
        Task<NumberingConfig> SaveNumberingAsync(NumberingConfig numbering, CancellationToken cancellationToken = default);
        Task<GstConfig> SaveGstAsync(GstConfig gst, CancellationToken cancellationToken = default);
        Task<FinancialYear> SaveFinancialYearAsync(FinancialYear year, CancellationToken cancellationToken = default);
        Task<AuthorizedSignatory> SaveSignatoryAsync(AuthorizedSignatory signatory, CancellationToken cancellationToken = default);

        // DTO-based helpers (keep controllers thin)
        Task<SettingsSaveAllDto> GetAllDtoAsync(long organizationId = 0, CancellationToken cancellationToken = default);
        Task<Organization> SaveOrganizationAsync(OrganizationDto organizationDto, CancellationToken cancellationToken = default);
        Task<NablAccreditation> SaveNablAsync(NablDto nablDto, CancellationToken cancellationToken = default);
        Task<object> SaveNumberingAsync(NumberingDto numberingDto, CancellationToken cancellationToken = default);
        Task<GstConfig> SaveGstAsync(GstDto gstDto, CancellationToken cancellationToken = default);
        Task<FinancialYear> SaveFinancialYearAsync(FinancialYearDto yearDto, CancellationToken cancellationToken = default);
        Task<AuthorizedSignatory[]> SaveSignatoriesAsync(SignatoryDto[] signatories, CancellationToken cancellationToken = default);
        Task SaveAllAsync(SettingsSaveAllDto payload, CancellationToken cancellationToken = default);

        Task SaveAllAsync(
            Organization organization,
            NablAccreditation? nabl,
            NumberingConfig[] numbering,
            GstConfig? gst,
            FinancialYear? year,
            AuthorizedSignatory[] signatories,
            CancellationToken cancellationToken = default
        );
        Task<string> UploadOrganizationLogoAsync(long organizationId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default);
        Task<string> UploadNablCertificateAsync(long nablId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default);
        Task<string> UploadSignatureAsync(long signatoryId, IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default);

        // Convenience upload endpoints: frontend often uploads files without an entity Id.
        Task<string> UploadOrganizationLogoAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default);
        Task<string> UploadNablCertificateAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default);
        Task<string> UploadSignatureAsync(IFormFile file, IWebHostEnvironment env, CancellationToken cancellationToken = default);

        // Thin-controller overloads (service handles all sequencing/mapping)
        Task<string> UploadOrganizationLogoAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<string> UploadNablCertificateAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<string> UploadSignatureAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task DeleteSignatoryAsync(long signatoryId, CancellationToken cancellationToken = default);
    }

}
