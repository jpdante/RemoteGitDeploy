using System;
using System.Linq;
using System.Net.Mail;
using HtcSharp.Core.Logging.Abstractions;

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

        public static bool ValidateUsername(string name, out string error) {
            if (name.Length < 3) {
                error = "The username must be at least 3 characters long.";
                return false;
            }
            if (name.Any(char.IsWhiteSpace)) {
                error = "The username can not have space.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateName(string name, out string error) {
            if (name.Length < 1) {
                error = "The name must be at least 1 character long.";
                return false;
            }
            if (name.Any(char.IsDigit)) {
                error = "The name can not have any digit.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateRepositoryName(string name, out string error) {
            if (name.Length < 3) {
                error = "The repository name must be at least 3 character long.";
                return false;
            }
            if (name.Any(char.IsWhiteSpace)) {
                error = "The repository name can not have space.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateGitLink(string git, out string error) {
            if (git.Length < 3) {
                error = "The git link must be at least 3 character long.";
                return false;
            }
            if (git.Any(char.IsWhiteSpace)) {
                error = "The git link name can not have space.";
                return false;
            }
            if (!Uri.TryCreate(git, UriKind.RelativeOrAbsolute, out var gitUri)) {
                error = "The git link must be a valid URL.";
                return false;
            }
            if (!gitUri.Scheme.Equals("http") && !gitUri.Scheme.Equals("https")) {
                error = "The git link must use the http or https protocol.";
                return false;
            }
            if (!gitUri.AbsolutePath.EndsWith(".git")) {
                error = "The git link must be a valid git URL.";
                return false;
            }
            error = null;
            return true;
        }
    }
}