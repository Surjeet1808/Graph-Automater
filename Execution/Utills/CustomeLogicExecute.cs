// using Microsoft.CodeAnalysis.CSharp.Scripting;
// using Microsoft.CodeAnalysis.Scripting;
// using operation.Model;
// using operation.Tasks;
// using operation.Common;

// namespace operation.CustomeLogic{
//   public class RuleEvaluator
// {
//     public class ScriptGlobals
//     {
//         public TaskTypes Task { get; set; }

//         public ScriptGlobals()
//         {
//             Task = new TaskTypes();
//         }
//     }

//     public async Task<object> EvaluateRuleAsync(string ruleCode, ScriptGlobals globals)
//     {
//         bool contains = ruleCode.Contains("customeCode");
//         if (contains)
//         {
//             return null!; // Use null-forgiving operator to suppress warnings
//         }

//         contains = ruleCode.Contains("EvaluateRuleAsync");
//         if (contains)
//         {
//             return null!; // Use null-forgiving operator to suppress warnings
//         }

//         var options = ScriptOptions.Default
//             .AddReferences(typeof(object).Assembly, typeof(TaskTypes).Assembly)
//             .AddImports("System", "System.Diagnostics", "operation.Tasks", "operation.Common");

//         var result = new object();
//         try
//         {
//             result = await CSharpScript.EvaluateAsync(ruleCode, options, globals);
//         }
//         catch (CompilationErrorException ex)
//         {
//             throw new NotSupportedException("Custom code compilation failed.");
//         }
//         return result;
//     }
// }
// }

