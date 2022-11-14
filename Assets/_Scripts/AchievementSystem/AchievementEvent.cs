using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AchievementEvent
{
    public abstract float pointsAchievement();
}

public class itemPoints : AchievementEvent
{
    public override float pointsAchievement()
    {
        return 100;
    }
}

public class fastPoints : AchievementEvent
{
    public override float pointsAchievement()
    {
        return 20;
    }
}

public class driftPoints : AchievementEvent
{
    public override float pointsAchievement()
    {
        return 10;
    }
}

public class hitPoints : AchievementEvent
{
    public override float pointsAchievement()
    {
        return 50;
    }
}
