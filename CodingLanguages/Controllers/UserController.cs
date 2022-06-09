using CodingLanguages.Models;
using CodingLanguages.Models.DB;
using CodingLanguages.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

// newsletter

namespace CodingLanguages.Controllers {
    public class UserController : Controller {

        private IRepositoryUsers _rep = new RepositoryUserDB();
        private readonly IStringLocalizer<UserController> _stringLocalizer;
        public UserController(IStringLocalizer<UserController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Register() {
            RegistrationViewModel rvm = new RegistrationViewModel();
            rvm.User = new Models.User();
            rvm.User.Birthdate = DateTime.Now;
            rvm.Countries = getCountries();
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
                        return View("_Message", new Message(_stringLocalizer["reg"].Value, _stringLocalizer["hello"].Value + userDataFromForm.User.Username + "!\n" + _stringLocalizer["reg.s"].Value));
                    }
                    else {
                        return View("_Message", new Message(_stringLocalizer["reg"].Value, "Registrierung fehlgeschlagen", _stringLocalizer["later"].Value));
                    }

                }
                catch (DbException) {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message(_stringLocalizer["reg"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

                }
                finally {
                    await _rep.DisconnectAsync();
                }
            }

            userDataFromForm.Countries = getCountries();
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
                        return View("_Message", new Message("Login", _stringLocalizer["hello"].Value + wholeUser.Username + "! " + _stringLocalizer["lin.s"].Value));
                    }
                    else
                    {
                        return View("_Message", new Message("Login", _stringLocalizer["lin.f"].Value, _stringLocalizer["lin.h"].Value));
                    }

                }
                catch (DbException)
                {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message("Login", _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

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
            return View("_Message", new Message("Logout", _stringLocalizer["lout"].Value));
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
                    return View("_Message", new Message(_stringLocalizer["profil"].Value, _stringLocalizer["db.laden"].Value, _stringLocalizer["later"].Value));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message(_stringLocalizer["profil"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

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
                    rvm.Countries = getCountries();
                    user.UsernameOld = user.Username;
                    user.EmailOld = user.Email;
                    rvm.User = user;
                    return View(rvm);
                }
                else {
                    return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.laden"].Value, _stringLocalizer["later"].Value));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

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
                    ModelState.AddModelError("User.Password", _stringLocalizer["pwd.false"].Value);
                }
            } catch(DbException) {
                return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));
            } finally {
                await _rep.DisconnectAsync();
            }

            if (ModelState.IsValid) {
                try {
                    await _rep.ConnectAsync();

                    if (await _rep.UpdateAsync(userDataFromForm.User)) {
                        HttpContext.Session.SetString("name", userDataFromForm.User.Username);
                        return View("_Message", new Message(_stringLocalizer["chpf"].Value, "User '" + userDataFromForm.User.Username + "' " + _stringLocalizer["update.succ"].Value));
                    }
                    else {
                        return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["update.fail"].Value, _stringLocalizer["later"].Value));
                    }

                }
                catch (DbException) {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

                }
                finally {
                    await _rep.DisconnectAsync();
                }
            }

            userDataFromForm.Countries = getCountries();
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
                    ModelState.AddModelError("PasswordOld", _stringLocalizer["pwd.false"].Value);
                }
            }
            catch (DbException) {
                return View("_Message", new Message(_stringLocalizer["chpwd"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));
            }
            finally {
                await _rep.DisconnectAsync();
            }

            if (ModelState.IsValid) {
                try {
                    await _rep.ConnectAsync();

                    if (await _rep.SetPasswordAsync(HttpContext.Session.GetString("name"), userDataFromForm.Password)) {
                        return View("_Message", new Message(_stringLocalizer["chpwd"].Value, _stringLocalizer["chpwd.succ"].Value));
                    }
                    else {
                        return View("_Message", new Message(_stringLocalizer["chpwd"].Value, _stringLocalizer["chpwd.fail"].Value, _stringLocalizer["later"].Value));
                    }

                }
                catch (DbException) {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message(_stringLocalizer["chpwd"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

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
                    return View("_Message", new Message(_stringLocalizer["del"].Value, "User '" + username + "'  " + _stringLocalizer["del.succ"].Value));
                }
                else {
                    return View("_Message", new Message(_stringLocalizer["del"].Value, _stringLocalizer["del.fail"].Value, _stringLocalizer["later"].Value));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message(_stringLocalizer["del"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

            }
            finally {
                await _rep.DisconnectAsync();
            }
        }

        public async Task<IActionResult> AdminArea() {
            if (HttpContext.Session.GetString("name") == "" || HttpContext.Session.GetString("name") == null)
                return RedirectToAction("Login");

            if (HttpContext.Session.GetInt32("admin") != 1 )
                return View("_Message", new Message(_stringLocalizer["adm"].Value, _stringLocalizer["adm.auth"].Value, _stringLocalizer["adm.auth2"].Value));

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
                    return View("_Message", new Message(_stringLocalizer["adm"].Value, _stringLocalizer["adm.data"].Value, _stringLocalizer["later"].Value));
                }

            }
            catch (DbException)
            {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message(_stringLocalizer["adm"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

            }
            finally
            {
                await _rep.DisconnectAsync();
            }
        }

        public async Task<IActionResult> DeleteAdmin(string id)
        {
            if(HttpContext.Session.GetInt32("admin") != 1)
                return View("_Message", new Message(_stringLocalizer["adm"].Value, _stringLocalizer["adm.auth"].Value, _stringLocalizer["adm.auth2"].Value));

            if (HttpContext.Session.GetString("name") == id)
                return RedirectToAction("Delete");

            try {
                await _rep.ConnectAsync();

                if (await _rep.DeleteAsync(id)) {
                    return View("_Message", new Message(_stringLocalizer["del"].Value, "User '" + id + "' " + _stringLocalizer["del.succ"].Value));
                }
                else {
                    return View("_Message", new Message(_stringLocalizer["del"].Value, _stringLocalizer["del.fail"].Value, _stringLocalizer["later"].Value));
                }

            }
            catch (DbException) {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message(_stringLocalizer["del"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

            }
            finally {
                await _rep.DisconnectAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAdmin(string id)
        {
            if (HttpContext.Session.GetInt32("admin") != 1)
                return View("_Message", new Message(_stringLocalizer["adm"].Value, _stringLocalizer["adm.auth"].Value, _stringLocalizer["adm.auth2"].Value));

            try
            {
                await _rep.ConnectAsync();
                User user = await _rep.GetUserAsync(id);

                if (user != null)
                {
                    RegistrationViewModel rvm = new RegistrationViewModel();
                    rvm.Countries = getCountries();
                    user.UsernameOld = user.Username;
                    user.EmailOld = user.Email;
                    rvm.User = user;
                    return View(rvm);
                }
                else
                {
                    return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.laden"].Value, _stringLocalizer["later"].Value));
                }

            }
            catch (DbException)
            {          // basisklasse datenbank-Exceptions
                return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

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
                        return View("_Message", new Message(_stringLocalizer["chpf"].Value, "User '" + userDataFromForm.User.Username + "' " + _stringLocalizer["update.succ"].Value));
                    }
                    else
                    {
                        return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["update.fail"].Value, _stringLocalizer["later"].Value));
                    }

                }
                catch (DbException)
                {          // basisklasse datenbank-Exceptions
                    return View("_Message", new Message(_stringLocalizer["chpf"].Value, _stringLocalizer["db.verb"].Value, _stringLocalizer["later"].Value));

                }
                finally
                {
                    await _rep.DisconnectAsync();
                }
            }

            userDataFromForm.Countries = getCountries();
            return View(userDataFromForm);
        }

        public IActionResult Newsletter() {
            return View();
        }

        private List<String> getCountries()
        {
            if (HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name == "de-DE")
                return getCountriesDE();
            return getCountriesEN();
        }

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
                ModelState.AddModelError("User.Username", _stringLocalizer["un.v"].Value);
            } else if(u.Username.Contains('@'))
            {
                ModelState.AddModelError("User.Username", _stringLocalizer["un.v2"].Value);
            }

            if (u.Firstname == null || u.Firstname.Trim().Length < 4 || u.Firstname.Trim().Length > 100) {
                ModelState.AddModelError("User.Firstname", _stringLocalizer["fn.v"].Value);
            }

            if (u.Lastname == null || u.Lastname.Trim().Length < 4 || u.Lastname.Trim().Length > 100) {
                ModelState.AddModelError("User.Lastname", _stringLocalizer["ln.v"].Value);
            }

            if (u.Password == null || u.Password.Length < 8) {
                ModelState.AddModelError("User.Password", _stringLocalizer["pwd.v"].Value);
            } else if(u.CPassword == null || !u.CPassword.Equals(u.Password)) {
                ModelState.AddModelError("User.CPassword", _stringLocalizer["cpwd.v"].Value);
            }

            if (u.Birthdate > DateTime.Now.AddYears(-12)) {
                ModelState.AddModelError("User.Birthdate", _stringLocalizer["bd.v"].Value);
            }

            if (u.Email == null || !u.Email.Contains('@') || u.Email.Trim().Length < 8 || u.Email.Trim().Length > 100) {
                ModelState.AddModelError("User.Email", _stringLocalizer["email.v"].Value);
            }
        }

        private void ValidateLoginData(User u) {

            if (u == null)
                return;

            if (u.Username == null || u.Username.Trim().Length < 4 || u.Username.Trim().Length > 100)
                ModelState.AddModelError("Username", _stringLocalizer["login.v"].Value);

            if (u.Password == null || u.Password.Length < 8)
                ModelState.AddModelError("Password", _stringLocalizer["pwd.v"].Value);
        }

        private void ValidateUpdateData(User u) {

            if (u == null) {
                return;
            }

            if (u.Username == null || u.Username.Trim().Length < 4 || u.Username.Trim().Length > 100) {
                ModelState.AddModelError("User.Username", _stringLocalizer["un.v"].Value);
            }
            else if (u.Username.Contains('@')) {
                ModelState.AddModelError("User.Username", _stringLocalizer["un.v2"].Value);
            }

            if (u.Firstname == null || u.Firstname.Trim().Length < 4 || u.Firstname.Trim().Length > 100) {
                ModelState.AddModelError("User.Firstname", _stringLocalizer["fn.v"].Value);
            }

            if (u.Lastname == null || u.Lastname.Trim().Length < 4 || u.Lastname.Trim().Length > 100) {
                ModelState.AddModelError("User.Lastname", _stringLocalizer["ln.v"].Value);
            }

            if (u.Birthdate > DateTime.Now.AddYears(-12)) {
                ModelState.AddModelError("User.Birthdate", _stringLocalizer["bd.v"].Value);
            }

            if (u.Email == null || !u.Email.Contains('@') || u.Email.Trim().Length < 8 || u.Email.Trim().Length > 100) {
                ModelState.AddModelError("User.Email", _stringLocalizer["email.v"].Value);
            }
        }

        private void ValidatePasswordData(User u) {

            if (u == null) {
                return;
            }

            if (u.Password == null || u.Password.Length < 8) {
                ModelState.AddModelError("Password", _stringLocalizer["pwd.v"].Value);
            }
            else if (u.CPassword == null || !u.CPassword.Equals(u.Password)) {
                ModelState.AddModelError("CPassword", _stringLocalizer["cpwd.v"].Value);
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
