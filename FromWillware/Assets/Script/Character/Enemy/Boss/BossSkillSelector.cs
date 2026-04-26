using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSkillSelector
{
    private Boss boss;
    private List<BossSkill> skills;

    private Queue<BossSkill> recentSkills = new Queue<BossSkill>();
    private int memoryCount = 2;

    public BossSkillSelector(Boss boss, List<BossSkill> skills)
    {
        this.boss = boss;
        this.skills = skills;
    }

    public BossSkill ChooseSkill(float distance)
    {
        var available = skills
            .Where(s => distance >= s.minRange && distance <= s.maxRange)
            .Where(s => !recentSkills.Contains(s))
            .ToList();

        if (available.Count == 0)
        {
            available = skills
                .Where(s => distance >= s.minRange && distance <= s.maxRange)
                .ToList();

            if (available.Count == 0)
                return null;
        }

        var chosen = available[Random.Range(0, available.Count)];

        RegisterSkillUse(chosen);

        return chosen;
    }

    private void RegisterSkillUse(BossSkill skill)
    {
        recentSkills.Enqueue(skill);

        if (recentSkills.Count > memoryCount)
            recentSkills.Dequeue();
    }
}