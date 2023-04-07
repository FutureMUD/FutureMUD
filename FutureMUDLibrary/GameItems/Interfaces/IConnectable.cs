using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Shape;

namespace MudSharp.GameItems.Interfaces {
    public delegate void ConnectedEvent(IConnectable other, ConnectorType type);

    public interface IConnectable : IGameItemComponent {
        IEnumerable<ConnectorType> Connections { get; }
        IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems { get; }
        IEnumerable<ConnectorType> FreeConnections { get; }

        /// <summary>
        ///     Whether this item functions as an independent item when connected. If false, it becomes dependent on its connected
        ///     item
        /// </summary>
        bool Independent { get; }

        /// <summary>
        ///     Intended to be called in CanConnect, this performs the reverse check to see if the connection is also valid in the
        ///     other direction
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool CanBeConnectedTo(IConnectable other);

        /// <summary>
        ///     Determines whether or not this IConnectable can be connected to another IConnectable.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool CanConnect(ICharacter actor, IConnectable other);

        void Connect(ICharacter actor, IConnectable other);
        void RawConnect(IConnectable other, ConnectorType type);
        string WhyCannotConnect(ICharacter actor, IConnectable other);
        bool CanBeDisconnectedFrom(IConnectable other);
        bool CanDisconnect(ICharacter actor, IConnectable other);
        void Disconnect(ICharacter actor, IConnectable other);

        /// <summary>
        ///     Raw Disconnect takes no other action when disconnecting - the caller takes all responsibility for what happens to
        ///     the connectable from there
        /// </summary>
        /// <param name="other">The other connectable to disconnect</param>
        /// <param name="handleEvents">
        ///     Whether to fire the opposite connectable's similar events. Should generally be true when
        ///     called externally.
        /// </param>
        void RawDisconnect(IConnectable other, bool handleEvents);

        string WhyCannotDisconnect(ICharacter actor, IConnectable other);
    }

    public class ConnectorType {
        public ConnectorType(Gender gender, string connectionType, bool powered = true) {
            Gender = gender;
            ConnectionType = connectionType;
            Powered = powered;
        }

        public ConnectorType(string toString) {
            if (toString.IndexOf('-') == -1) {
                Gender = Gender.Indeterminate;
                ConnectionType = "Unknown";
                Powered = true;
            }
            else {
                var split = toString.Split('-');
                Gender = (Gender) short.Parse(split[0]);
                ConnectionType = split[1];
                Powered = (split.Length != 3) || bool.Parse(split[2]);
            }
        }

        public Gender Gender { get; }
        public string ConnectionType { get; }
        public bool Powered { get; }

        public bool CompatibleWith(ConnectorType type) {
            if (type?.ConnectionType.Equals(ConnectionType, StringComparison.InvariantCultureIgnoreCase) != true) {
                return false;
            }

            switch (Gender) {
                case Gender.Male:
                    return type.Gender == Gender.Female;
                case Gender.Female:
                    return type.Gender == Gender.Male;
                case Gender.Neuter:
                    return type.Gender == Gender.Neuter;
                case Gender.Indeterminate:
                    return true;
            }

            return true;
        }

        #region Overrides of Object

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return $"{(short) Gender:N0}-{ConnectionType}-{Powered}";
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            var objAsConnectorType = obj as ConnectorType;
            return (objAsConnectorType?.ConnectionType?.Equals(ConnectionType) ?? false) &&
                   (objAsConnectorType.Gender == Gender) &&
                   (objAsConnectorType.Powered == Powered);
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>
        ///     A hash code for the current object.
        /// </returns>
        public override int GetHashCode() {
            return Gender.GetHashCode() + ConnectionType.GetHashCode() + Powered.GetHashCode();
        }

        #endregion
    }
}