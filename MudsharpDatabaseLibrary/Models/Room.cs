using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Room
    {
        public Room()
        {
            ActiveProjects = new HashSet<ActiveProject>();
            RoomOverlays = new HashSet<RoomOverlay>();
            RoomsForagableYields = new HashSet<RoomsForagableYield>();
            RoomsGameItems = new HashSet<RoomsGameItems>();
            RoomsMagicResources = new HashSet<RoomMagicResource>();
            RoomsRangedCovers = new HashSet<RoomsRangedCovers>();
            RoomsTags = new HashSet<RoomsTags>();
            CharacterLog = new HashSet<CharacterLog>();
            Characters = new HashSet<Character>();
            CharacterInstances = new HashSet<CharacterInstance>();
            ClansAdministrationRooms = new HashSet<ClanAdministrationRoom>();
            ClansHallRooms = new HashSet<ClanHallRoom>();
            ClansTreasuryRooms = new HashSet<ClanTreasuryRoom>();
            Crimes = new HashSet<Crime>();
            HooksPerceivables = new HashSet<HooksPerceivable>();
            ShopsStockroomRoom = new HashSet<Shop>();
            ShopsStoreroomRooms = new HashSet<ShopsStoreroomRoom>();
            ShopsWorkshopRoom = new HashSet<Shop>();
            Zones = new HashSet<Zone>();
            Tracks = new HashSet<Track>();
            VehicleDockings = new HashSet<VehicleDocking>();
        }

        public long Id { get; set; }
        public long? CurrentOverlayId { get; set; }
        public long? ForagableProfileId { get; set; }
        public bool Temporary { get; set; }
        public string EffectData { get; set; }
        public long? HostedVehicleId { get; set; }
        public long? HostedVehicleCompartmentId { get; set; }

        public virtual RoomOverlay CurrentOverlay { get; set; }
        public virtual AgricultureField AgricultureField { get; set; }
        public virtual RouteRoom RouteRoom { get; set; }
        public virtual Vehicle HostedVehicle { get; set; }
        public virtual VehicleCompartment HostedVehicleCompartment { get; set; }
        public virtual ICollection<ActiveProject> ActiveProjects { get; set; }
        public virtual ICollection<RoomOverlay> RoomOverlays { get; set; }
        public virtual ICollection<RoomsForagableYield> RoomsForagableYields { get; set; }
        public virtual ICollection<RoomsGameItems> RoomsGameItems { get; set; }
        public virtual ICollection<RoomMagicResource> RoomsMagicResources { get; set; }
        public virtual ICollection<RoomsRangedCovers> RoomsRangedCovers { get; set; }
        public virtual ICollection<RoomsTags> RoomsTags { get; set; }
        public virtual ICollection<CharacterLog> CharacterLog { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<CharacterInstance> CharacterInstances { get; set; }
        public virtual ICollection<ClanAdministrationRoom> ClansAdministrationRooms { get; set; }
        public virtual ICollection<ClanHallRoom> ClansHallRooms { get; set; }
        public virtual ICollection<ClanTreasuryRoom> ClansTreasuryRooms { get; set; }
        public virtual ICollection<Crime> Crimes { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
        public virtual ICollection<Shop> ShopsStockroomRoom { get; set; }
        public virtual ICollection<ShopsStoreroomRoom> ShopsStoreroomRooms { get; set; }
        public virtual ICollection<Shop> ShopsWorkshopRoom { get; set; }
        public virtual ICollection<Zone> Zones { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
        public virtual ICollection<VehicleDocking> VehicleDockings { get; set; }
    }
}
