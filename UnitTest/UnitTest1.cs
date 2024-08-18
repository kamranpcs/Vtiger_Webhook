using ParsVT;

namespace UnitTest;

public class Tests
{


    [Test]
    public void Test1()
    {
        int a = 2;
        int b = 3;
        
        Assert.AreEqual(5,a+b);
    }

    [Test]
    public void TestTask()
    {
        string r = "s:10:\"workflowId\";\ns:2:\"81\";";
        var convertor = new Convertor();
        KeyValuePair<string, string> expected = new KeyValuePair<string, string>( "workflowId","81") ;
        Assert.AreEqual(expected,convertor.ToJson(r));

    }
}