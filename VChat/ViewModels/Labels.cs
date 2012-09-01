using System.Globalization;
using Caliburn.Micro;
using VChat.Properties;

namespace VChat.ViewModels
{
    public class Labels : PropertyChangedBase
    {
        private static Labels _instance;

        public static Labels Instance
        {
            get { return _instance; }
        }

        public CultureInfo Culture
        {
            get { return Resources.Culture; }
            set
            {
                Resources.Culture = value;
                Refresh();
            }
        }

        public string Username
        {
            get { return Resources.Username; }
        }

        public string Password
        {
            get { return Resources.Password; }
        }

        public string SignIn
        {
            get { return Resources.SignIn; }
        }

        public string SignUp
        {
            get { return Resources.SignUp; }
        }

        public string SignUpDescription
        {
            get { return Resources.SignUpDescription; }
        }

        public string ConfirmationSend
        {
            get { return Resources.ConfirmationSend; }
        }

        public string Code
        {
            get { return Resources.Code; }
        }

        public string Confirm
        {
            get { return Resources.Confirm; }
        }

        public string Authenticating
        {
            get { return Resources.Authenticating; }
        }

        public string YourPhoneNumber
        {
            get { return Resources.YourPhoneNumber; }
        }

        public string YourFirstName
        {
            get { return Resources.YourFirstName; }
        }

        public string YourLastName
        {
            get { return Resources.YourLastName; }
        }

        public Labels()
        {
            _instance = this;
        }
    }
}