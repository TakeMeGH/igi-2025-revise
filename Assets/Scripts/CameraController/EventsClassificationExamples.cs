using Perspective.Character.NPC;
using UnityEngine;

namespace Perspective.CameraController
{
    public enum ClassificationLabel
    {
        TidakRelevan,
        Netral,
        ProPemerintah,
        ProRakyat
    }

    [CreateAssetMenu(
        fileName = "EventsClassificationExamples",
        menuName = "Scriptable Objects/EventsClassificationExamples"
    )]
    public class EventsClassificationExamples : ScriptableObject
    {
        public NpcEvent npcEvent;

        [TextArea(2, 5)] public string imageContext;

        [System.Serializable]
        public class Example
        {
            [TextArea(2, 5)] public string headline;
            public ClassificationLabel output;
        }

        public Example[] examples;
    }
}