using Microsoft.Data.SqlClient;
using MirTest.Models;
using Task = MirTest.Models.Task;

namespace MirTest.Repositories;

public class MemberRepository : IMemberRepository
{
    private IConfiguration _configuration;

    public MemberRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public TeamMemberDTO GetMemberTasks(int MemberId)
    {
        TeamMemberDTO teamMemberDto = null;
        using SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            @"SELECT Task.Name as TName, Task.Description as TDes, Task.Deadline as TDate,
                                TaskType.Name as TTName, Project.Name as PName
                                FROM Task
                                JOIN TaskType on Task.IdTaskType = TaskType.IdTaskType
                                JOIN Project on Task.IdProject = Project.IdProject
                                WHERE IdAssignedTo = @Id
                                ORDER BY Task.Deadline DEsc";
        command.Parameters.AddWithValue("@Id", MemberId);

        SqlDataReader reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return null;
        }
        while (reader.Read())
        {
            if (teamMemberDto == null)
            {
                teamMemberDto = new TeamMemberDTO()
                {
                    Assigned = new List<Task>()
                    {
                        new Task()
                        {
                            Name = (string)reader["TName"], Description = (string)reader["TDes"],
                            Deadline = (DateTime)reader["Tdate"], TaskType = (string)reader["TTName"],
                            ProjectName = (string)reader["PName"]
                        }
                    }, Created = new List<Task>()
                };
            }
            else
            {
                teamMemberDto.Assigned.Add(new Task()
                {
                    Name = (string)reader["TName"], Description = (string)reader["TDes"],
                    Deadline = (DateTime)reader["Tdate"], TaskType = (string)reader["TTName"],
                    ProjectName = (string)reader["PName"]
                });
            }
        }
        
        command.CommandText =
            @"SELECT Task.Name as TName, Task.Description as TDes, Task.Deadline as TDate,
                                TaskType.Name as TTName, Project.Name as PName
                                FROM Task
                                JOIN TaskType on Task.IdTaskType = TaskType.IdTaskType
                                JOIN Project on Task.IdProject = Project.IdProject
                                WHERE IdCreator = @Id
                                ORDER BY Task.Deadline DEsc";
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Id", MemberId);
        
        reader.Dispose();

        reader = command.ExecuteReader();
        while (reader.Read())
        {
            
                teamMemberDto.Created.Add(new Task()
                {
                    Name = (string)reader["TName"], Description = (string)reader["TDes"],
                    Deadline = (DateTime)reader["Tdate"], TaskType = (string)reader["TTName"],
                    ProjectName = (string)reader["PName"]
                });
            }
        return teamMemberDto;
    }

    public int AddTask(TaskInsertDto insertDto)
    {
        using SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        SqlTransaction transaction = connection.BeginTransaction();
        command.Transaction = transaction;

        try
        {
            command.CommandText = "SELECT 1 from Project where IdProject = @IdPro";
            command.Parameters.AddWithValue("@IdPro", insertDto.IdProject);
            if ((int)command.ExecuteScalar() != 1)
            {
                return -1;
            }

            command.CommandText = "SELECT 1 from TaskType where IDTaskType = @IdTT";
            command.Parameters.AddWithValue("@IdTT", insertDto.IdTaskType);
            if ((int)command.ExecuteScalar() != 1)
            {
                return -2;
            }

            command.CommandText = "SELECT 1 from TeamMember where IdTeamMember = @IdA";
            command.Parameters.AddWithValue("@IdA", insertDto.IdAssignedTo);
            if ((int)command.ExecuteScalar() != 1)
            {
                return -3;
            }

            command.CommandText = "SELECT 1 from TeamMember where IdTeamMember = @IdC";
            command.Parameters.AddWithValue("@IdC", insertDto.IdCreator);
            if ((int)command.ExecuteScalar() != 1)
            {
                return -4;
            }

            command.CommandText =
                @"INSERT INTO TASK (Name, Description, Deadline, IdProject, IdTaskType, IdAssignedTo, IdCreator )
                                    Values (@Name, @Des, @Dead, @IdPro, @IdTT, @IdAT, @IdC); Select Scope_Identity()";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Name", insertDto.Name);
            command.Parameters.AddWithValue("@Des", insertDto.Description);
            command.Parameters.AddWithValue("@Dead", insertDto.Deadline);
            command.Parameters.AddWithValue("@IdPro", insertDto.IdProject);
            command.Parameters.AddWithValue("@IdTT", insertDto.IdTaskType);
            command.Parameters.AddWithValue("@IdAT", insertDto.IdAssignedTo);
            command.Parameters.AddWithValue("@IdC", insertDto.IdCreator);
            var id = command.ExecuteScalar();

            return Convert.ToInt32(id);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            transaction.Rollback();
            return 0;
        }
    }
}