using NUnit.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
public class HighPriorityAttribute : CategoryAttribute
{ }
public class MediumPriorityAttribute : CategoryAttribute
{ }

public class LowPriorityAttribute : CategoryAttribute
{ }
