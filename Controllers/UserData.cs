using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace keithjasper.co.uk_restfulapi.Controllers
{
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private static List<User> users;

    private static readonly string dataFilePath = "users.json";

    public UsersController()
    {
        LoadDataFromFile();
    }

    private void LoadDataFromFile()
    {
        if (System.IO.File.Exists(dataFilePath))
        {
            string json = System.IO.File.ReadAllText(dataFilePath);
            users = JsonConvert.DeserializeObject<List<User>>(json);
        }
        else
        {
            users = new List<User>();
        }
    }

    private void SaveDataToFile()
    {
        string json = JsonConvert.SerializeObject(users);
        System.IO.File.WriteAllText(dataFilePath, json);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(users);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        if (!users.Remove(item: users.Find(u => u.Id == id)))
        {
            Console.WriteLine("User not found");
            return NotFound();
        }
        else
        {
            Console.WriteLine("User found and deleting");
            return Ok();
        }
    }

    [HttpPost]
    public IActionResult AddUser(User user)
    {
        users.Add(user);
        SaveDataToFile();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = users.Find(u => u.Id == id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
}
