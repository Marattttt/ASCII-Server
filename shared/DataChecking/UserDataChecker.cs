using System.Text.RegularExpressions;

using shared.DTOs;

namespace shared.DataChecking;

public static class UserDataChecker {
    public static bool isValidPassword(string password) {
        //At least 8 characters long, at least one digit and one letter, all ASCII
        //"password1" - valid
        //"пс врд" - invalid
        string pattern = @"^(?!.*\s)(?=.*\d)(?=.*[a-z])(?=.*[a-zA-Z]).{8,}\S$";
        var check = new Regex(pattern);

        bool result = check.IsMatch(password);
        return result;
    }
    
    public static bool isValidUserName(string userName) {
        //Only letters of english alphabet and numbers without spaces
        //Length from 5 to 20
        //user_name36 - valid
        //юзер наме - invalid
        string pattern = @"^(?=.*[a-zA-Z]).{5,}\S$";
        var check = new Regex(pattern);

        bool result = check.IsMatch(userName);
        return result;
    }

    public static string CheckFullUserDto(FullUserInfoDTO dto) {
        string invalidDataErrorMessage = String.Empty;
        if (!isValidPassword(dto.Password)) {
            invalidDataErrorMessage += "invalid password; ";
        }
        if (!isValidUserName(dto.UserName)) {
            invalidDataErrorMessage += "invalid user name; ";
        }

        if (invalidDataErrorMessage != String.Empty) {
            invalidDataErrorMessage += "user id = " + dto.UserId;
        }
        return invalidDataErrorMessage;
    }
}