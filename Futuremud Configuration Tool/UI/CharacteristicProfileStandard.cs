using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using FME;
using MudSharp.Database;
using MudSharp.Form.Characteristics;

namespace Futuremud_Configuration_Tool.UI {

    public static class ProfileExtensions
    {
        public static bool SameOrChildDefinition(this FME.CharacteristicDefinition definition,
            FME.CharacteristicDefinition otherDefinition)
        {
            if (definition == null || otherDefinition == null)
            {
                return false;
            }

            if (definition.Id == otherDefinition.Id)
            {
                return true;
            }

            return otherDefinition.ParentId.HasValue && definition.SameOrChildDefinition(otherDefinition.Parent);
        }
    }

    public class CharacteristicProfileStandard : INotifyPropertyChanged
    {
        public class ValueSelection
        {
            public FME.CharacteristicValue Value { get; set; }
            public bool Selected { get; set; }
        }

        private long _ID;
        public long ID { get { return _ID; } set { _ID = value; OnPropertyChanged("ID"); } }

        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

        private string _description;
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged("Description"); } }

        private BindingList<ValueSelection> _values;

        public BindingList<ValueSelection> Values
        {
            get { return _values; }
            set
            {
                _values = value;
                OnPropertyChanged("Values");
            }
        }

        private FME.CharacteristicDefinition _definition;

        public FME.CharacteristicDefinition Definition
        {
            get { return _definition; }
            set
            {
                _definition = value;
                OnPropertyChanged("Definition");
                Values.Clear();

                var tempVals = new List<ValueSelection>(
                    (from item in FMDB.Context.CharacteristicValues.OrderBy(x => x.Name)
                    where item.CharacteristicDefinition.SameOrChildDefinition(Definition)
                    select new ValueSelection {Value = item, Selected = false}).ToList());
                if (Definition.Type == (int) CharacteristicType.Coloured)
                {
                    var colours = (from colour in FMDB.Context.Colours
                        select colour).ToDictionary(x => x.Id, x => x.Basic);
                    tempVals = new List<ValueSelection>(tempVals.OrderBy(x => colours[int.Parse(x.Value.Value)]).ThenBy(x => x.Value.Name).ToList());
                }

                foreach (var item in tempVals)
                {
                    Values.Add(item);
                }
            }
        }
        public CharacteristicProfileStandard()
        {
            _values = new BindingList<ValueSelection>(
                    (from item in FMDB.Context.CharacteristicValues.OrderBy(x => x.Name)
                     where item.CharacteristicDefinition.SameOrChildDefinition(Definition)
                     select new ValueSelection { Value = item, Selected = false }).ToList());
            if (Definition.Type == (int)CharacteristicType.Coloured) {
                var colours = (from colour in FMDB.Context.Colours
                               select colour).ToDictionary(x => x.Id, x => x.Basic);
                _values = new BindingList<ValueSelection>(Values.OrderBy(x => colours[int.Parse(x.Value.Value)]).ThenBy(x => x.Value.Name).ToList());
            }
        }

        public CharacteristicProfileStandard(FME.CharacteristicProfile definition)
        {
            _ID = definition.Id;
            _name = definition.Name;
            _description = definition.Description;
            _definition = definition.CharacteristicDefinition;

            using (new FMDB())
            {
                var xmlElements =
                    XElement.Parse(definition.Definition).Elements("Value").Select(x => int.Parse(x.Value)).ToList();
                var underlying = new List<ValueSelection>();
                foreach (
                    var item in
                        FMDB.Context.CharacteristicValues.OrderBy(x => x.Name)) {
                            if (item.CharacteristicDefinition.SameOrChildDefinition(Definition))
                            {
                                underlying.Add(new ValueSelection { Value = item, Selected = xmlElements.Any(x => x == item.Id) });
                            }
                }
                if (Definition.Type == (int)CharacteristicType.Coloured) {
                    var colours = (from colour in FMDB.Context.Colours
                                   select colour).ToDictionary(x => x.Id, x => x.Basic);
                    underlying = new List<ValueSelection>(underlying.OrderBy(x => colours[int.Parse(x.Value.Value)]).ThenBy(x => x.Value.Name));
                }
                _values = new BindingList<ValueSelection>(underlying);
            }
        }

        public void Save(FME.CharacteristicProfile profile)
        {
            profile.Name = Name;
            profile.Description = Description;
            profile.CharacteristicDefinition = Definition;
            profile.Definition = SaveToXml();
            profile.Type = "Standard";
            FMDB.Context.SaveChanges();
            _changed = false;
        }

        public void Save()
        {
            if (!_changed) return;
            using (new FMDB())
            {
                FME.CharacteristicProfile profile;
                if (ID == 0)
                {
                    profile = FMDB.Context.CharacteristicProfiles.Create();
                    FMDB.Context.CharacteristicProfiles.Add(profile);
                }
                else
                {
                    profile = FMDB.Context.CharacteristicProfiles.Find(ID);
                }

                if (profile == null)
                {
                    return;
                }

                profile.Name = Name;
                profile.Definition = Description;
                profile.CharacteristicDefinition = Definition;
                profile.Definition = SaveToXml();
                profile.Type = "Standard";
                FMDB.Context.SaveChanges();
            }
            _changed = false;
        }

        private string SaveToXml()
        {
            return new XElement("Definition", new object[]
            {
                from item in Values
                where item.Selected
                select new XElement("Value", item.Value.Id)
            }
                ).ToString();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _changed = false;
        private void OnPropertyChanged(string property)
        {
            _changed = true;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
