var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


// format is url route, and thenmethod that is called when that route is hit
app.MapGet("/", () => "Hello World!");

app.Run();

var problems = new List<Problem>
{
    new Problem("1", "Two Sum", "Find two numbers that add up to a specific target.", "Easy", "Array", "Array, Hash Table", "https://leetcode.com/problems/two-sum/"),
    new Problem("2", "Add Two Numbers", "Add two numbers represented by linked lists.", "Medium", "Linked List", "Linked List, Math", "https://leetcode.com/problems/add-two-numbers/"),
    new Problem("3", "Longest Substring Without Repeating Characters", "Find the length of the longest substring without repeating characters.", "Medium", "String", "Hash Table, String, Sliding Window", "https://leetcode.com/problems/longest-substring-without-repeating-characters/")
};

app.MapPost("/Problems", (Problem problem) =>
{
    // Here you would typically save the problem to a database or perform some action
    problems.Add(problem);
    return Results.Created($"/Problems/{problem.Id}", problem);
});

public record Problem(string Id, string Title, string Description, string Difficulty, string Category, string Tags, string Link);





// CRUD = Create, Read, Update, Delete