public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {        
        this.Context.Logger.LogInformation($"Start ExecuteAsync");
        var content = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        this.Context.Logger.LogInformation($"Content: {content}");

        JObject json = JObject.Parse(content);
        
        string property = json.GetValue("Property")?.Value<string>();
        JArray valuesArray = json.GetValue("Values") as JArray;
        if (valuesArray == null)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid input format. Expected a non-empty array of objects or float values.")
            };
        }

        List<float> input = new List<float>();
        foreach (var value in valuesArray)
        {
            if (value is JObject obj && property != null && obj.TryGetValue(property, out JToken propertyValue))
            {
                if (float.TryParse(propertyValue.ToString(), out float floatValue))
                {
                    input.Add(floatValue);
                }
                else {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"Invalid input format. Couldn't convert {propertyValue.ToString()} to float.")
                    };
                
                }
            }
            else if (value is JValue jValue)
            {
                if (float.TryParse(jValue.ToString(), out float floatValue))
                {
                    input.Add(floatValue);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent($"Invalid input format. Could not convert {jValue.ToString()} to float.")
                    };
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Invalid input format. Expected an array of objects with a {property} field or array of values.")
                };
            }
        }

        string valuesString = string.Join(", ", input.Select(v => v.ToString()));
        this.Context.Logger.LogInformation($"Received values: {valuesString}");

        var responseContent = new JObject
        {
            ["Sum"] = input.Sum(),
            ["Avg"] = input.Average(),
            ["Max"] = input.Max(),
            ["Min"] = input.Min(),
            ["StDev"] = StandardDeviation(input.ToArray())
        };

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = CreateJsonContent(responseContent.ToString());
        return response;
    }

    static float StandardDeviation(float[] input)
    {
        if(input.Length == 0) return 0;
        float avg = input.Average();
        float sum = input.Sum(v => (v - avg) * (v - avg));
        return (float)Math.Sqrt(sum / input.Length);
    }
}
