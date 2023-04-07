using System;

namespace MudSharp.Body.Traits {
    public class TraitChangedEventArgs : EventArgs {
        public TraitChangedEventArgs(ITrait trait, double oldValue, double newValue) {
            Trait = trait;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public ITrait Trait { get; protected set; }
        public double OldValue { get; protected set; }
        public double NewValue { get; protected set; }
    }
}