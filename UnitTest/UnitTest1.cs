using NUnit.Framework.Internal;
using ParsVT;

namespace UnitTest;

public class Tests
{
    [Test]
    public void TestTask()
    {
        string r = "s:10:\"workflowId\";\ns:2:\"81\";";
        var convertor = new Convertor();
        KeyValuePair<string, string> expected = new KeyValuePair<string, string>( "workflowId","81") ;
        Assert.AreEqual(expected,convertor.ToJson(r));

    }

    [Test]
    public void MultiStringConvertTest()
    {
        string input = "s:10:\"workflowId\";s:2:\"82\";s:7:\"summary\";s:14:\"test webhook 2\";s:6:\"active\";b:1;s:7:\"trigger\";N;s:19:\"field_value_mapping\";s:155:\"[{\"fieldname\":\"\\u062a\\u0633\\u062a 1\",\"value\":\"mobile\",\"valuetype\":\"fieldname\"},{\"fieldname\":\"\\u062a\\u0633\\u062a 3\",\"value\":\"city\",\"valuetype\":\"fieldname\"}]\"";
        var convertor = new Convertor();
        Dictionary<string, string> expected = new Dictionary<string, string>
        {
            {
                "workflowId","82"
            },
            {
                "summary","test webhook 2"
            },
            {
                "active", "1"
            },
            {
                "trigger","N"
            },
            {
                "field_value_mapping" ,"{\"fieldname\":\"\\u062a\\u0633\\u062a 1\",\"value\":\"mobile\",\"valuetype\":\"fieldname\"},{\"fieldname\":\"\\u062a\\u0633\\u062a 3\",\"value\":\"city\",\"valuetype\":\"fieldname\"}"
            }
          
        };
        Assert.AreEqual(expected.Values,convertor.JsonCovert(input).Values);
        
    }
}