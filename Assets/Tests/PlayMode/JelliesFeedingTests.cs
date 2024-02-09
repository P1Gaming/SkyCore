using Jellies;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class JelliesFeedingTests
{
    [UnityTest]
    public IEnumerator TestIfJellyCanBeFed()
    {
        GameObject testJelly = new GameObject("TestJelly");
        Parameters parameters = testJelly.AddComponent<Parameters>();
        Feeding feeding = testJelly.AddComponent<Feeding>();
        float initValue = 50;
        float increasseValue = 10;
        parameters.SetFoodSaturation(initValue);
        
        feeding.FeedJelly(increasseValue);
        
        Assert.AreEqual(initValue + increasseValue, parameters.FoodSaturation);
        yield return null;
    }
}