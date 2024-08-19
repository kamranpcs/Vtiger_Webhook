using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ParsVT;

var builder = WebApplication.CreateBuilder(args);
MySqlConnection connection = new MySqlConnection(builder.Configuration.GetConnectionString("MySQLConnection"));
MySqlCommand command = new MySqlCommand();
command.Connection = connection;
var app = builder.Build();

app.MapGet("/", () =>
{
    command.CommandText = "SELECT * FROM com_vtiger_workflowtasks";
    connection.Open();
    MySqlDataReader reader = command.ExecuteReader();
    List<WebHookTaskConfig> configs = new List<WebHookTaskConfig>();
    while (reader.Read())
    {
        var TableData = new WebHookTaskConfig
        {
            TaskId = Convert.ToInt32(reader["task_id"]),
            WorkFlowId = Convert.ToInt32(reader["workflow_id"]),
            Summary = reader["summary"].ToString(),
            Task = reader["task"].ToString()
        };
        configs.Add(TableData);
    }

    connection.Close();
    return configs;
});
app.MapGet("/task/{task_id}", ([FromRoute] int task_id) =>
    {
        command.CommandText = "SELECT * FROM com_vtiger_workflowtasks where task_id=@task_id";
        command.Parameters.AddWithValue("task_id", task_id);
        connection.Open();
        MySqlDataReader reader = command.ExecuteReader();
        if (reader.Read() == true)
        {
            var Result = PhpSerializerNET.PhpSerialization.Deserialize(reader["task"].ToString());
            connection.Close();
            return Result;
        }

        connection.Close();

        return null;
    }
);
app.Run();