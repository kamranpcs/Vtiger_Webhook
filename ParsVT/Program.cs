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
app.MapGet("/FullConfig", () =>
{
    using (var Connection = new MySqlConnection(ConnectionString))
    {
        string JoinSQlCMD = "SELECT * From com_vtiger_workflowtasks wft INNER JOIN  com_vtiger_workflows wf ON wft.workflow_id=wf.workflow_id where task like '%WebHookTask%'";
        
        var webHookConfigs = Connection.Query(JoinSQlCMD).ToList();
        
        // for (int i = 0; i < webHookConfigs.Count; i++)
        // {
        //     var ObjSerialize = JsonSerializer.Serialize(webHookConfigs[i]);
        //     Dictionary<string, string> ConfigDic = new Dictionary<string, string>();
        //     
        //     ConfigDic.Add(key:"ModulName",ObjSerialize["module_name"].ToString());
        //     ConfigDic.Add(key:"Test",ObjSerialize["test"].ToString());
        //     ConfigDic.Add(key:"ExecutionCondition",ObjSerialize["execution_condition"].ToString());
        //     ConfigDic.Add(key:"Summary",ObjSerialize["summary"].ToString());
        //     ConfigDic.Add(key:"ModulName",ObjSerialize["module_name"].ToString());
        //     ConfigDic.Add(key:"ModulName",ObjSerialize["module_name"].ToString());
        // }
        //

        return Results.Ok(webHookConfigs);
    }
});
app.Run();