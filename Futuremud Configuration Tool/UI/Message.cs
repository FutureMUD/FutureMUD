using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futuremud_Configuration_Tool.UI {
    /// <summary>
    /// The Message class is designed to handle program flow and error messages from the model, and implements INotifyPropertyChanged so it can use Data Binding.
    /// </summary>
    public class Message : INotifyPropertyChanged {
        protected static Message _instance = new Message();
        public static Message Instance { get { return _instance; } }

        public BindingList<string> Messages { get; set; }

        public Message() {
            Messages = new BindingList<string>();
        }

        public static void Send(string message) {
            Instance.Messages.Add(message);
        }

        protected void PropertyHasChanged(string whichProperty) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(whichProperty));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
