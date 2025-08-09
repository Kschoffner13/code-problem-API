using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Code_Problem_Fetcher.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
builder.Services.AddDbContext<ProblemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddScoped<ProblemService, DatabaseProblemService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProblemDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "Problems"));



//// Create functiuns for CRUD operations
app.MapPost("/Problems", async (Problem problem, ProblemService service) =>
{
    await service.addProblem(problem);
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
    if (problem.Tags == null || problem.Tags.Length == 0)
        errors.Add(nameof(problem.Tags), ["At least one tag is required."]);
    if (problem.Input == null || !problem.Input.Any())
        errors.Add("Input", ["Input is required."]);
    if (problem.ExpectedOutput == null)
        errors.Add("ExpectedOutput", ["Expected output is required."]);
    if (problem.Constraints == null || !problem.Constraints.Any())
            errors.Add("Constraints", ["At least one constraint is required."]);


    if (errors.Count > 0) 
    {
        Console.WriteLine("Problem validation failed, returning errors.");
        // if there are errors, return a validation problem
        return await Task.FromResult(Results.ValidationProblem(errors));
    }

    Console.WriteLine("Problem is valid, adding to the list.");
    // if no errors, continue to the next middleware
    return await next(context);
});


//// Read functions to get all problems or a specific problem by ID

// format is url route, and thenmethod that is called when that route is hit
app.MapGet("/test", () => "Hello World!");
app.MapGet("/Problems", async (ProblemService service) => await service.getAllProblems());
// app.MapGet("/Problems/{id}", async (string id, ProblemService service) =>
// {
//     var problem = await service.getProblemById(id);
//     return problem is not null ? TypedResults.Ok(problem) : TypedResults.NotFound();
// });


//// Update functions to update a problem by ID (may not be needed)



//// Delete function to remove a problem by ID (may not be needed)
// app.MapDelete("/Problems/{id}", async (string id, [FromServices] ProblemService service) =>
// {
//     var problem = await service.getProblemById(id);
//     if (problem is null)
//     {
//         return TypedResults.NotFound();
//     }

//     await service.deleteProblem(id);
//     return TypedResults.NoContent();
// })
// .AddEndpointFilter(async (context, next) =>
// {
//     var id = context.GetArgument<string>(0); // get the ID argument

//     var errors = new Dictionary<string, string[]>(); // list to collect errors

//     if (string.IsNullOrWhiteSpace(id))
//     {
//         errors.Add("Id", ["Id is required."]); // if id is null or empty, add an error
//         Console.WriteLine("Problem is valid, deleting from the list.");

//     }
//     if (errors.Count > 0)
//     {
//         Console.WriteLine("Problem is valid, deleting from the list.");
//         // if there are errors, return a validation problem wrapped in a Task
//         return await Task.FromResult(Results.ValidationProblem(errors));
//     }
    
//     // if the problem exists, continue to the next middleware
//     return await next(context);
// });

app.Run();







// CRUD = Create, Read, Update, Delete


// Create → MapPost()
// Read → MapGet()
// Update → MapPut() or MapPatch()
// Delete → MapDelete()