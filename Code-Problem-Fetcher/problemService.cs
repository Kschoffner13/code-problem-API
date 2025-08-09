

// reminder: interfaces are used to outline a contract for classes to implement.
using Code_Problem_Fetcher.Data;
using Microsoft.EntityFrameworkCore;

interface ProblemService
{
    Task<Problem?> getProblemById(string id);
    Task<IEnumerable<Problem>> getAllProblems();
    Task<Problem?> addProblem(Problem problem);
    Task<Problem?> updateProblem(string id, Problem updatedProblem);
    Task<bool> deleteProblem(string id);
}

class InMemProblemService : ProblemService
{
    private List<Problem> problems = new();

    public Task<Problem?> getProblemById(string id)
    {
        var problem = problems.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(problem);
    }

    public Task<IEnumerable<Problem>> getAllProblems()
    {
        return Task.FromResult<IEnumerable<Problem>>(problems);
    }

    public Task<Problem?> addProblem(Problem problem)
    {
        if (problems.Any(p => p.Id == problem.Id))
        {
            return Task.FromResult<Problem?>(null);
        }
        
        problems.Add(problem);
        return Task.FromResult<Problem?>(problem);
    }

    public Task<Problem?> updateProblem(string id, Problem updatedProblem)
    {
        var existingProblem = problems.FirstOrDefault(p => p.Id == id);
        if (existingProblem is null)
        {
            return Task.FromResult<Problem?>(null);
        }

        problems.Remove(existingProblem);
        problems.Add(updatedProblem);
        return Task.FromResult<Problem?>(updatedProblem);
    }

    public Task<bool> deleteProblem(string id)
    {
        var problem = problems.FirstOrDefault(p => p.Id == id);
        if (problem is not null)
        {
            problems.Remove(problem);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

class DatabaseProblemService : ProblemService
{
    private readonly ProblemDbContext _context;

    public DatabaseProblemService(ProblemDbContext context)
    {
        _context = context;
    }

    public async Task<Problem?> getProblemById(string id)
    {
        // No longer need Include since TestCases are stored as JSON within Problem
        return await _context.Problems.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Problem>> getAllProblems()
    {
        // No longer need Include since TestCases are stored as JSON within Problem
        return await _context.Problems.ToListAsync();
    }

    public async Task<Problem?> addProblem(Problem problem)
    {
        if (await _context.Problems.AnyAsync(p => p.Id == problem.Id))
        {
            return null;
        }

        // TestCases are automatically serialized to JSON by EF - no need to set ProblemId
        _context.Problems.Add(problem);
        await _context.SaveChangesAsync();
        return problem;
    }

    public async Task<Problem?> updateProblem(string id, Problem updatedProblem)
    {
        var existingProblem = await _context.Problems.FirstOrDefaultAsync(p => p.Id == id);
            
        if (existingProblem is null)
        {
            return null;
        }

        // Update all properties including TestCases (which will be serialized to JSON)
        existingProblem.Title = updatedProblem.Title;
        existingProblem.Description = updatedProblem.Description;
        existingProblem.Difficulty = updatedProblem.Difficulty;
        existingProblem.Category = updatedProblem.Category;
        existingProblem.Tags = updatedProblem.Tags;
        existingProblem.Link = updatedProblem.Link;
        existingProblem.Input = updatedProblem.Input;
        existingProblem.ExpectedOutput = updatedProblem.ExpectedOutput;
        existingProblem.Constraints = updatedProblem.Constraints;

        await _context.SaveChangesAsync();
        return existingProblem;
    }

    public async Task<bool> deleteProblem(string id)
    {
        var problem = await _context.Problems.FindAsync(id);
        if (problem is null)
        {
            return false;
        }

        _context.Problems.Remove(problem);
        await _context.SaveChangesAsync();
        return true;
    }
}