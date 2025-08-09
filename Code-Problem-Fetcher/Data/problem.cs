namespace Code_Problem_Fetcher.Data;

// defines the structure of a problem
public class Problem
{
    // Parameterless constructor for Entity Framework
    // public Problem() { }
    
    // // Constructor for creating new problems
    // public Problem(string id, string title, string description, string difficulty, 
    //                string category, string[] tags, string link, 
    //                IEnumerable<TestCase> testCases, Dictionary<string, object> constraints)
    // { 
    //     Id = id;
    //     Title = title;
    //     Description = description;
    //     Difficulty = difficulty;
    //     Category = category;
    //     Tags = tags;
    //     Link = link;
    //     TestCases = testCases.ToList();
    //     Constraints = constraints;
    // }
    
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Link { get; set; } = string.Empty;

    public Dictionary<string, object> Input { get; set; } = new();
    public object ExpectedOutput { get; set; } = new();
    
    public Dictionary<string, object> Constraints { get; set; } = new();
}




// exmpale:

// {
//   "id": "1",
//   "title": "Two Sum",
//   "testCases": [
//     {
//       "input": {
//         "nums": [2, 7, 11, 15],
//         "target": 9
//       },
//       "expectedOutput": [0, 1],
//     }
//   ],
//   "constraints": {
//     "arrayLength": "2 <= nums.length <= 10^4",
//     "uniqueSolution": "Only one valid answer exists"
//   }
// }