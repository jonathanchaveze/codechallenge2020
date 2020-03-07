using System.Security.Cryptography;
using System.Text;

namespace Jobsity.CodeChallenge.WebApp.Services
{
    public class ProfilePictureService
    {
        public string GetProfilePictureByEmail(string email)
        {
            var pictureFile = GetMd5String(email);

            return $"https://www.gravatar.com/avatar/{pictureFile}";
        }

        private string GetMd5String(string input)
        {
            var data = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(input));
            var sBuilder = new StringBuilder();

            foreach (var b in data)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}