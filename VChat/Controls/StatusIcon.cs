using System.Windows;
using System.Windows.Controls;

namespace VChat.Controls
{
    [TemplateVisualState(Name = "Empty", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Visible", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Collapsed", GroupName = "CommonStates")]
    public class StatusIcon : ContentControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StatusIcon), new PropertyMetadata(null, OnTextChanged));

        public static readonly DependencyProperty ShowWhenEmptyProperty =
            DependencyProperty.Register("ShowWhenEmpty", typeof(bool), typeof(StatusIcon), new PropertyMetadata(false, OnShowWhenEmptyChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StatusIcon)d).UpdateVisualState();
        }

        private static void OnShowWhenEmptyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StatusIcon)d).UpdateVisualState();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool ShowWhenEmpty
        {
            get { return (bool)GetValue(ShowWhenEmptyProperty); }
            set { SetValue(ShowWhenEmptyProperty, value); }
        }

        public StatusIcon()
        {
            DefaultStyleKey = typeof(StatusIcon);
        }

        public override void OnApplyTemplate()
        {
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (string.IsNullOrEmpty(Text))
            {
                VisualStateManager.GoToState(this, ShowWhenEmpty ? "Empty" : "Collapsed", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Visible", false);
            }
        }
    }
}