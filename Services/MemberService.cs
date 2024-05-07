using MirTest.Models;
using MirTest.Repositories;

namespace MirTest.Services;

public class MemberService : IMemberService
{
    private IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public TeamMemberDTO GetMemberTasks(int MemberId)
    {
        return _memberRepository.GetMemberTasks(MemberId);
    }

    public int AddTask(TaskInsertDto insertDto)
    {
        return _memberRepository.AddTask(insertDto);
    }
}