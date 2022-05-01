using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingLanguages.Models.ViewModels {
    public class RegistrationViewModel {
        public User User {
            get; set;
        }
        public List<String> Countries {
            get; set;
        }
    }
}
