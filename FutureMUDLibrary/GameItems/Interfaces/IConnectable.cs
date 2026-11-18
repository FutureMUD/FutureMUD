using MudSharp.Character;
using MudSharp.Form.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

#nullable enable annotations

namespace MudSharp.GameItems.Interfaces
{
    public delegate void ConnectedEvent(IConnectable other, ConnectorType type);

    public interface IConnectable : IGameItemComponent
    {
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
        bool CanConnect(ICharacter? actor, IConnectable other);

        void Connect(ICharacter? actor, IConnectable other);
        void RawConnect(IConnectable other, ConnectorType type);
        string WhyCannotConnect(ICharacter? actor, IConnectable other);
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

    public class ConnectorType
    {
        public ConnectorType(Gender gender, string connectionType, bool powered = true, int? portIndex = null,
            string? portId = null)
        {
            Gender = gender;
            ConnectionType = connectionType;
            Powered = powered;
            PortIndex = portIndex;
            PortId = portId ?? Guid.NewGuid().ToString("N");
        }

        public ConnectorType(string toString)
        {
            if (toString.IndexOf('-') == -1)
            {
                Gender = Gender.Indeterminate;
                ConnectionType = "Unknown";
                Powered = true;
            }
            else
            {
                string[] split = toString.Split('-');
                Gender = (Gender)short.Parse(split[0]);
                ConnectionType = split[1];
                Powered = (split.Length != 3) || bool.Parse(split[2]);
            }
        }

        public ConnectorType(string toString, int? portIndex, int? peerPortIndex = null, string? portId = null,
            string? peerPortId = null) : this(toString)
        {
            PortIndex = portIndex;
            PeerPortIndex = peerPortIndex;
            PortId = portId;
            PeerPortId = peerPortId;
        }

        public Gender Gender { get; }
        public string ConnectionType { get; }
        public bool Powered { get; }

        /// <summary>The zero-based physical connector position recorded by a persisted connection, when known.</summary>
        public int? PortIndex { get; private set; }

        /// <summary>The physical connector position on the opposite endpoint, when recorded.</summary>
        public int? PeerPortIndex { get; private set; }

        /// <summary>The persistent physical connector identity on the opposite endpoint.</summary>
        public string? PeerPortId { get; private set; }

        /// <summary>Persistent identity of this physical connector within its component prototype.</summary>
        public string? PortId { get; private set; }

        internal void SetPortId(string portId) => PortId = portId;

        public static (ConnectorType First, ConnectorType Second) CreatePairedConnectionInstances(
            ConnectorType firstPort, ConnectorType secondPort)
        {
            if (!firstPort.CompatibleWith(secondPort))
            {
                throw new ArgumentException("Connector ports must be compatible to form a physical pair.");
            }

            var first = firstPort.CreateConnectionInstance();
            var second = secondPort.CreateConnectionInstance();
            first.PeerPortId = second.PortId;
            second.PeerPortId = first.PortId;
            return (first, second);
        }

        private ConnectorType CreateConnectionInstance() =>
            new(ToString(), PortIndex, PeerPortIndex, PortId, PeerPortId);

        /// <summary>
        /// Assigns persistent identities to connector definitions. Existing explicit IDs are retained; older
        /// definitions receive deterministic IDs so item connections can be upgraded without ambiguity.
        /// </summary>
        public static void EnsurePrototypePortIds(long prototypeId, IEnumerable<ConnectorType> connectors, XElement? definition)
        {
            var ports = connectors.ToArray();
            var savedPorts = definition?.Element("Connectors")?.Elements("Connection").ToArray() ??
                             (definition?.Element("InputConnectors")?.Elements("Connection") ?? [])
                             .Concat(definition?.Element("OutputConnectors")?.Elements("Connection") ?? [])
                             .ToArray();
            if (savedPorts.Length == 0)
            {
                savedPorts = definition?.Elements().Where(x => x.Name.LocalName is "Connection" or "Connector" or "VenousConnection" or "ArterialConnection").ToArray() ?? [];
            }
            var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
            for (var i = 0; i < ports.Length; i++)
            {
                var port = ports[i];
                var key = $"{(short)port.Gender}|{port.ConnectionType}|{port.Powered}";
                occurrences.TryGetValue(key, out var occurrence);
                occurrences[key] = occurrence + 1;
                var savedId = i < savedPorts.Length ? savedPorts[i].Attribute("id")?.Value : null;
                port.SetPortId(string.IsNullOrWhiteSpace(savedId)
                    ? DeterministicPortId(prototypeId, key, occurrence)
                    : savedId);
            }
        }

        private static string DeterministicPortId(long prototypeId, string descriptor, int occurrence)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"FutureMUD connector port|{prototypeId}|{descriptor}|{occurrence}"));
            return new Guid(bytes.AsSpan(0, 16)).ToString("N");
        }

        public bool CompatibleWith(ConnectorType type)
        {
            if (type?.ConnectionType.Equals(ConnectionType, StringComparison.InvariantCultureIgnoreCase) != true)
            {
                return false;
            }

            switch (Gender)
            {
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
        public override string ToString()
        {
            return $"{(short)Gender:N0}-{ConnectionType}-{Powered}";
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            ConnectorType objAsConnectorType = obj as ConnectorType;
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
        public override int GetHashCode()
        {
            return Gender.GetHashCode() + ConnectionType.GetHashCode() + Powered.GetHashCode();
        }

        #endregion
    }
}
