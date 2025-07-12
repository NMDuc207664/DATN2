using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DATN2.Scripts.BehaviorEditor
{
    public class CommentNode : BaseNode
    {
        // string comment = "This is a comment";
        public Dictionary<Character, EmotionType> CharacterEmotions = new Dictionary<Character, EmotionType>();
        public override void DrawWindow()
        {
            //comment = GUILayout.TextField(comment, 200);
            GUILayout.Label("Condition Node: ", EditorStyles.boldLabel);
            foreach (Character character in System.Enum.GetValues(typeof(Character)))
            {
                if (!CharacterEmotions.ContainsKey(character))
                    CharacterEmotions[character] = EmotionType.Happy; // Default emotion

                CharacterEmotions[character] = (EmotionType)EditorGUILayout.EnumPopup(character.ToString(), CharacterEmotions[character]);
            }
        }
        public override void DrawCurves() { }
        public bool HasEmotion(Character character, EmotionType emotion)
        {
            return CharacterEmotions.ContainsKey(character) && CharacterEmotions[character] == emotion;
        }

        // Method to set a character's emotion
        public void SetEmotion(Character character, EmotionType emotion)
        {
            CharacterEmotions[character] = emotion;
        }
    }
}
public enum EmotionType
{
    Happy,
    Anger,
    Fear
}

public enum Character
{
    A,
    B,
    C
}