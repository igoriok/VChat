using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
using Microsoft.Phone.Controls;

using ApplicationBar = BindableApplicationBar.BindableApplicationBar;

namespace VChat.Interactivity
{
    [ContentProperty("Items")]
    public class ApplicationBarBehavior : Behavior<Pivot>
    {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList), typeof(ApplicationBarBehavior), new PropertyMetadata(null));

        public IList Items
        {
            get { return (IList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public ApplicationBarBehavior()
        {
            Items = new List<ApplicationBar>();
        }

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += Pivot_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= Pivot_SelectionChanged;
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateApplicationBar();
        }

        private void UpdateApplicationBar()
        {
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null && frame.Content is PhoneApplicationPage)
            {
                var applicationBar = GetApplicationBar(Items, AssociatedObject.SelectedIndex);

                BindableApplicationBar.Bindable.SetApplicationBar((DependencyObject)frame.Content, applicationBar);
            }
        }

        private ApplicationBar GetApplicationBar(IList list, int index)
        {
            if (list != null && index >= 0 && list.Count > index)
            {
                return list[index] as ApplicationBar;
            }

            return null;
        }
    }
}