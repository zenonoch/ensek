using System;
using System.IO;

namespace Framework;

public static class Globals 
{
    private static string testExecutionTimeStamp;
    public static string TestExecutionTimeStamp => testExecutionTimeStamp;
    public static string OutputDir => Path.Join(Directory.GetCurrentDirectory(), "Output-" + testExecutionTimeStamp);


    static Globals(){
        testExecutionTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    }
}
