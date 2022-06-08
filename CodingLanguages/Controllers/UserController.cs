using CodingLanguages.Models;
using CodingLanguages.Models.DB;
using CodingLanguages.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

// multilingual
// newsletter

namespace CodingLanguages.Controllers {
    public class UserController : Controller {

        private IRepositoryUsers _rep = new RepositoryUserDB();

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Register() {
            RegistrationViewModel rvm = new RegistrationViewModel();
            rvm.User = new Models.User();
            rvm.User.Birthdate = DateTime.Now;
            rvm.Countries = getCountriesEN();
            return View(rvm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel userDataFromForm) {

            if (userDataFromForm == null) {
                return RedirectToAction("Registration");
            }

            ValidateRegistrationData(userDataFromForm.User);

            if (ModelState.IsValid) {
                try {
                    await _rep.ConnectAsync();

                    if (await _rep.InsertAsync(userDataFromForm.User)) {
                        HttpContext.Session.SetString("name", userDataFromForm.User.Username);
                        return View("_Message", new Message("Registrierung", "Hallo " + userDataFromForm.User.Username + "!\nSie haben sich erfolgreich registriert!"));
                    }
                    else {
                        return View("_Message", new Message("Registrierung", "Registrierung fehlgeschlagen", "Bitte probieren Sie es später erneut"));
                    }

                }
                catch (DbException) {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message("Registrierung", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

                }
                finally {
                    await _rep.DisconnectAsync();
                }
            }

            userDataFromForm.Countries = getCountriesEN();
            return View(userDataFromForm);
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User userDataFromForm) {

            if (userDataFromForm == null)
            {
                return RedirectToAction("Registration");
            }

            ValidateLoginData(userDataFromForm);

            if (ModelState.IsValid)
            {
                try
                {
                    await _rep.ConnectAsync();
                    User wholeUser = await _rep.LoginAsync(userDataFromForm.Username, userDataFromForm.Password);

                    if (wholeUser != null)
                    {
                        HttpContext.Session.SetString("name", wholeUser.Username);
                        HttpContext.Session.SetInt32("admin", wholeUser.Admin);
                        return View("_Message", new Message("Login", "Hallo " + wholeUser.Username + "! Sie haben sich erfolgreich eingeloggt!"));
                    }
                    else
                    {
                        return View("_Message", new Message("Login", "Login fehlgeschlagen", "Username / Email oder Password falsch!"));
                    }

                }
                catch (DbException)
                {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message("Login", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

                }
                finally
                {
                    await _rep.DisconnectAsync();
                }
            }
            return View(userDataFromForm);
        }

        public IActionResult Logout() {
            HttpContext.Session.SetString("name", "");
            HttpContext.Session.SetInt32("admin", 0);
            return View("_Message", new Message("Logout", "Sie haben sich erfolgreich ausgeloggt!"));
        }
 
        public async Task<IActionResult> Profil() {
            if(HttpContext.Session.GetString("name") == "" || HttpContext.Session.GetString("name") == null)
                return RedirectToAction("Login");

            try {
                await _rep.ConnectAsync();
                User user = await _rep.GetUserAsync(HttpContext.Session.GetString("name"));

                if (user != null) {
                    return View(user);
                }
                else {
                    return View("_Message", new Message("Profil", "Daten konnten nicht vom Server geladen werden", "Bitte probieren Sie es später erneut"));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message("Profil", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

            }
            finally {
                await _rep.DisconnectAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update() {
            if (HttpContext.Session.GetString("name") == "" || HttpContext.Session.GetString("name") == null)
                return RedirectToAction("Login");

            try {
                await _rep.ConnectAsync();
                User user = await _rep.GetUserAsync(HttpContext.Session.GetString("name"));

                if (user != null) {
                    RegistrationViewModel rvm = new RegistrationViewModel();
                    rvm.Countries = getCountriesEN();
                    user.UsernameOld = user.Username;
                    user.EmailOld = user.Email;
                    rvm.User = user;
                    return View(rvm);
                }
                else {
                    return View("_Message", new Message("Update", "Daten konnten nicht vom Server geladen werden", "Bitte probieren Sie es später erneut"));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message("Update", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

            }
            finally {
                await _rep.DisconnectAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(RegistrationViewModel userDataFromForm) {

            if (userDataFromForm == null) {
                return RedirectToAction("Update");
            }

            ValidateUpdateData(userDataFromForm.User);

            try {
                await _rep.ConnectAsync();
                User u = await _rep.LoginAsync(userDataFromForm.User.UsernameOld, userDataFromForm.User.Password);
                
                if(u == null) {
                    ModelState.AddModelError("User.Password", "Falsches Passwort!");
                }
            } catch(DbException) {
                return View("_Message", new Message("Update", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));
            } finally {
                await _rep.DisconnectAsync();
            }

            if (ModelState.IsValid) {
                try {
                    await _rep.ConnectAsync();

                    if (await _rep.UpdateAsync(userDataFromForm.User)) {
                        HttpContext.Session.SetString("name", userDataFromForm.User.Username);
                        return View("_Message", new Message("Update", "User '" + userDataFromForm.User.Username + "' wurde erfolgreich upgedated!"));
                    }
                    else {
                        return View("_Message", new Message("Update", "Update fehlgeschlagen", "Bitte probieren Sie es später erneut"));
                    }

                }
                catch (DbException) {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message("Update", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

                }
                finally {
                    await _rep.DisconnectAsync();
                }
            }

            userDataFromForm.Countries = getCountriesEN();
            return View(userDataFromForm);
        }

        [HttpGet]
        public IActionResult ChangePassword() {
            if (HttpContext.Session.GetString("name") == "" || HttpContext.Session.GetString("name") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(User userDataFromForm) {
            if (userDataFromForm == null) {
                return RedirectToAction("ChangePassword");
            }

            ValidatePasswordData(userDataFromForm);

            try {
                await _rep.ConnectAsync();
                User u = await _rep.LoginAsync(HttpContext.Session.GetString("name"), userDataFromForm.PasswordOld);

                if (u == null) {
                    ModelState.AddModelError("PasswordOld", "Falsches Passwort!");
                }
            }
            catch (DbException) {
                return View("_Message", new Message("Password Change", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));
            }
            finally {
                await _rep.DisconnectAsync();
            }

            if (ModelState.IsValid) {
                try {
                    await _rep.ConnectAsync();

                    if (await _rep.SetPasswordAsync(HttpContext.Session.GetString("name"), userDataFromForm.Password)) {
                        return View("_Message", new Message("Password Change", "Passwort erfolgreich geändert!"));
                    }
                    else {
                        return View("_Message", new Message("Password Change", "Passwortänderung fehlgeschlagen", "Bitte probieren Sie es später erneut"));
                    }

                }
                catch (DbException) {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message("Password Change", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

                }
                finally {
                    await _rep.DisconnectAsync();
                }
            }

            return View(userDataFromForm);
        }

        public async Task<IActionResult> Delete() {
            if (HttpContext.Session.GetString("name") == "" || HttpContext.Session.GetString("name") == null)
                return RedirectToAction("Login");

            string username = HttpContext.Session.GetString("name");

            try {
                await _rep.ConnectAsync();

                if (await _rep.DeleteAsync(username)) {
                    HttpContext.Session.SetString("name", "");
                    HttpContext.Session.SetInt32("admin", 0);
                    return View("_Message", new Message("Delete", "User '" + username + "' wurde erfolgreich gelöscht!"));
                }
                else {
                    return View("_Message", new Message("Delete", "Delete fehlgeschlagen", "Bitte probieren Sie es später erneut"));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message("Delete", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

            }
            finally {
                await _rep.DisconnectAsync();
            }
        }

        public async Task<IActionResult> AdminArea() {
            if (HttpContext.Session.GetString("name") == "" || HttpContext.Session.GetString("name") == null)
                return RedirectToAction("Login");

            if (HttpContext.Session.GetInt32("admin") != 1 )
                return View("_Message", new Message("Admin Area", "Fehlende Zugriffsberechtigung", "Bitte melden Sie sich als Admin an!"));

            try
            {
                await _rep.ConnectAsync();
                List<User> users = await _rep.GetAllUsersAsync();

                if (users?.Count() != 0)
                {
                    return View(users);
                }
                else
                {
                    return View("_Message", new Message("Admin Area", "Keine Daten vorhanden!", "Bitte probieren Sie es später erneut"));
                }

            }
            catch (DbException)
            {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message("Admin Area", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

            }
            finally
            {
                await _rep.DisconnectAsync();
            }
        }

        public async Task<IActionResult> DeleteAdmin(string id)
        {
            if(HttpContext.Session.GetInt32("admin") != 1)
                return View("_Message", new Message("Admin Area", "Fehlende Zugriffsberechtigung", "Bitte melden Sie sich als Admin an!"));

            if (HttpContext.Session.GetString("name") == id)
                return RedirectToAction("Delete");

            try {
                await _rep.ConnectAsync();

                if (await _rep.DeleteAsync(id)) {
                    return View("_Message", new Message("Delete", "User '" + id + "' wurde erfolgreich gelöscht!"));
                }
                else {
                    return View("_Message", new Message("Delete", "Delete fehlgeschlagen", "Bitte probieren Sie es später erneut"));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message("Delete", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

            }
            finally {
                await _rep.DisconnectAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAdmin(string id)
        {
            if (HttpContext.Session.GetInt32("admin") != 1)
                return View("_Message", new Message("Admin Area", "Fehlende Zugriffsberechtigung", "Bitte melden Sie sich als Admin an!"));

            try
            {
                await _rep.ConnectAsync();
                User user = await _rep.GetUserAsync(id);

                if (user != null)
                {
                    RegistrationViewModel rvm = new RegistrationViewModel();
                    rvm.Countries = getCountriesEN();
                    user.UsernameOld = user.Username;
                    user.EmailOld = user.Email;
                    rvm.User = user;
                    return View(rvm);
                }
                else
                {
                    return View("_Message", new Message("Update", "Daten konnten nicht vom Server geladen werden", "Bitte probieren Sie es später erneut"));
                }

            }
            catch (DbException)
            {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message("Update", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

            }
            finally
            {
                await _rep.DisconnectAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdmin(RegistrationViewModel userDataFromForm)
        {

            ValidateUpdateData(userDataFromForm.User);

            if (ModelState.IsValid)
            {
                try
                {
                    await _rep.ConnectAsync();

                    if (await _rep.UpdateAsync(userDataFromForm.User))
                    {
                        return View("_Message", new Message("Update", "User '" + userDataFromForm.User.Username + "' wurde erfolgreich upgedated!"));
                    }
                    else
                    {
                        return View("_Message", new Message("Update", "Update fehlgeschlagen", "Bitte probieren Sie es später erneut"));
                    }

                }
                catch (DbException)
                {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message("Update", "Datenbank-Verbindungs-Fehler", "Bitte probieren Sie es später erneut"));

                }
                finally
                {
                    await _rep.DisconnectAsync();
                }
            }

            userDataFromForm.Countries = getCountriesEN();
            return View(userDataFromForm);
        }

        public IActionResult Newsletter() {
            return View();
        }

        // mögl in db abspeichern, bzw als json downloaden
        private List<String> getCountriesEN() {
            List<String> countries = new List<String> { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda", "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cabo Verde", "Cambodia", "Cameroon", "Canada", "Central African Republic (CAR)", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo, Democratic Republic of the", "Congo, Republic of the", "Costa Rica", "Cote d'Ivoire", "Croatia", "Cuba", "Cyprus", "Czechia", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kosovo", "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "North Macedonia", "Norway", "Oman", "Pakistan", "Palau", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda", "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States of Americ", "Uruguay", "Uzbekistan", "Vanuatu", "Vatican City", "Venezuela", "Vietnam", "Yemen", "Zambia", "Zimbabwe" };
            return countries;
        }

        private List<String> getCountriesDE() {
            List<String> countries = new List<String> { "Afghanistan", "Ägypten", "Albanien", "Algerien", "Andorra", "Angola", "Antigua und Barbuda", "Äquatorialguinea", "Argentinien", "Armenien", "Aserbaidschan", "Äthiopien", "Australien", "Bahamas", "Bahrain", "Bangladesch", "Barbados", "Belgien", "Belize", "Benin", "Bhutan", "Bolivien", "Bosnien und Herzegowina", "Botswana", "Brasilien", "Brunei", "Bulgarien", "Burkina Faso", "Burundi", "Cabo Verde", "Chile", "China", "Costa Rica", "Dänemark", "Deutschland", "Dominica", "Dominikanische Republik", "Dschibuti", "Ecuador", "El Salvador", "Elfenbeinküste", "Eritrea", "Estland", "Eswatini", "Fidschi", "Finnland", "Frankreich", "Gabun", "Gambia", "Georgien", "Ghana", "Grenada", "Griechenland", "Guatemala", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Honduras", "Indien", "Indonesien", "Irak", "Iran", "Irland", "Island", "Israel", "Italien", "Jamaika", "Japan", "Jemen", "Jordanien", "Kambodscha", "Kamerun", "Kanada", "Kasachstan", "Katar", "Kenia", "Kirgisistan", "Kiribati", "Kolumbien", "Komoren", "Kongo, Demokratische Republik", "Kongo, Republik", "Kosovo", "Kroatien", "Kuba", "Kuwait", "Laos", "Lesotho", "Lettland", "Libanon", "Liberia", "Libyen", "Liechtenstein", "Litauen", "Luxemburg", "Madagaskar", "Malawi", "Malaysia", "Malediven", "Mali", "Malta", "Marokko", "Marshallinseln", "Mauretanien", "Mauretanien", "Mexiko", "Mikronesien", "Moldawien", "Monaco", "Mongolei", "Montenegro", "Mosambik", "Myanmar", "Namibia", "Nauru", "Nepal", "neuseeland", "Nicaragua", "Niederlande", "Niger", "Nigeria", "Nordkorea", "Nord-Mazedonien", "Norwegen", "Oman", "Österreich", "Pakistan", "Palästina", "Palau", "Panama", "Papua-Neuguinea", "Paraguay", "Peru", "Philippinen", "Polen", "Portugal", "Ruanda", "Rumänien", "Russland", "Salomonen", "Sambia", "Samoa", "San Marino", "Sao Tome und Principe", "Saudi-Arabien", "Schweden", "Schweiz", "Senegal", "Serbien", "Seychellen", "Sierra Leone", "Simbabwe", "Singapur", "Slowakei", "Slowenien", "Somalia", "Spanien", "Sri Lanka", "St. Kitts und Nevis", "St. Lucia", "St. Vincent und die Grenadinen", "Süd-Afrika", "Sudan", "Südkorea", "Südsudan", "Surinam", "Syrien", "Tadschikistan", "Taiwan", "Tansania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad und Tobago", "Tschad", "Tschechische Republik", "Tunesien", "Türkei", "Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "Ungarn", "Uruguay", "Usbekistan", "Vanuatu", "Vatikanstadt", "Venezuela", "Vereinigte Arabische Emirate", "Vereinigte Staaten von Amerika", "Vereinigtes Königreich (UK)", "Vietnam", "Weißrussland", "Zentralafrikanische Republik", "Zypern" };
            return countries;
        }
        private void ValidateRegistrationData(User u) {

            if (u == null) {
                return;
            }

            if (u.Username == null || u.Username.Trim().Length < 4 || u.Username.Trim().Length > 100) {
                ModelState.AddModelError("User.Username", "Der Benutzername muss mind. 4  und max. 100 Zeichen lang sein.");
            } else if(u.Username.Contains('@'))
            {
                ModelState.AddModelError("User.Username", "Der Benutzername darf kein @ enthalten.");
            }

            if (u.Firstname == null || u.Firstname.Trim().Length < 4 || u.Firstname.Trim().Length > 100) {
                ModelState.AddModelError("User.Firstname", "Der Vorname muss mind. 4 und max. 100 Zeichen lang sein.");
            }

            if (u.Lastname == null || u.Lastname.Trim().Length < 4 || u.Lastname.Trim().Length > 100) {
                ModelState.AddModelError("User.Lastname", "Der Nachname muss mind. 4 und max. 100 Zeichen lang sein.");
            }

            if (u.Password == null || u.Password.Length < 8) {
                ModelState.AddModelError("User.Password", "Das Passwort muss mind. 8 Zeichen lang sein.");
            } else if(u.CPassword == null || !u.CPassword.Equals(u.Password)) {
                ModelState.AddModelError("User.CPassword", "Passwörter stimmen nicht überein.");
            }

            if (u.Birthdate > DateTime.Now.AddYears(-12)) {
                ModelState.AddModelError("User.Birthdate", "User müssen mindestens 12 Jahre alt sein.");
            }

            if (u.Email == null || !u.Email.Contains('@') || u.Email.Trim().Length < 8 || u.Email.Trim().Length > 100) {
                ModelState.AddModelError("User.Email", "Es muss eine gültige Email-Addresse eingegeben werden, welche länger als 7 und kürzer als 100 Zeichen ist.");
            }
        }

        private void ValidateLoginData(User u) {

            if (u == null)
                return;

            if (u.Username == null || u.Username.Trim().Length < 4 || u.Username.Trim().Length > 100)
                ModelState.AddModelError("Username", "Benutzername oder Email muss min. 4 und max. 100 Zeichen enthalten.");

            if (u.Password == null || u.Password.Length < 8)
                ModelState.AddModelError("Password", "Das Passwort muss mind. 8 Zeichen lang sein.");
        }

        private void ValidateUpdateData(User u) {

            if (u == null) {
                return;
            }

            if (u.Username == null || u.Username.Trim().Length < 4 || u.Username.Trim().Length > 100) {
                ModelState.AddModelError("User.Username", "Der Benutzername muss mind. 4  und max. 100 Zeichen lang sein.");
            }
            else if (u.Username.Contains('@')) {
                ModelState.AddModelError("User.Username", "Der Benutzername darf kein @ enthalten.");
            }

            if (u.Firstname == null || u.Firstname.Trim().Length < 4 || u.Firstname.Trim().Length > 100) {
                ModelState.AddModelError("User.Firstname", "Der Vorname muss mind. 4 und max. 100 Zeichen lang sein.");
            }

            if (u.Lastname == null || u.Lastname.Trim().Length < 4 || u.Lastname.Trim().Length > 100) {
                ModelState.AddModelError("User.Lastname", "Der Nachname muss mind. 4 und max. 100 Zeichen lang sein.");
            }

            if (u.Birthdate > DateTime.Now.AddYears(-12)) {
                ModelState.AddModelError("User.Birthdate", "User müssen mindestens 12 Jahre alt sein.");
            }

            if (u.Email == null || !u.Email.Contains('@') || u.Email.Trim().Length < 8 || u.Email.Trim().Length > 100) {
                ModelState.AddModelError("User.Email", "Es muss eine gültige Email-Addresse eingegeben werden, welche länger als 7 und kürzer als 100 Zeichen ist.");
            }
        }

        private void ValidatePasswordData(User u) {

            if (u == null) {
                return;
            }

            if (u.Password == null || u.Password.Length < 8) {
                ModelState.AddModelError("Password", "Das Passwort muss mind. 8 Zeichen lang sein.");
            }
            else if (u.CPassword == null || !u.CPassword.Equals(u.Password)) {
                ModelState.AddModelError("CPassword", "Passwörter stimmen nicht überein.");
            }
        }

        public async Task<bool> IsUniqueEmail(string email) {
            bool result = true;
            try {
                await _rep.ConnectAsync();
                result = await _rep.IsUniqueEmailAsync(email);
            }
            catch (DbException) {

            }
            finally {
                await _rep.DisconnectAsync();
            }

            return result;
        }

        public async Task<bool> IsUniqueUsername(string username) {
            bool result = true;
            try {
                await _rep.ConnectAsync();
                result = await _rep.IsUniqueUsernameAsync(username);
            }
            catch (DbException) {

            }
            finally {
                await _rep.DisconnectAsync();
            }

            return result;
        }
    }
}
