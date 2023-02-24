using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingManager : SingletonDontDestroy<LevelingManager>
{
   // [SerializeField] private LevelWindow levelWindow;

    public LevelData levelData;
    public LevelWindow levelWindow;

    public LevelSystem levelSystem { get; private set; }
    public LevelSystemAnimated levelSystemAnimated { get; private set; }
    public override void Awake()
    {
        base.Awake();

        

        
    }
    private void Start()
    {
        levelData = Resources.Load<LevelData>("LevelData");

        levelSystem = new LevelSystem(levelData);
        levelWindow.SetLevelSystem(levelSystem);

        levelSystemAnimated = new LevelSystemAnimated(levelSystem);
        levelWindow.SetLevelSystemAnimated(levelSystemAnimated);

        GameSaveManager.OnResetGame += levelSystem.SetLevelSystem;
        GameSaveManager.OnResetGame += levelSystemAnimated.SetLevelSystem;
    }
    private void FixedUpdate()
    {
        levelSystem.Update();
        levelSystemAnimated.Update();
    }
}
