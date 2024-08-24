using System.Text.Json.Nodes;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;

namespace ParsVT;

public class ClientSide
{
    public string ModulName { get; set; }
    public string Test { get; set; }
    public string ExecutionCondition { get; set; }
    public string Summary { get; set; }
    public string FieldValueMapping { get; set; }
    public string HaederValueMapping { get; set; }
    public string WebHookUrl { get; set; }
}