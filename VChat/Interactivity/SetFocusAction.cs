using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace VChat.Interactivity
{
    public class SetFocusAction : TargetedTriggerAction<Control>
    {
        public bool Negatiate { get; set; }

        #region TriggerAction

        protected override void Invoke(object parameter)
        {
            if (Negatiate)
            {
                ((Control)Application.Current.RootVisual).Focus();
            }
            else
            {
                Target.Focus();
            }
        }

        #endregion
    }
}