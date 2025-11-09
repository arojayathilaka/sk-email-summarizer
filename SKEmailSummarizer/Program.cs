using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) 
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var openAiKey = config["OpenAIKey"]!;
var tenantId  = config["AzureTenantId"];
var clientId  = config["AzureClientId"];

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", openAiKey)
    .Build();

var emailSkill = new EmailPlugin(tenantId, clientId);

kernel.Plugins.AddFromObject(emailSkill, "email");

string goal = "Fetch unread emails using the 'email' skill, summarize them, and save the summary report.";

var planner = new FunctionCallingStepwisePlanner();

var result = await planner.ExecuteAsync(kernel, goal);

Console.WriteLine(result.FinalAnswer);



