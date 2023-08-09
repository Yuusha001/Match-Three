using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace MatchThreeEngine
{
    [CreateAssetMenu(menuName = "Match 3 Engine/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Settup")]
        [Range(0,10)]
        public int rowLength;
        [Range(0, 10)]
        public int colLength;
        [Range(0, 10)]
        public int numberOfType;
        [Header("Point per Star")]
        [Range(0, 1000)]
        public int _1stStar;
        [Range(1000, 1500)]
        public int _2ndStar;
        [Range(1500, 2000)]
        public int _3rdStar;

        [Header("Win Condition")]
        public EGameMode gameMode;
        public int totalTurns;
        [HideInInspector] public int tileTypeCollect;
    }

    public enum EGameMode
    {
        Normal,
        Collect,
        Break
    };

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelData))]
    public class Custom_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // for other non-HideInInspector fields

            LevelData script = (LevelData)target;
            if (script.gameMode is EGameMode.Collect)
            {
                script.tileTypeCollect = EditorGUILayout.IntField("Collect type", script.tileTypeCollect);
            }

            /*if (script.gameMode is EGameMode.Collect) // if bool is true, show other fields
            {
                script.useLevelLoad = EditorGUILayout.Toggle("Use Load Level", script.useLevelLoad);
                if (script.useLevelLoad) // if bool is true, show other fields
                {
                    script.loadLevelTest = EditorGUILayout.IntField("Level", script.loadLevelTest);
                }
            }*/
        }
    }
#endif
}
