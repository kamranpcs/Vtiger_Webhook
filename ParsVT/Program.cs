using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);
MySqlConnection connection = new MySqlConnection(builder.Configuration.GetConnectionString("MySQLConnection"));
MySqlCommand command = new MySqlCommand();
command.Connection = connection;
var app = builder.Build();

app.MapGet("/", () =>
{
  
});

app.Run();