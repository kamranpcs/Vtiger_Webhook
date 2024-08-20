using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ParsVT;
using Dapper;

var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString("MySQLConnection");
var app = builder.Build();
app.MapGet("/", () =>
{
    using (var Connection = new MySqlConnection(ConnectionString))
    {
        string SQlCMD = "SELECT * FROM com_vtiger_workflowtasks where task like '%WebHookTask%'";
        List<WebHookTaskConfig> webHookTaskConfigs = Connection.Query<WebHookTaskConfig>(SQlCMD).ToList();
        return Results.Ok(webHookTaskConfigs);
    }
});
app.Run();