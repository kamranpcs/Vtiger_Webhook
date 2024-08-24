using System.Dynamic;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ParsVT;
using Dapper;
using Kaitai;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math.EC;
using JsonSerializer = System.Text.Json.JsonSerializer;


var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString("MySQLConnection");
var app = builder.Build();
app.MapGet("/", () =>
{
    using (var Connection = new MySqlConnection(ConnectionString))
    {
        string TaskSQlCMD = "SELECT task FROM com_vtiger_workflowtasks where task like '%WebHookTask%'";
        string WFSQLCMD = "SELECT task FROM com_vtiger_workflow ";
        List<string> webHookTaskConfigs = Connection.Query<string>(TaskSQlCMD).ToList();
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
app.MapGet("/FullConfig", () =>
{
    using (var Connection = new MySqlConnection(ConnectionString))
    {
        string JoinSQlCMD =
            "SELECT wft.task,wft.workflow_id,wft.task_id,wft.summary,wf.module_name,wf.execution_condition,wf.test From com_vtiger_workflowtasks as wft INNER JOIN  com_vtiger_workflows as wf ON wft.workflow_id=wf.workflow_id where task like '%WebHookTask%'";

        var webHookConfigs = Connection.Query(JoinSQlCMD).ToList();
        
        List<ClientSide> ListClientSides = new List<ClientSide>();
        List<string> teststring = new List<string>();
        List<object> objtest = new List<object>();

        for (int i = 0; i < webHookConfigs.Count; i++)
        {
            dynamic ObjSerialize = JsonSerializer.SerializeToNode(webHookConfigs[i]);
            var ObjTask = JsonSerializer.SerializeToNode(PhpSerializerNET.PhpSerialization.Deserialize(ObjSerialize["task"].ToString()));
            
            
            var latestserialize = JsonSerializer.Serialize(ObjTask["field_value_mapping"].ToString());
            var objField = JsonSerializer.SerializeToNode(ObjTask["field_value_mapping"]);
            ClientSide CLSide = new ClientSide
            {
                ModulName = ObjSerialize["module_name"].ToString(),
                Test = ObjSerialize["test"].ToString(),
                Summary = ObjSerialize["summary"].ToString(),
                FieldValueMapping = JsonConvert.DeserializeObject<List<JsonField>>(ObjTask["field_value_mapping"].ToString()),
                WebHookUrl = ObjTask["webhook_url"].ToString()
            };
            ListClientSides.Add(CLSide);
            
            
        }


        return Results.Ok(ListClientSides);
    }
});
app.Run();