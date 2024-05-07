using Microsoft.AspNetCore.Mvc;
using MirTest.Models;
using MirTest.Services;

namespace MirTest.Controllers;
[ApiController]
[Route("api[controller]")]
public class MemberController : ControllerBase
{
    private IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public IActionResult GetMember(int id)
    {
        var check = _memberService.GetMemberTasks(id);
        if (check == null)
            return BadRequest();

        return Ok(check);
    }

    [HttpPost]
    public IActionResult insertTask(TaskInsertDto dto)
    {
        var check = _memberService.AddTask(dto);
        
        switch (check)
        {
            case -1 : return NotFound("Project not found");
            case -2 : return NotFound("TaskType not found");
            case -3 : return NotFound("Member not found");
            case -4 : return NotFound("Creator not found");
            case 0 : return BadRequest("An error occured");
        }

        return Ok(check);
    }
    
}