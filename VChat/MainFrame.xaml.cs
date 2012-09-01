using System.Globalization;
using System.Windows.Markup;
using Microsoft.Phone.Controls;

namespace VChat
{
    public partial class MainFrame : TransitionFrame
    {
        public MainFrame()
        {
            InitializeComponent();

            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);
        }
    }
}