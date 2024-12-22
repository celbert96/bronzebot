using Npgsql;

namespace BronzeBot.Services;

public class PostgresDatabaseService(string connectionString) : IDatabaseService
{
    private const int CommandTimeout = 1000;

    public List<Dictionary<string, object>> PerformQuery(string query)
    {
        using var conn = new NpgsqlConnection();
        conn.ConnectionString = connectionString;
        
        var cmd = new NpgsqlCommand(query, conn);
        cmd.CommandTimeout = CommandTimeout;
        cmd.Connection.Open();
        var reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        
        var queryResults = new List<Dictionary<string, object>>();
        List<string> columns = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToList();
        
        while(reader.Read())
        {
            var row = new Dictionary<string, object>();

            for(var i = 0; i < columns.Count; i++) 
            {
                row.Add(columns[i], reader.GetValue(i));
            }

            queryResults.Add(row);
        }

        return queryResults;
    }

    public List<Dictionary<string, object>> PerformQuery(string query, Dictionary<string, object> bindVars)
    {
        using var conn = new NpgsqlConnection();
        conn.ConnectionString = connectionString;
        
        var cmd = new NpgsqlCommand(query, conn);
        cmd.CommandTimeout = CommandTimeout;
        cmd.Connection.Open();

        foreach(var item in bindVars)
        {
            cmd.Parameters.Add(new NpgsqlParameter(item.Key, item.Value));
        }

        var reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        var queryResults = new List<Dictionary<string, object>>();
        List<string> columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

        while(reader.Read())
        {
            var row = new Dictionary<string, object>();
            for(var i = 0; i < columns.Count; i++)
            {
                row.Add(columns[i], reader.GetValue(i));
            }

            queryResults.Add(row);
        }

        return queryResults;
    }

    public int PerformNonQuery(string sql, Dictionary<string, object> bindVars)
    {
        using var conn = new NpgsqlConnection();
        conn.ConnectionString = connectionString;
        
        var cmd = new NpgsqlCommand(sql, conn);
        cmd.CommandTimeout = CommandTimeout;
        cmd.Connection.Open();
        foreach(var item in bindVars)
        {
            cmd.Parameters.Add(new NpgsqlParameter(item.Key, item.Value));
        }

        return cmd.ExecuteNonQuery();
    }

    public int PerformNonQueries(List<KeyValuePair<string, Dictionary<string, object>>> commands)
    {
        using var conn = new NpgsqlConnection();
        var numUpdates = 0;
        conn.ConnectionString = connectionString;
        conn.Open();

        foreach(var (sql, value) in commands)
        {
            var cmd = new NpgsqlCommand(sql, conn);

            foreach(var bindVar in value)
            {
                cmd.Parameters.Add(new NpgsqlParameter(bindVar.Key, bindVar.Value));
            }

            numUpdates += cmd.ExecuteNonQuery();
        }

        return numUpdates;
    }
}