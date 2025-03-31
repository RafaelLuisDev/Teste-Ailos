namespace Questao5.Infrastructure.Sqlite
{
    public interface IDatabaseBootstrap
    {
        void Setup();
        IEnumerable<T> ExecuteSelectQuery<T>(string sqlSelectQuery, object queryParams);
        IEnumerable<int> ExecuteQueries(List<KeyValuePair<string, object>> queries);
    }
}