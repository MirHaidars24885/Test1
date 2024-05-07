using MirTest.Models;

namespace MirTest.Repositories;

public interface IMemberRepository
{
    public TeamMemberDTO GetMemberTasks(int MemberId);
    public int AddTask(TaskInsertDto insertDto);
}