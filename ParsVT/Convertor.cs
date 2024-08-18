using Google.Protobuf.WellKnownTypes;
using Mysqlx.Expect;

namespace ParsVT;

public class Convertor
{
    public KeyValuePair<string, string> ToJson(string Input)
    {
        
        
        string[] elements = Input.Split(";");
        string key = elements[0];
        string value = elements[1];
        key = SplitString(key);
        value = SplitString(value);
        return new KeyValuePair<string, string>(key,value);
    }

    public Dictionary<string, string> JsonCovert(string input)
    {
        Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
        string[] elements = input.Split(";");
        for (int i = 0; i <= elements.Length; i += 2  )
        {
            string Key = elements[i];
            string Value = elements[i + 1];
           jsonDictionary.Add(SplitString(Key),SplitString(Value));
            
        }

        return jsonDictionary;
    }
    public string SplitString(string InputString)
    {
        string[] splitStrings = InputString.Split(":");
        return splitStrings.Last().Trim('\"');
       
    }

}