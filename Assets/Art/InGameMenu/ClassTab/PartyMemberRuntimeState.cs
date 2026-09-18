using System.Collections.Generic;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Mutable, per-save-file state for one party member. Kept separate from
    /// CharacterDefinition (which is a shared, authored asset) so progress
    /// doesn't get baked into project assets.
    /// </summary>
    public class PartyMemberRuntimeState
    {
        public CharacterDefinition definition;

        public int currentLevel;
        public int currentGold;

        /// <summary>Faction id -> numeric reputation value.</summary>
        public Dictionary<string, int> reputationValues = new Dictionary<string, int>();

        /// <summary>Faction id -> tier label ("Neutral", "Honored", "Exalted", ...).</summary>
        public Dictionary<string, string> reputationTiers = new Dictionary<string, string>();

        /// <summary>Ids of prior-class seals/tokens this character has earned.</summary>
        public HashSet<string> ownedSeals = new HashSet<string>();

        /// <summary>classId of every class this character has already unlocked/evolved into.</summary>
        public HashSet<string> unlockedClassIds = new HashSet<string>();

        public string currentClassId;

        public PartyMemberRuntimeState(CharacterDefinition definition)
        {
            this.definition = definition;
            if (definition.rootClass != null)
            {
                currentClassId = definition.rootClass.classId;
                unlockedClassIds.Add(definition.rootClass.classId);
            }
        }

        public bool IsClassUnlocked(ClassNodeData node) => unlockedClassIds.Contains(node.classId);
    }

    /// <summary>
    /// Checks a ClassNodeData's requirements against a PartyMemberRuntimeState
    /// and produces both the pass/fail result and the "current / required"
    /// strings the footer requirement rows display.
    /// </summary>
    public static class RequirementEvaluator
    {
        public struct Result
        {
            public bool met;
            public string currentDisplay;
            public string requiredDisplay;
        }

        public static bool AllMet(ClassNodeData node, PartyMemberRuntimeState state)
        {
            foreach (var req in node.requirements)
                if (!Evaluate(req, state).met)
                    return false;
            return true;
        }

        public static Result Evaluate(ClassRequirement req, PartyMemberRuntimeState state)
        {
            switch (req.type)
            {
                case RequirementType.Level:
                    return new Result
                    {
                        met = state.currentLevel >= req.requiredAmount,
                        currentDisplay = state.currentLevel.ToString(),
                        requiredDisplay = req.requiredAmount.ToString()
                    };

                case RequirementType.Gold:
                    return new Result
                    {
                        met = state.currentGold >= req.requiredAmount,
                        currentDisplay = state.currentGold.ToString(),
                        requiredDisplay = req.requiredAmount.ToString()
                    };

                case RequirementType.Reputation:
                    state.reputationTiers.TryGetValue(req.requiredTag, out var tier);
                    bool repMet = tier == req.requiredReputationTier;
                    return new Result
                    {
                        met = repMet,
                        currentDisplay = string.IsNullOrEmpty(tier) ? "—" : tier,
                        requiredDisplay = req.requiredReputationTier
                    };

                case RequirementType.PriorClassSeal:
                    bool hasSeal = state.ownedSeals.Contains(req.requiredTag);
                    return new Result
                    {
                        met = hasSeal,
                        currentDisplay = hasSeal ? "1" : "0",
                        requiredDisplay = "1"
                    };

                default:
                    return new Result { met = false, currentDisplay = "?", requiredDisplay = "?" };
            }
        }
    }
}
