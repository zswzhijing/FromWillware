using UnityEngine;

public class BossWallManager : MonoBehaviour
{
    public Boss boss;
    public WallTrigger[] walls;
    
    void Start()
    {
        CheckSavedState();
        
        if (boss != null)
        {
            boss.OnBossDeath += OnBossDefeated;
        }
    }

    void CheckSavedState()
    {
        int defeated = PlayerPrefs.GetInt("BossDefeated_" + boss.name, 0);
        if (defeated == 1)
        {
            foreach (var wall in walls)
            {
                if (wall != null)
                {
                    wall.DestroyWall();
                }
            }
        }
    }

    void OnBossDefeated()
    {
        foreach (var wall in walls)
        {
            if (wall != null)
            {
                wall.DestroyWall();
            }
        }
        
        PlayerPrefs.SetInt("BossDefeated_" + boss.name, 1);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        if (boss != null)
        {
            boss.OnBossDeath -= OnBossDefeated;
        }
    }
}