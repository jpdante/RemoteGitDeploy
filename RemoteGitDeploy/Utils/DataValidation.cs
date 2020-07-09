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

        public static bool ValidatePassword(string password) {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsLetter)) return false;
            if (!password.Any(char.IsDigit)) return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (!password.Any(char.IsSymbol)) return false;
            return true;
        }
    }
}