using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NovelCore.Tests.Editor
{

/// <summary>
/// Base class for all EditMode tests providing common setup/teardown.
/// </summary>
public abstract class BaseTestFixture
{
    [SetUp]
    public virtual void SetUp()
    {
    }

    [TearDown]
    public virtual void TearDown()
    {
    }

    protected void AssertNoLogErrors()
    {
        LogAssert.NoUnexpectedReceived();
    }
}

}
