using Common.Dtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using FluentValidation;

namespace Logic
{
    public class StepsReadLogic : IStepsReadLogic
    {
        private readonly IImportBatchDataAccess _importBatchDataAccess;

        public StepsReadLogic(IImportBatchDataAccess importBatchDataAccess)
        {
            _importBatchDataAccess = importBatchDataAccess;
        }

        public LatestStepsImportDto GetLatestSteps()
        {
            var importBatch = _importBatchDataAccess.GetLatestImportBatch();

            if (importBatch == null)
            {
                throw new ValidationException("No Steps-Import could be found.");
            }

            return new LatestStepsImportDto
            {
                ImportBatchId = importBatch.Id,
                Source = importBatch.Source,
                ExportVersion = importBatch.ExportVersion,
                ExportedAt = importBatch.ExportedAt,
                ImportedAt = importBatch.ImportedAt,
                RecordCount = importBatch.RecordCount,
                StepEntries = importBatch.StepEntries
                    .OrderBy(x => x.Date)
                    .Select(x => new StepEntryReadDto
                    {
                        Date = x.Date,
                        Count = x.Count,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    })
                    .ToList()
            };
        }
    }
}