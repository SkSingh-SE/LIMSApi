namespace LIMSApi.Dtos
{
    public class ExecutionLayoutListRequest : PageFilter
    {
        public string? Status { get; set; }
    }

    public class ExecutionLayoutItemDto
    {
        public long ID { get; set; }
        public long ExecutionLayoutSectionID { get; set; }
        public string ReferenceType { get; set; } = "None";
        public long? ReferenceID { get; set; }
        public string? DisplayLabel { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEditable { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public string? ReferenceName { get; set; }
        public string? ReferenceCode { get; set; }
        public bool ReferenceIsActive { get; set; }
    }

    public class ExecutionLayoutSectionDto
    {
        public long ID { get; set; }
        public long ExecutionLayoutID { get; set; }
        public string SectionCode { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string SectionType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public bool IsCollapsible { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public string? PresentationStyle { get; set; }
        public string? SectionTypeDisplayName { get; set; }
        public List<ExecutionLayoutItemDto> Items { get; set; } = new();
    }

    public class ExecutionLayoutDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RendererType { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string? CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public List<ExecutionLayoutSectionDto> Sections { get; set; } = new();
    }

    public class ExecutionLayoutListItemDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? RendererType { get; set; }
        public int SectionsCount { get; set; }
        public bool IsActive { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
    }

    public class ExecutionLayoutCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RendererType { get; set; }
        public int DisplayOrder { get; set; }
        public List<ExecutionLayoutSectionDto> Sections { get; set; } = new();
    }

    public class ExecutionLayoutUpdateDto
    {
        public long ID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RendererType { get; set; }
        public int DisplayOrder { get; set; }
        public List<ExecutionLayoutSectionDto> Sections { get; set; } = new();
    }

    public class ExecutionLayoutDropdownDto : DropdwonSelector { }

    public class SectionTypeMetaDto
    {
        public string SectionType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DefaultPresentationStyle { get; set; } = string.Empty;
        public List<string> AllowedReferenceTypes { get; set; } = new();
    }

    public class PresentationStyleMetaDto
    {
        public string Style { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ExecutionLayoutMetadataDto
    {
        public List<SectionTypeMetaDto> SectionTypes { get; set; } = new();
        public List<PresentationStyleMetaDto> PresentationStyles { get; set; } = new();
        public List<string> AllowedRendererTypes { get; set; } = new();
    }
}
