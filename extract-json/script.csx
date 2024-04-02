
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        this.Context.Logger.LogInformation($"Start ExecuteAsync");
        var content = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        this.Context.Logger.LogInformation($"Content: {content}");

        JObject json = JObject.Parse(content);

        string regex = json.GetValue("Regex")?.Value<string>();
        string replace = json.GetValue("Replace")?.Value<string>();
        JObject input = json.GetValue("Input") as JObject;
        if (input == null)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid input format. Expected an object.")
            };
        }

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = CreateJsonContent(TransformJsonProperties(input, new Regex(regex), replace).ToString())
        };
    }

    static JObject TransformJsonProperties(JObject originalJson, Regex propertyRegex, string replacementString)
    {
        // Create a new JObject to store the transformed properties
        JObject transformedJson = new JObject();

        // Iterate over all JProperties in the original JObject
        foreach (var property in originalJson.Properties())
        {
            // Check if the property name matches the regex
            if (propertyRegex.IsMatch(property.Name))
            {
                // Replace the matched part of the property name and add it to the new JObject
                string newName = propertyRegex.Replace(property.Name, replacementString);
                transformedJson.Add(newName, property.Value);
            }
        }

        return transformedJson;
    }

}

