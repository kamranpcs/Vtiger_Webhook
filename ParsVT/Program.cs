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
using Microsoft.AspNetCore.Http.HttpResults;
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
            var ObjTask =
                JsonSerializer.SerializeToNode(
                    PhpSerializerNET.PhpSerialization.Deserialize(ObjSerialize["task"].ToString()));


            var latestserialize = JsonSerializer.Serialize(ObjTask["field_value_mapping"].ToString());
            var objField = JsonSerializer.SerializeToNode(ObjTask["field_value_mapping"]);
            ClientSide CLSide = new ClientSide
            {
                id = ObjTask["id"].ToString(),
                ModulName = ObjSerialize["module_name"].ToString(),
                Test = ObjSerialize["test"].ToString(),
                Summary = ObjSerialize["summary"].ToString(),
                FieldValueMapping =
                    JsonConvert.DeserializeObject<List<JsonField>>(ObjTask["field_value_mapping"].ToString()),
                WebHookUrl = ObjTask["webhook_url"].ToString()
                
            };
            ListClientSides.Add(CLSide);
        }
        return Results.Ok(ListClientSides);
    }
});
app.MapPost("/CreateWebhook",  (ClientSide _clientSide) =>
{
    string tes = "";
    int execution_condition = 3;
    int filtersavedinnewworkflowname = 6;
    int status = 1;
    string sqlcommand =
        $"insert into com_vtiger_workflows (module_name,summary,test,execution_condition,type,filtersavedinnew,status,workflowname) values (@module_name,@summary,@test,@execution_condition,@type,@filtersavedinnew,@status,@workflowname)";
    MySqlConnection connection = new MySqlConnection(ConnectionString);
    MySqlCommand command = new MySqlCommand(sqlcommand, connection);
    command.Parameters.AddWithValue("@module_name", _clientSide.ModulName);
    command.Parameters.AddWithValue("@summary", _clientSide.Summary);
    command.Parameters.AddWithValue("@test", _clientSide.Test);
    command.Parameters.AddWithValue("@execution_condition",execution_condition);
    command.Parameters.AddWithValue("@type", "basic");
    command.Parameters.AddWithValue("@filtersavedinnew", filtersavedinnewworkflowname);
    command.Parameters.AddWithValue("@status",status);
    command.Parameters.AddWithValue("@workflowname",_clientSide.Summary);
    connection.Open();
    command.ExecuteNonQuery();
    sqlcommand = "SELECT LAST_INSERT_ID()";
    var LatestIdInsert = connection.Query(sqlcommand).FirstOrDefault();
    string workflowid = "";
    foreach (var id in LatestIdInsert)
    {
        var key = id.Key;
        var value = id.Value;
        workflowid = value.ToString();
    }
   
    
    
    connection.Close();
    //////////////////////////////// End WorkFlow Insert/////////////////////////////////////
    // string FieldSQLCommand = $"SELECT * FROM vtiger_field where tablename like '%{_clientSide.ModulName}%'";
    // connection.Open();
    // var ModulFields = connection.Query(FieldSQLCommand).ToList();
    // connection.Close();
    // List<JsonField> FieldsForJson = new List<JsonField>();
    // foreach (var field in ModulFields)
    // {
    //     JsonField jsonField = new JsonField
    //     {
    //         Fieldname = field["fieldname"].ToString(),
    //         Value = field["fieldname"].ToString(),
    //         Valuetype = "fieldname"
    //     };
    //     FieldsForJson.Add(jsonField);
    // }
    // return FieldsForJson;
    List<JsonField> field = _clientSide.FieldValueMapping;
    TaskConfiguration _taskConfig = new TaskConfiguration
    {
        executeImmediately = true,
        workflowId = workflowid,
        summary = _clientSide.Summary,
        active = true,
        trigger = null,
        field_value_mapping = field,
        function_value_mapping = "",
        header_value_mapping = "",
        webhook_description = _clientSide.Summary,
        notify_method = "OPTIONS",
        webhook_url = _clientSide.WebHookUrl,
        content_type = "JSON",
        authentication_type = "NO",
        webhook_username = "",
        webhook_password = "",
        webhook_token = "",
        disable_ssl = "Yes",
        webhook_timeout = "600",
        webhook_separtor = "|",
        webhook_function = "",
        update_value_mapping = null,
        use_response = "1",
        soap_version = "Auto",
        id =Convert.ToInt32(_clientSide.id) 
    };
    var SerializedConfiguration = PhpSerializerNET.PhpSerialization.Serialize(_taskConfig);
    return SerializedConfiguration;
});
app.Run();