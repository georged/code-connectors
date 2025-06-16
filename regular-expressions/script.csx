public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        var contentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        var contentAsJson = JObject.Parse(contentAsString);

        var textToMatch = (string)contentAsJson["input"];
        var regexInput = (string)contentAsJson["pattern"];
        var regexOptions = (string)contentAsJson["options"];

        var jSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        });

        var regex = new Regex(regexInput,
            (regexOptions == "IgnoreCase") ? (RegexOptions.IgnoreCase | RegexOptions.Compiled) :
            (regexOptions == "Singleline") ? (RegexOptions.Singleline | RegexOptions.Compiled) :
            (regexOptions == "Multiline") ? (RegexOptions.Multiline | RegexOptions.Compiled) :
            (RegexOptions.None | RegexOptions.Compiled),
 
            TimeSpan.FromSeconds(1)
        );

        switch (this.Context.OperationId)
        {
            case "RegexMatch":
                var resultMatch = new JObject
                {
                    ["isMatch"] = regex.IsMatch(textToMatch),
                    ["matches"] = JArray.FromObject(regex.Matches(textToMatch), jSerializer),
                };
                
                var responseMatch = new HttpResponseMessage(HttpStatusCode.OK);
                responseMatch.Content = CreateJsonContent(resultMatch.ToString());
                return responseMatch;
            case "RegexReplace":
                var regexNewText = (string)contentAsJson["newText"];

                var resultReplace = new JObject
                {
                    ["isMatch"] = regex.IsMatch(textToMatch),
                    ["output"] = regex.Replace(textToMatch,regexNewText),
                };

                var responseReplace = new HttpResponseMessage(HttpStatusCode.OK);
                responseReplace.Content = CreateJsonContent(resultReplace.ToString());
                return responseReplace;
            default:
                var responseDefault = new HttpResponseMessage(HttpStatusCode.BadRequest);
                responseDefault.Content = CreateJsonContent($"Unknown operation ID '{this.Context.OperationId}'");
                return responseDefault;
        }
 
        
    }

}