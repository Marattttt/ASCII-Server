using System.Text.RegularExpressions;

namespace api.Services;

public static class UserDataChecker {
    public static bool isValidPassword(string password) {
        //At least 8 characters long, at least one digit and one letter, all ASCII
        //"password1" - valid
        //"пс врд" - invalid
        string regEx = @"/^(?!.*\s)(?=.*\d)(?=.*[a-z])(?=.*[a-zA-Z]).{8,}$/gm";
        int matches = Regex.Matches(password, regEx).Count();
        return matches == 1;
    }
}