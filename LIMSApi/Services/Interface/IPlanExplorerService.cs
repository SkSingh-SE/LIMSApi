using System.Threading.Tasks;
using LIMSApi.Dtos;

namespace LIMSApi.Services.Interface
{
    public interface IPlanExplorerService
    {
        Task<ProductMasterExplorerDto?> GetProductMasterExplorerAsync(long productMasterId);
        Task<MetalExplorerDto?> GetMetalClassificationExplorerAsync(long metalClassificationId);
        Task<LabTestExplorerDto?> GetLabTestExplorerAsync(long labTestId);
    }
}
