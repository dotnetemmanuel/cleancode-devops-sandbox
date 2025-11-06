namespace DataValidationLib;

public class Validator
{
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>?/".Contains(ch))) return false;

        return true;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsPalindrome(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var cleaned = new string(input
            .Where(char.IsLetterOrDigit) // vi tar bort allt annat än bokstäver och siffror (blanksteg, interpunktion, etc.)
            .Select(char.ToLower) // vi tvingar samtliga tecken till sin gemenversion
            .ToArray()); // resulatet av filtreringen blir en char[] som "new string()" bygger om till en sträng.

        var reversed = new string(cleaned.Reverse().ToArray()); //Reverse
        return cleaned == reversed;
    }
}