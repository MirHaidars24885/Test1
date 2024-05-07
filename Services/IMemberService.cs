using MirTest.Models;

namespace MirTest.Services;

public interface IMemberService
{
    public TeamMemberDTO GetMemberTasks(int MemberId);
    public int AddTask(TaskInsertDto insertDto);
}