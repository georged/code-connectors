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
            ["StDev"] = StandardDeviation(input),
            ["Variance"] = Variance(input),
            ["Median"] = Median(input),
            ["ModeSngl"] = ModeSngl(input),
            ["Product"] = Product(input)
        };

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = CreateJsonContent(responseContent.ToString());
        return response;
    }

    static float StandardDeviation(List<float> input)
    {
        if(input.Count == 0) return 0;
        float avg = input.Average();
        float sum = input.Sum(v => (v - avg) * (v - avg));
        return (float)Math.Sqrt(sum / input.Count);
    }

    static float Variance(List<float> input)
    {
        if(input.Count == 0) return 0;
        float avg = input.Average();
        float sum = input.Sum(v => (v - avg) * (v - avg));
        return sum / input.Count;
    }

    static float Median(List<float> input)
    {
        if (input.Count == 0) return 0;
        input.Sort();
        int mid = input.Count / 2;
        if (input.Count % 2 == 0)
        {
            return (input[mid] + input[mid - 1]) / 2;
        }
        return input[mid];        
    }

    static float ModeSngl(List<float> input)
    {
        if (input.Count == 0) return 0;
        Dictionary<float, int> counts = new Dictionary<float, int>();
        foreach (var value in input)
        {
            if (counts.ContainsKey(value))
            {
                counts[value]++;
            }
            else
            {
                counts[value] = 1;
            }
        }
        return counts.OrderByDescending(kvp => kvp.Value).First().Key;
    }

    static float Product(List<float> input)
    {
        if (input.Count == 0) return 0;
        float product = 1;
        foreach (var value in input)
        {
            product *= value;
        }
        return product;
    }
}
