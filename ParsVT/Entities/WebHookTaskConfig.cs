namespace ParsVT;

public class WebHookTaskConfig
{
    public int TaskId { get; set; }
    public int WorkFlowId { get; set; }
    public string Summary { get; set; }
    public TaskConfiguration Task { get; set; }
    
}