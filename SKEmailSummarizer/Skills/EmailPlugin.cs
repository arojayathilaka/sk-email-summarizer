using Azure.Identity;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

public class EmailPlugin
{
    private readonly GraphServiceClient _graph;

    public EmailPlugin(string tenantId, string clientId)
    {
        var credential = new DeviceCodeCredential(
            (info, ct) => { Console.WriteLine(info.Message); return Task.CompletedTask; },
            tenantId, clientId
        );
        
        _graph = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }


    [KernelFunction]
    [Description("Fetch unread emails (max 5) and return subject + preview.")]
    public async Task<string> FetchUnreadEmailsAsync()
    {
        // Fetch unread messages
        var messages = await _graph.Me.Messages.GetAsync(request =>
        {
            request.QueryParameters.Top = 5;
            request.QueryParameters.Filter = "isRead eq false";
            request.QueryParameters.Select = new[] { "subject", "bodyPreview" };
        });

        var sb = new StringBuilder();
        if (messages?.Value != null)
        {
            foreach (var m in messages.Value)
                sb.AppendLine($"Subject: {m.Subject}\nBody: {m.BodyPreview}\n---");
        }

        return sb.ToString();
    }

    [KernelFunction]
    [Description("Write the given text to a file named EmailSummaryReport.txt.")]
    public Task WriteReportAsync(string content)
    {
        File.WriteAllText("EmailSummaryReport.txt", content);
        return Task.CompletedTask;
    }
}
