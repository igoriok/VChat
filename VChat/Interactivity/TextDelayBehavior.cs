using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace VChat.Interactivity
{
    public class TextDelayBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextDelayBehavior), new PropertyMetadata(OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextDelayBehavior)d).OnPropertyChanged(e.NewValue as string);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private readonly DispatcherTimer _timer;

        private bool _skipUpdate;

        public TextDelayBehavior()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
        }

        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += TextBox_TextChanged;
        }

        protected override void OnDetaching()
        {
            _timer.Stop();

            AssociatedObject.TextChanged -= TextBox_TextChanged;
        }

        private void OnPropertyChanged(string newValue)
        {
            if (AssociatedObject != null && !_skipUpdate)
            {
                _skipUpdate = true;
                AssociatedObject.Text = newValue ?? string.Empty;
                _skipUpdate = false;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _timer.Stop();

            if (!_skipUpdate)
            {
                _timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs eventArgs)
        {
            _timer.Stop();

            _skipUpdate = true;
            Text = AssociatedObject.Text;
            _skipUpdate = false;
        }
    }
}