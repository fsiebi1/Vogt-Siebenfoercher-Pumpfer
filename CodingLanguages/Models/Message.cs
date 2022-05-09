using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Models {
    public class Message {

        public string Header { get; set; }
        public string MessageText { get; set; }
        public string Solution { get; set; }

        public Message() : this("", "", "") { }     // standard ctor
        public Message(string header, string message) : this(header, message, "") { } // erfolgsmeldungen
        public Message(string header, string message, string solution) {        // fehlermeldungen
            this.Header = header;
            this.MessageText = message;
            this.Solution = solution;
        }
    }
}