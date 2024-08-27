namespace ParsVT;

public class TaskConfiguration
{
    public bool executeImmediately { get; set; }
    public string workflowId { get; set; }
    public string summary { get; set; }
    public bool active { get; set; }
    public Nullable<DateTime> trigger { get; set; }
    public List<JsonField> field_value_mapping { get; set; }
    public string function_value_mapping { get; set; }
    public string header_value_mapping { get; set; }
    public string webhook_description { get; set; }
    public string notify_method { get; set; }
    public string webhook_url { get; set; }
    public string content_type { get; set; }
    public string authentication_type { get; set; }
    public string webhook_username { get; set; }
    public string webhook_password { get; set; }
    public string webhook_token { get; set; }
    public string disable_ssl { get; set; }
    public string webhook_timeout { get; set; }
    public string webhook_separtor { get; set; }
    public string webhook_function { get; set; }
    public Array update_value_mapping { get; set; }
    public string use_response { get; set; }
    public string soap_version { get; set; }
    public int id { get; set; }
}