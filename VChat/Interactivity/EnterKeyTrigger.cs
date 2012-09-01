using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VChat.Interactivity
{
    public class EnterKeyTrigger : TriggerBase<UIElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InvokeActions(e);
            }
        }
    }
}