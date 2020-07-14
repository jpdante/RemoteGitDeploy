using System.Linq;
using System.Net.Mail;

namespace RemoteGitDeploy.Utils {
    public class DataValidation {
        public static bool ValidateEmail(string email) {
            try {
                _ = new MailAddress(email);
                return true;
            } catch {
                return false;
            }
        }

        public static bool ValidatePassword(string password, out string error) {
            if (password.Length < 8) {
                error = "The password must be at least 8 characters long.";
                return false;
            }
            if (!password.Any(char.IsLetter)) {
                error = "The password must have at least 1 letter.";
                return false;
            }
            if (!password.Any(char.IsDigit)) {
                error = "The password must have at least 1 digit.";
                return false;
            }
            if (!password.Any(char.IsPunctuation)) {
                error = "The password must have at least 1 punctuation.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateTeamName(string name, out string error) {
            if (name.Length < 3) {
                error = "The team name must be at least 3 characters long.";
                return false;
            }
            if (name.Any(char.IsWhiteSpace)) {
                error = "The team name can not have space.";
                return false;
            }
            error = null;
            return true;
        }
    }
}