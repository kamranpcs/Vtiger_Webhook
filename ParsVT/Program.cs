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
        string SQlCMD = "SELECT task FROM com_vtiger_workflowtasks where task like '%WebHookTask%'";
        List<string> webHookTaskConfigs = Connection.Query<string>(SQlCMD).ToList();
        List<object> ResultJson = new List<object>();
        for (int i = 0; i < webHookTaskConfigs.Count; i++)
        {
            ResultJson.Add(PhpSerializerNET.PhpSerialization.Deserialize(webHookTaskConfigs[i]));
        }

        return Results.Ok(ResultJson);
    }
});
app.Run();