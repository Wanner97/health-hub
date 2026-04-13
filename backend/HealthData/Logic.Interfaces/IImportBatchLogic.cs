using Common.Dtos.DataReadDtos;
using Common.Models;

namespace Logic.Interfaces
{
    public interface IImportBatchLogic
    {
        List<ImportBatchReadDto> GetImportBatches(DateOnly? from, DateOnly? to);
        ImportBatch ImportActivity(Stream stream);
    }
}
