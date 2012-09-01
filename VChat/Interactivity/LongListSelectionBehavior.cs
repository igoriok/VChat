using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;

namespace VChat.Interactivity
{
    public class LongListSelectionBehavior : Behavior<LongListSelector>
    {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(LongListSelectionBehavior), new PropertyMetadata(OnSelectedItemPropertyChanged));

        private bool _skip;

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LongListSelectionBehavior)d).OnSelectedItemChanged(e.NewValue);
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += LongListSelector_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= LongListSelector_SelectionChanged;
        }

        private void LongListSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _skip = true;
            SelectedItem = AssociatedObject.SelectedItem;
            _skip = false;
        }

        private void OnSelectedItemChanged(object obj)
        {
            if (!_skip && AssociatedObject != null)
            {
                AssociatedObject.SelectedItem = obj;
            }
        }
    }
}