// Copyright (c) Microsoft. All rights reserved.

using DotnetReferenceSkill;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Python.Runtime;

const string RANDOM_ACTIVITY_PROMPT = "Find me an activity to do, return only a single activity. I like to {{$INPUT}}. Be creative!";
const string DEGREES_OF_SEPARATION_PROMPT = "How many degrees of separation are there between {{$ITEM1}} and {{$ITEM2}}? Bonus points for Kevin Bacon reference.";

//configure your Azure OpenAI backend
var key = "5b742c40-bc2b-4a4f-902f-ee9f644d8844";
var endpoint = "https://piloteers-temporary-openai-westus.openai.azure.com/";
var model = "dv3";


var sk = Kernel.Builder.Configure(c => c.AddAzureOpenAICompletionBackend(model, model, endpoint, key)).Build();

#region Semantic Function Loading
sk.CreateSemanticFunction(RANDOM_ACTIVITY_PROMPT,
            skillName: "RandomActivityLLMSkill",
            functionName: "GetRandomActivity",
            description: "Finds a random activity based on the interest input",
            maxTokens: 256,
            temperature: 0.1,
            topP: 0.5);

sk.CreateSemanticFunction(DEGREES_OF_SEPARATION_PROMPT,
            skillName: "DegreesOfSeparation",
            functionName: "FindDegrees",
            description: "Returns the degrees of separation between two concepts.",
            maxTokens: 256,
            temperature: 0.2,
            topP: 0.5);
#endregion

//TODO: load your python skill here
//currently loading a skill that will pull a random activity from an API.
//We will compare the activity to the one from the LLM and return true if they match, false if not
var randomActivitySkill = new RandomActivitySkill();
sk.ImportSkill(randomActivitySkill, nameof(RandomActivitySkill));

var thingiliketodo = "read";
var llmResult = string.Empty;
var apiResult = string.Empty;

//ask the LLM for a random activity
var llmRandomActivityResult = await sk.RunAsync(thingiliketodo, sk.Skills.GetSemanticFunction("RandomActivityLLMSkill", "GetRandomActivity"));
llmResult = llmRandomActivityResult.Result;
Console.WriteLine("LLM suggested: " + llmResult);
PythonEngine.Initialize();
using (Py.GIL())
{
    dynamic requests = Py.Import("requests");
    dynamic json = Py.Import("json");
    var result = requests.get("https://www.boredapi.com/api/activity");
    Console.WriteLine(result.text);
    // Or
    String getActivity = "result = requests.get('https://www.boredapi.com/api/activity')";
    using (PyModule scope = Py.CreateScope())
    {
        String parseActivity = @"import json;
import requests;
result = requests.get('https://www.boredapi.com/api/activity').text;
activityObject = json.loads(result);
print('Activity in python: ' + activityObject['activity'])";
        scope.Exec(parseActivity);
        var activityObject = scope.Get<object>("activityObject");
        Console.WriteLine("Activity in C#: " + activityObject.ToString());
    }

}



//ask the skill to find us a random activity via API
var skillApiRandomActivityResult = await sk.RunAsync(sk.Skills.GetNativeFunction(nameof(RandomActivitySkill), "GetRandomActivity"));
apiResult = skillApiRandomActivityResult.Result;
Console.WriteLine("Skill suggested: " + apiResult);

//lastly, for fun, find out how many degrees of separation exist between these concepts :)
var contextVariables = new ContextVariables();
contextVariables.Set("Item1", llmResult);
contextVariables.Set("Item2", apiResult);

var llmDegreesOfSeparationResult = await sk.RunAsync(contextVariables, sk.Skills.GetSemanticFunction("DegreesOfSeparation", "FindDegrees"));
Console.WriteLine("Degrees of separation: " + llmDegreesOfSeparationResult.Result);
