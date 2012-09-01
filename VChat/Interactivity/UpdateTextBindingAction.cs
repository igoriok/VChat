using System.Windows.Controls;
using System.Windows.Interactivity;

namespace VChat.Interactivity
{
    public class UpdateTextBindingAction : TriggerAction<TextBox>
    {
        #region TriggerAction

        protected override void Invoke(object parameter)
        {
            var expression = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (expression != null)
            {
                expression.UpdateSource();
            }
        }

        #endregion
    }
}