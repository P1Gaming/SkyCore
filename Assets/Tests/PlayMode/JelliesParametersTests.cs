using Jellies;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class JelliesParametersTests
{
    private void CreateTestJelly(out GameObject testJelly, out Parameters parameters)
    {
        testJelly = new GameObject("TestJelly");
        parameters = testJelly.AddComponent<Parameters>();
    }
    
    [UnityTest]
    public IEnumerator TestIncreaseFoodSaturation()
    {
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float initValue = 50;
        float increaseValue = 10;
        parameters.SetFoodSaturation(initValue);
        
        parameters.IncreaseFoodSaturation(increaseValue);
        
        Assert.AreEqual(initValue + increaseValue, parameters.FoodSaturation);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator TestDecreaseFoodSaturation()
    { 
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float initValue = 50;
        float increaseValue = 10;
        parameters.SetFoodSaturation(initValue);
        
        parameters.DecreaseFoodSaturation(increaseValue);
        
        Assert.AreEqual(initValue - increaseValue, parameters.FoodSaturation);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestIfFoodDoesNotExceedMaxSaturation()
    {
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float maxValue = parameters.MaxFoodSaturation;
        parameters.SetFoodSaturation(maxValue);
        parameters.IncreaseFoodSaturation(1f);

        Assert.LessOrEqual(parameters.FoodSaturation, maxValue);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestIfJellyGetsHungrierAfter10Seconds()
    {
        CreateTestJelly(out GameObject testJelly, out Parameters parameters);
        float initValue = parameters.MaxFoodSaturation;
        float timeToWait = 10f;
        parameters.SetFoodSaturation(initValue);
        
        yield return new WaitForSeconds(timeToWait);
        Assert.LessOrEqual(parameters.FoodSaturation, parameters.MaxFoodSaturation);
    }
}