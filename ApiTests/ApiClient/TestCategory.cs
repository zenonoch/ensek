using NUnit.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
public class AccountsAttribute : CategoryAttribute
{ }
public class PoolsAttribute : CategoryAttribute
{ }

