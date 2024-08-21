using System.IO.Pipes;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ParsVT;
using Dapper;
using Kaitai;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;


var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString("MySQLConnection");
var app = builder.Build();
app.MapGet("/", () =>
{
    using (var Connection = new MySqlConnection(ConnectionString))
    {
        string SQlCMD = "SELECT task FROM com_vtiger_workflowtasks where task like '%WebHookTask%'";
        List<string> webHookTaskConfigs = Connection.Query<string>(SQlCMD).ToList();
        // Dictionary<string, Dictionary<string, string>>
        //     resultDictionary = new Dictionary<string, Dictionary<string, string>>();
        List<Dictionary<string, string>> resultDictionary = new List<Dictionary<string, string>>();
        for (int i = 0; i < webHookTaskConfigs.Count; i++)
        {
            var DeserializeToJson = PhpSerializerNET.PhpSerialization.Deserialize(webHookTaskConfigs[i]);
            var ObjToJson = JsonSerializer.SerializeToNode(DeserializeToJson);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(key: "id", ObjToJson["id"].ToString());
            dic.Add(key: "summery", ObjToJson["summary"].ToString());
            dic.Add(key: "webhook_url", ObjToJson["webhook_url"].ToString());
            resultDictionary.Add(dic);
        }

        return Results.Ok(resultDictionary);
    }
});
app.Run();