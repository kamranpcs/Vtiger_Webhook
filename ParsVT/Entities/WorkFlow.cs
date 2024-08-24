namespace ParsVT;

public class WorkFlow
{
    public int WorkFlowId { get; set; }
    public string ModulNAme { get; set; }
    public string Summary { get; set; }
    public string Test { get; set; }
    public int ExecutionCondition { get; set; }
    public int DefaultWorkFlow { get; set; }
    public string Type { get; set; }
    public int FilterSavedInNew { get; set; }
    public int SchTypeId { get; set; }
    public string SchDayOfMonth { get; set; }
    public string SchDayOfWeek { get; set; }
    public string SchAnnualDates { get; set; }
    public string SchTime { get; set; }
    public string NextTriggerTime { get; set; }
    public int Status { get; set; }
    public string WorkFlowName { get; set; }
    
    
}