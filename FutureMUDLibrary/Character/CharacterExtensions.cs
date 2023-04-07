using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Character {
    public static class CharacterExtensions {
        public static bool IsDead(this CharacterState state)
        {
            return state.HasFlag(CharacterState.Dead);
        }

        public static bool IsAlive(this CharacterState state)
        {
            return !state.HasFlag(CharacterState.Dead);
        }

        public static bool IsAble(this CharacterState state)
        {
            return CharacterState.Able.HasFlag(state);
        }

        public static bool IsDisabled(this CharacterState state)
        {
            return !CharacterState.Able.HasFlag(state);
        }

        public static bool IsConscious(this CharacterState state)
        {
            return CharacterState.Conscious.HasFlag(state);
        }

        public static bool IsUnconscious(this CharacterState state)
        {
            return !CharacterState.Conscious.HasFlag(state);
        }

        public static bool IsAsleep(this CharacterState state)
        {
            return state.HasFlag(CharacterState.Sleeping);
        }

        public static bool IsAwake(this CharacterState state)
        {
            return !state.HasFlag(CharacterState.Sleeping);
        }

        public static string Describe(this CharacterState state) {
            switch (state) {
                case CharacterState.Awake:
                    return "Awake";
                case CharacterState.Sleeping:
                    return "Sleeping";
                case CharacterState.Unconscious:
                    return "Unconscious";
                case CharacterState.Paralysed:
                    return "Paralysed";
                case CharacterState.Dead:
                    return "Dead";
                case CharacterState.Stasis:
                    return "In Stasis";
            }

            var states = new List<string>();
            if (state.HasFlag(CharacterState.Sleeping)) {
                states.Add("Sleeping");
            }
            if (state.HasFlag(CharacterState.Unconscious)) {
                states.Add("Unconscious");
            }
            if (state.HasFlag(CharacterState.Paralysed)) {
                states.Add("Paralysed");
            }
            if (state.HasFlag(CharacterState.Stasis))
            {
                states.Add("In Stasis");
            }
            if (state.HasFlag(CharacterState.Dead))
            {
                states.Add("Dead");
            }

            if (!states.Any()) {
                return "Unknown";
            }

            return states.ListToString();
        }
    }
}