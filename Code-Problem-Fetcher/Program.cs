using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseRewriter(new RewriteOptions().AddRedirect("^$", "Problems"));

// tmp exmaple object
var problems = new List<Problem> {
    new Problem(
        "1",
        "Two Sum",
        "Find two numbers that add up to a specific target.",
        "Easy",
        "Array",
        new[] { "Array", "Hash Table" },
        "https://leetcode.com/problems/two-sum/",
        new[]
        {
            new TestCase(
                new Dictionary<string, object>
                {
                    { "nums", new[] { 2, 7, 11, 15 } },
                    { "target", 9 }
                },
                new[] { 0, 1 }
            )
        },
        new Dictionary<string, object>
        {
            { "arrayLength", "2 <= nums.length <= 10^4" },
            { "uniqueSolution", "Only one valid answer exists" }
        }
    ),

    new Problem(
        "2",
        "Add Two Numbers",
        "Add two numbers represented by linked lists.",
        "Medium",
        "Linked List",
        new[] { "Linked List", "Math" },
        "https://leetcode.com/problems/add-two-numbers/",
        new[]
        {
            new TestCase(
                new Dictionary<string, object>
                {
                    { "l1", new[] { 2, 4, 3 } },
                    { "l2", new[] { 5, 6, 4 } }
                },
                new[] { 7, 0, 8 }
            )
        },
        new Dictionary<string, object>
        {
            { "listLength", "1 <= list.length <= 100" },
            { "nonNegative", "The numbers do not contain any leading zeros." }
        }
    )
};


//// Create functiuns for CRUD operations
app.MapPost("/Problems", (Problem problem) =>
{
    problems.Add(problem);
    return TypedResults.Created($"/Problems/{problem.Id}", problem);
})
.AddEndpointFilter(async (context, next) =>
{

    Console.WriteLine("Validating problem...");
    var problem = context.GetArgument<Problem>(0); // get the Problem argument
    var errors = new Dictionary<string, string[]>(); // list to collect errors

    // problems need and id, title, description, difficulty, catagory and at least one valid test case
    if (string.IsNullOrWhiteSpace(problem.Id))
        errors.Add(nameof(problem.Id), ["Id is required."]);
    if (string.IsNullOrWhiteSpace(problem.Title))
        errors.Add(nameof(problem.Title), ["Title is required."]);
    if (string.IsNullOrWhiteSpace(problem.Description))
        errors.Add(nameof(problem.Description), ["Description is required."]);
    if (string.IsNullOrWhiteSpace(problem.Difficulty))
        errors.Add(nameof(problem.Difficulty), ["Difficulty is required."]);
    if (string.IsNullOrWhiteSpace(problem.Category))
        errors.Add(nameof(problem.Category), ["Category is required."]);
    if (problem.TestCases == null || problem.TestCases.Length == 0)
        errors.Add("TestCases", ["At least one test case is required."]);
    if (problem.TestCases.Any(tc => tc.Input == null || !tc.Input.Any()))
        errors.Add("TestCaseInput", ["Test case input is required."]);
    if (problem.TestCases.Any(tc => tc.ExpectedOutput == null))
        errors.Add("TestCaseOutput", ["Test case expected output is required."]);
    if (problem.Constraints == null || !problem.Constraints.Any())
        errors.Add("Constraints", ["At least one constraint is required."]);


    if (errors.Count > 0)
    {
        Console.WriteLine("Problem is valid, adding to the list.");
        // if no errors, continue to the next middleware
        return Results.ValidationProblem(errors);
    }

    // if there are errors, return a validation problem
    return await next(context);
});


//// Read functions to get all problems or a specific problem by ID

// format is url route, and thenmethod that is called when that route is hit
app.MapGet("/test", () => "Hello World!");
app.MapGet("/Problems", () => problems);
app.MapGet("/Problems/{id}", Results<Ok<Problem>, NotFound> (string id) =>
{
    var problem = problems.FirstOrDefault(p => p.Id == id);
    return problem is not null ? TypedResults.Ok(problem) : TypedResults.NotFound();
});


//// Update functions to update a problem by ID (may not be needed)



//// Delete function to remove a problem by ID (may not be needed)
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






// CRUD = Create, Read, Update, Delete


// Create → MapPost()
// Read → MapGet()
// Update → MapPut() or MapPatch()
// Delete → MapDelete()