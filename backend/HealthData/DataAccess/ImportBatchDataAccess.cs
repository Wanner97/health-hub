using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ImportBatchDataAccess : IImportBatchDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public ImportBatchDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public ImportBatch CreateImportBatch(ImportBatch importBatch)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.ImportBatches.Add(importBatch);
                context.SaveChanges();

                return importBatch;
            }
        }
    }
}
