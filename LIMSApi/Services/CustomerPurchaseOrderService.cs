using LIMSApi.Data;
using LIMSApi.Dtos;
using LIMSApi.Models;
using LIMSApi.Repositories.Interface;
using LIMSApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Services
{
    public class CustomerPurchaseOrderService : ICustomerPurchaseOrderService
    {
        private readonly ICustomerPurchaseOrderRepository _repo;
        private readonly LIMSContext _context;

        public CustomerPurchaseOrderService(ICustomerPurchaseOrderRepository repo, LIMSContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<PagedResponse<object>> GetAll(PageFilter filter)
        {
            return await _repo.GetAll(filter);
        }

        public async Task<CustomerPurchaseOrderDto?> GetById(long id)
        {
            var po = await _repo.GetById(id);
            if (po == null) return null;

            return new CustomerPurchaseOrderDto
            {
                Id = po.ID,
                CustomerId = po.CustomerId,
                CustomerName = po.Customer?.Name ?? "",
                PONumber = po.PONumber,
                PODate = po.PODate,
                ValidUntil = po.ValidUntil,
                POAmount = po.POAmount,
                UtilizedAmount = po.UtilizedAmount,
                RemainingAmount = po.RemainingAmount,
                Status = po.Status,
                Terms = po.Terms,
                FilePath = po.FilePath,
                FileName = po.FileName
            };
        }

        public async Task<CustomerPurchaseOrderDto> Create(CreateCustomerPurchaseOrderDto dto)
        {
            if (await _repo.ExistsByPONumber(dto.PONumber, dto.CustomerId))
                throw new InvalidOperationException($"PO Number '{dto.PONumber}' already exists for this customer.");

            var model = new CustomerPurchaseOrder
            {
                CustomerId = dto.CustomerId,
                PONumber = dto.PONumber,
                PODate = dto.PODate,
                ValidUntil = dto.ValidUntil,
                POAmount = dto.POAmount,
                UtilizedAmount = 0,
                RemainingAmount = dto.POAmount,
                Status = "Active",
                Terms = dto.Terms,
                FilePath = dto.FilePath,
                FileName = dto.FileName,
                UploadReferenceID = dto.UploadReferenceID
            };

            var result = await _repo.Add(model);

            return new CustomerPurchaseOrderDto
            {
                Id = result.ID,
                CustomerId = result.CustomerId,
                PONumber = result.PONumber,
                PODate = result.PODate,
                ValidUntil = result.ValidUntil,
                POAmount = result.POAmount,
                UtilizedAmount = result.UtilizedAmount,
                RemainingAmount = result.RemainingAmount,
                Status = result.Status
            };
        }

        public async Task<CustomerPurchaseOrderDto> Update(long id, CreateCustomerPurchaseOrderDto dto)
        {
            var existing = await _repo.GetById(id);
            if (existing == null)
                throw new InvalidOperationException("Purchase Order not found.");

            if (await _repo.ExistsByPONumberAndNotId(dto.PONumber, dto.CustomerId, id))
                throw new InvalidOperationException($"PO Number '{dto.PONumber}' already exists for this customer.");

            existing.PONumber = dto.PONumber;
            existing.PODate = dto.PODate;
            existing.ValidUntil = dto.ValidUntil;
            existing.POAmount = dto.POAmount;
            existing.RemainingAmount = dto.POAmount - existing.UtilizedAmount;
            existing.Terms = dto.Terms;
            existing.FilePath = dto.FilePath;
            existing.FileName = dto.FileName;
            existing.UploadReferenceID = dto.UploadReferenceID;

            // Update status based on remaining amount
            if (existing.RemainingAmount <= 0)
                existing.Status = "Exhausted";
            else if (existing.ValidUntil.HasValue && existing.ValidUntil.Value < DateTime.UtcNow)
                existing.Status = "Expired";
            else
                existing.Status = "Active";

            var result = await _repo.Update(existing);

            return new CustomerPurchaseOrderDto
            {
                Id = result.ID,
                CustomerId = result.CustomerId,
                PONumber = result.PONumber,
                PODate = result.PODate,
                ValidUntil = result.ValidUntil,
                POAmount = result.POAmount,
                UtilizedAmount = result.UtilizedAmount,
                RemainingAmount = result.RemainingAmount,
                Status = result.Status
            };
        }

        public async Task<bool> Delete(long id)
        {
            await _repo.Delete(id);
            return true;
        }

        public async Task<List<DropdwonSelector>> GetDropdown(long customerId, string? searchTerm, int pageNo, int pageSize)
        {
            return await _repo.GetDropdown(customerId, searchTerm, pageNo, pageSize);
        }

        public async Task<POUtilizationDto?> GetUtilization(long id)
        {
            var po = await _repo.GetById(id);
            if (po == null) return null;

            var invoices = await _context.TaxInvoices
                .Where(i => i.PurchaseOrderId == id)
                .Select(i => new POInvoiceLineDto
                {
                    InvoiceId = i.ID,
                    InvoiceNo = i.InvoiceNo,
                    InvoiceDate = i.InvoiceDate,
                    Amount = i.GrandTotal
                })
                .ToListAsync();

            return new POUtilizationDto
            {
                POId = po.ID,
                PONumber = po.PONumber,
                POAmount = po.POAmount,
                UtilizedAmount = po.UtilizedAmount,
                RemainingAmount = po.RemainingAmount,
                Invoices = invoices
            };
        }
    }
}
