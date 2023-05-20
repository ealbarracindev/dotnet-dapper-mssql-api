using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Collections;

namespace WebApi.Helpers;
public class DapperWrap
{
    private DbSettings _dbSettings;

    public DapperWrap(IOptions<DbSettings> dbSettings)
    {
        _dbSettings = dbSettings.Value;
    }
    public async Task<IEnumerable<T>> GetRecords<T>(string sql, object parameters = null)
    {
        IEnumerable<T> records = default(IEnumerable<T>);
        var connectionString = $"Server={_dbSettings.Server}; Database={_dbSettings.Database}; User Id={_dbSettings.UserId}; Password={_dbSettings.Password}; TrustServerCertificate=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.StatisticsEnabled = true;
            await connection.OpenAsync();

            try
            {
                records = await connection.QueryAsync<T>(sql, parameters);
            }
            catch (Exception originalException)
            {
                throw AddAdditionalInfoToException(originalException, "Error: GetRecords: " + typeof(T).Name, sql, parameters);
            }

            var stats = connection.RetrieveStatistics();
            LogInfo("GetRecords: " + typeof(T).Name, stats, sql, parameters);
        }

        return records;
    }

    public async Task<T> GetRecord<T>(string sql, object parameters = null)
    {
        T record = default(T);
        var connectionString = $"Server={_dbSettings.Server}; Database={_dbSettings.Database}; User Id={_dbSettings.UserId}; Password={_dbSettings.Password}; TrustServerCertificate=True";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.StatisticsEnabled = true;
            await connection.OpenAsync();

            try
            {
                record = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
            catch (Exception originalException)
            {
                throw AddAdditionalInfoToException(originalException, "Error: GetRecord: " + typeof(T).Name, sql, parameters);
            }

            var stats = connection.RetrieveStatistics();
            LogInfo("GetRecord: " + typeof(T).Name, stats, sql, parameters);
        }

        return record;
    }

    private void LogInfo(string logPrefix, IDictionary stats, string sql, object parameters = null)
    {
        long elapsedMilliseconds = (long)stats["ConnectionTime"];

         Log.ForContext("SQL", sql)
            .ForContext("Parameters", parameters)
            .ForContext("ExecutionTime", stats["ExecutionTime"])
            .ForContext("NetworkServerTime", stats["NetworkServerTime"])
            .ForContext("BytesSent", stats["BytesSent"])
            .ForContext("BytesReceived", stats["BytesReceived"])
            .ForContext("SelectRows", stats["SelectRows"])
            .Information("{logPrefix} in {ElaspedTime:0.0000} ms", logPrefix, elapsedMilliseconds);
    }

    private Exception AddAdditionalInfoToException(Exception originalException, string message, string sql, object parameters = null)
    {
        var additionalInfoException = new Exception(message, originalException);
        additionalInfoException.Data.Add("SQL", sql);
        var props = parameters.GetType().GetProperties();
        foreach (var prop in props)
        {
            additionalInfoException.Data.Add(prop.Name, prop.GetValue(parameters));
        }

        return additionalInfoException;
    }
}
