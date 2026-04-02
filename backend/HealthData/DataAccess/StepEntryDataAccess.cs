using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class StepEntryDataAccess : IStepEntryDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public StepEntryDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public StepEntry CreateStepEntry(StepEntry stepEntry)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.StepEntries.Add(stepEntry);
                context.SaveChanges();

                return stepEntry;
            }
        }
    }
}
