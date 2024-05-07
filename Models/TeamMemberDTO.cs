namespace MirTest.Models;

public class TeamMemberDTO
{
    public List<Task> Created { get; set; }
    public List<Task> Assigned { get; set; }
}

public class Task
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public string ProjectName { get; set; }
    public string TaskType { get; set; }
}
