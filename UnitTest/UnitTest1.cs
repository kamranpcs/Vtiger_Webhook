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
        string input = "s:10:\"workflowId\";s:2:\"82\";s:7:\"summary\";s:14:\"test webhook 2\";s:6:\"active\";b:1";
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
            }
          
        };
        Assert.AreEqual(expected.Keys,convertor.JsonCovert(input).Keys);
        
    }
}