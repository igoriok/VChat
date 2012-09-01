using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace VChat.Interactivity
{
    public class MirrorBehavior : Behavior<Control>
    {
        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register("CenterY", typeof(double), typeof(MirrorBehavior), new PropertyMetadata(default(double)));

        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        private readonly ScaleTransform _transform = new ScaleTransform();

        public MirrorBehavior()
        {
            _transform.ScaleX = 1;
            _transform.ScaleY = -1;

            BindingOperations.SetBinding(_transform, ScaleTransform.CenterYProperty, new Binding("CenterY") { Source = this });
        }

        protected override void OnAttached()
        {
            CenterY = AssociatedObject.ActualHeight / 2;

            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;

            AssociatedObject.RenderTransform = _transform;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RenderTransform = null;

            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;

            CenterY = default(double);
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterY = AssociatedObject.ActualHeight / 2;
        }
    }
}