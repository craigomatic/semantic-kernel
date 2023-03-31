namespace PythonSkillRunner;

public class RandomActivitySkillTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetRandomActivityAsync_ShouldReturnActivity()
    {
        var skill = new PythonRandomActivitySkill();
        var output = skill.GetRandomActivityAsync();
        Assert.IsTrue(output.Length > 0, "The output should not be empty.");
    }
}
