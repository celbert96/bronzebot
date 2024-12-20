namespace BronzeBot.Services;

public interface IDatabaseService
{
    /*
     * Perform a single query with no bind variables
     *
     * Results of the query are returned as a List of Dictionaries, representing database rows
     */
    public List<Dictionary<string, object>> PerformQuery(string query);
    
    /*
     * Perform a single query with bind variables
     *
     * Results of the query are returned as a List of Dictionaries, representing database rows
     */
    public List<Dictionary<string, object>> PerformQuery(string query, Dictionary<string, object> bindVars);
    
    /*
     * Perform a single Insert, Update, or Delete SQL command on the database
     *
     * Returns the number of rows affected by the SQL command
     */
    public int PerformNonQuery(string sql, Dictionary<string, object> bindVars);
    
    /*
     * Perform a series of Insert, Update, or Delete SQL command on the database
     *
     * Returns the number of rows affected by the SQL commands
     */
    public int PerformNonQueries(List<KeyValuePair<string, Dictionary<string, object>>> commands);
}