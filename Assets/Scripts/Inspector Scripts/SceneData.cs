using UnityEngine;


[CreateAssetMenu(fileName = "NewSceneData", menuName = "Scene Data", order = 200)]
public class SceneData : ScriptableObject
{
    public string sceneName;

   public roomProperties.roomType roomType;

    public enum EntranceDir { Unassigned, North, East, South, West}
    public EntranceDir entranceDir;

    public enum roomTheme { Default, Puzzle, TimedPuzzle, Boss }
    public roomTheme roomtheme;

    public bool hasCollectible;
}
