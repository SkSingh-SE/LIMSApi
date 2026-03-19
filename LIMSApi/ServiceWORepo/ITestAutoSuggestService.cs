using LIMSApi.Dtos;

namespace LIMSApi.ServiceWORepo
{
    public interface ITestAutoSuggestService
    {
        /// <summary>
        /// Given a specification grade, find all laboratory tests linked through spec lines.
        /// </summary>
        Task<List<SuggestedTestDto>> GetSuggestedTestsBySpecification(long specificationGradeId);

        /// <summary>
        /// Given a product specification, find all required tests per batch.
        /// </summary>
        Task<List<SuggestedTestDto>> GetSuggestedTestsByProductSpec(long productSpecificationId);

        /// <summary>
        /// Combined: given grade + product spec, return unified deduplicated test list.
        /// </summary>
        Task<TestAutoSuggestResult> GetUnifiedSuggestions(long? specificationGradeId, long? productSpecificationId);

        /// <summary>
        /// Smart auto-suggest: scores and ranks tests based on specification, lab scope, customer history,
        /// global frequency, and trending data.
        /// </summary>
        Task<SmartSuggestResult> GetSmartSuggestions(SmartSuggestRequest request);
    }
}
