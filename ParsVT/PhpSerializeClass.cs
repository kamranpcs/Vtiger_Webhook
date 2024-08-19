using Microsoft.AspNetCore.Mvc;

namespace ParsVT;

public class PhpSerializeClass
{
    public object ToJson(string input)
    {
        var Result = PhpSerializerNET.PhpSerialization.Deserialize(input);
        return Result;
    }
    
}