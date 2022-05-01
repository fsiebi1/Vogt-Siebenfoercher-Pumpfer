using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Models {
    public class User {
        private int _userId;
        public int UserID {
            get { return this._userId; }
            set {
                if (value >= 0) {
                    this._userId = value;
                }
            }
        }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string CPassword { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public Gender Gender { get; set; }

        // ctor's + ToString()
    }
}
