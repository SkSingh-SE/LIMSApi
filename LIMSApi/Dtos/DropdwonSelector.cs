namespace LIMSApi.Dtos
{
    public class DropdwonSelector
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // --- Hierarchical Tree Properties ---
        public int Level { get; set; } = 0;                     // 0 = Test Group, 1 = SubGroup, 2 = AnalysisType
        public bool Selectable { get; set; } = true;             // false for non-selectable headers/groups
        public string NodeType { get; set; } = "Item";          // "TestGroup", "SubGroup", "AnalysisType", "Header", "Item"
        public long? ParentId { get; set; }                     // FK to parent node

        // --- Backward Compatibility properties for existing UI ---
        public bool IsHeader { get; set; } = false;
        public bool IsChild { get; set; } = false;

        public Dictionary<string, object>? AdditionalValues { get; set; }
    }
}
