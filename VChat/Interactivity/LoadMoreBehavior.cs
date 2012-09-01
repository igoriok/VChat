using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace VChat.Interactivity
{
    public class LoadMoreBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(LoadMoreBehavior), new PropertyMetadata(OnVerticalOffsetPropertyChanged));

        public static readonly DependencyProperty ScrollableHeightProperty =
            DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(LoadMoreBehavior), new PropertyMetadata(OnScrollableHeightPropertyChanged));

        public static readonly DependencyProperty LoadCommandProperty =
            DependencyProperty.Register("LoadCommand", typeof(ICommand), typeof(LoadMoreBehavior), new PropertyMetadata(default(ICommand)));

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadMoreBehavior)d).OnScroll();
        }

        private static void OnScrollableHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadMoreBehavior)d).OnScroll();
        }

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public double ScrollableHeight
        {
            get { return (double)GetValue(ScrollableHeightProperty); }
            set { SetValue(ScrollableHeightProperty, value); }
        }

        public ICommand LoadCommand
        {
            get { return (ICommand)GetValue(LoadCommandProperty); }
            set { SetValue(LoadCommandProperty, value); }
        }

        private ScrollViewer _scroll;

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += (s, e) =>
            {
                _scroll = FindChildOfType<ScrollViewer>(AssociatedObject);

                BindingOperations.SetBinding(this, VerticalOffsetProperty, new Binding("VerticalOffset") { Source = _scroll });
                BindingOperations.SetBinding(this, ScrollableHeightProperty, new Binding("ScrollableHeight") { Source = _scroll });
            };
        }

        protected override void OnDetaching()
        {
            ClearValue(VerticalOffsetProperty);
            ClearValue(ScrollableHeightProperty);

            _scroll = null;
        }

        private void OnScroll()
        {
            if (VerticalOffset > ScrollableHeight * 0.80)
            {
                var command = LoadCommand;
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }

        private static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();

            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                for (var index = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= index; index--)
                {
                    var child = VisualTreeHelper.GetChild(current, index);

                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }

                    queue.Enqueue(child);
                }
            }

            return null;
        }
    }
}