using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseRewriter(new RewriteOptions().AddRedirect("^$", "Problems"));


var problems = new List<Problem>
{
    new Problem("1", "Two Sum", "Find two numbers that add up to a specific target.", "Easy", "Array", "Array, Hash Table", "https://leetcode.com/problems/two-sum/"),
    new Problem("2", "Add Two Numbers", "Add two numbers represented by linked lists.", "Medium", "Linked List", "Linked List, Math", "https://leetcode.com/problems/add-two-numbers/"),
    new Problem("3", "Longest Substring Without Repeating Characters", "Find the length of the longest substring without repeating characters.", "Medium", "String", "Hash Table, String, Sliding Window", "https://leetcode.com/problems/longest-substring-without-repeating-characters/")
};


// format is url route, and thenmethod that is called when that route is hit
app.MapGet("/test", () => "Hello World!");
app.MapGet("/Problems", () => problems);
app.MapGet("/Problems/{id}", Results<Ok<Problem>, NotFound> (string id) =>
{
    var problem = problems.FirstOrDefault(p => p.Id == id);
    return problem is not null ? TypedResults.Ok(problem) : TypedResults.NotFound();
});

app.MapPost("/Problems", (Problem problem) =>
{
    problems.Add(problem);
    return TypedResults.Created($"/Problems/{problem.Id}", problem);
});

app.MapDelete("/Problems/{id}", Results<NoContent, NotFound> (string id) =>
{
    var problem = problems.FirstOrDefault(p => p.Id == id);
    if (problem is null)
    {
        return TypedResults.NotFound();
    }
    
    problems.Remove(problem);
    return TypedResults.NoContent();
});

app.Run();


public record Problem(string Id, string Title, string Description, string Difficulty, string Category, string Tags, string Link);




// CRUD = Create, Read, Update, Delete


// Create → MapPost()
// Read → MapGet()
// Update → MapPut() or MapPatch()
// Delete → MapDelete()