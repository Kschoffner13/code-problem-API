




// defines the structure of a problem

using System.ComponentModel.DataAnnotations;

public record Problem(
    string Id,
    string Title,  
    string Description,
    string Difficulty,
    string Category,
    string[] Tags,
    string Link,
    TestCase[] TestCases,
    Dictionary<string, object> Constraints
);

public record TestCase(
    Dictionary<string, object> Input,
    object ExpectedOutput
);



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