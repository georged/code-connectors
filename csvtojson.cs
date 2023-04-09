public class Script : ScriptBase
{
    private readonly List<List<string>> entries = new List<List<string>>();
    private string currentEntry = "";
    private bool insideQuotation = false;

    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        var contentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        this.Context.Logger.LogDebug($"Parsing headers: {this.Context.Request.Headers}");
        bool trimEntries = true;
        bool skipBlankEntries = true;
        bool.TryParse(GetHeaderString(this.Context.Request.Headers, "x-trim-whitespace"), out trimEntries);
        bool.TryParse(GetHeaderString(this.Context.Request.Headers, "x-skip-blank-entries"), out skipBlankEntries);
        this.Context.Logger.LogDebug($"trim: {trimEntries}, skip: {skipBlankEntries}");
        response.Content = CreateJsonContent(ConvertCsvToJsonObject(contentAsString, trimEntries, skipBlankEntries));
        return response;
    }

    private string ConvertCsvToJsonObject(string csvText, bool trimEntries, bool skipBlankEntries) 
    {
        var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var properties = lines[0].Split(',');
        for (int j = 0; j < properties.Length; j++) {
            properties[j] = properties[j].Trim();            
        }

        foreach (string line in lines.Skip(1)) {
            if(insideQuotation || !string.IsNullOrWhiteSpace(line)) {
                ScanNextLine(line);
            }
        }

        var listObjResult = new List<Dictionary<string, string>>();
        
        foreach(var entry in entries) {
            var emptyLine = true;
            var objResult = new Dictionary<string, string>();
            for (int j = 0; j < properties.Length; j++) {
                var value = entry[j];
                
                objResult.Add(properties[j], value);
                if(!string.IsNullOrWhiteSpace(value)) {
                    emptyLine = false;
                }
            }
            
            if(!emptyLine || !skipBlankEntries) {
                listObjResult.Add(objResult);
            }
        }

        return JsonConvert.SerializeObject(listObjResult); 
    }

    private void ScanNextLine(string line)
    {
        // At the beginning of the line
        if (!insideQuotation)
        {
            entries.Add(new List<string>());
        }

        // The characters of the line
        foreach (char c in line)
        {
            if (insideQuotation)
            {
                if (c == '"')
                {
                    insideQuotation = false;
                }
                else
                {
                    currentEntry += c;
                }
            }
            else if (c == ',')
            {
                entries[entries.Count - 1].Add(currentEntry);
                currentEntry = "";
            }
            else if (c == '"')
            {
                insideQuotation = true;
            }
            else 
            {
                currentEntry += c;
            }
        }

        // At the end of the line
        if (!insideQuotation)
        {
            entries[entries.Count - 1].Add(currentEntry);
            currentEntry = "";
        }
        else
        {
            currentEntry += "\n";
        }
    }
    private static string GetHeaderString(HttpHeaders headers, string name)
    {
            IEnumerable<string> values;

            if (headers.TryGetValues(name, out values))
            {
                return values.FirstOrDefault();
            }

            return null;
    }
}
