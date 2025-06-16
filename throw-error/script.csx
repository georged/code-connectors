using System.Net.Http;

public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        var contentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        var contentAsJson = JObject.Parse(contentAsString);

        var message = (string)contentAsJson["message"];
        var statusCode = (int)contentAsJson["statusCode"];

        var jsonResult = new JObject
        {
            ["message"] = message
        };

        return new HttpResponseMessage
        {
            StatusCode = (System.Net.HttpStatusCode)statusCode,
            Content = new StringContent(jsonResult.ToString(), System.Text.Encoding.UTF8, "application/json")
        };

    }
}
