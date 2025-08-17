namespace Perspective.Character.NPC
{
    public enum NpcEvent
    {
        None = 0,
        DisableEvent = 1,
        // --- Social / Neutral ---
        Conversation, // Two or more NPCs talking
        BeggingInteraction, // Beggar asks, Civilian responds (or ignores)
        CrowdGathering, // NPCs cluster to watch or chat

        // --- Conflict / Crime ---
        PickPocket, // Thief tries to steal from victim
        Intimidation, // Aggressor threatens victim
        Fight, // NPCs punching each other

        // --- Reactive / Crowd ---
        Yell, // Alert others about danger
        Chase, // One NPC pursues another (e.g., Guard → Thief)
        Panic, // Group scatters in fear

        // --- Complex / Emergent ---
        ChainReaction, // Combination event (e.g., theft → yell → chase)
        StreetPerformance // NPC draws attention (begging, yelling, performing)
    }
}