namespace Common.Import
{
    public class ImportUpsertData<T>
    {
        public List<T> InsertedItems { get; set; } = new();

        public List<T> UpdatedItems { get; set; } = new();

        public int UnchangedCount { get; set; }
    }
}