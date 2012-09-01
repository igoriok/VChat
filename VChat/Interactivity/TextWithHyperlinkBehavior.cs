using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VChat.Interactivity
{
    public class TextWithHyperlinkBehavior : Behavior<RichTextBox>
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextWithHyperlinkBehavior), new PropertyMetadata(OnTextPropertyChanged));

        public static readonly DependencyProperty NavigateCommandProperty =
            DependencyProperty.Register("NavigateCommand", typeof(ICommand), typeof(TextWithHyperlinkBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand NavigateCommand
        {
            get { return (ICommand)GetValue(NavigateCommandProperty); }
            set { SetValue(NavigateCommandProperty, value); }
        }


        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextWithHyperlinkBehavior)d).UpdateInlines();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnAttached()
        {
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Blocks.Clear();
        }

        private void UpdateInlines()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Blocks.Clear();

                var paragraph = new Paragraph();

                foreach (var text in Text.Split(' '))
                {
                    Uri uri;

                    if (Uri.TryCreate(text, UriKind.Absolute, out uri) ||
                        (text.StartsWith("www.") && Uri.TryCreate("http://" + text, UriKind.Relative, out uri)))
                    {
                        var hyperlink = new Hyperlink { NavigateUri = uri };

                        hyperlink.Inlines.Add(text);

                        BindingOperations.SetBinding(hyperlink, Hyperlink.CommandProperty, new Binding("NavigateCommand") { Source = this });

                        paragraph.Inlines.Add(hyperlink);
                    }
                    else
                    {
                       paragraph.Inlines.Add(text);
                    }
                }

                AssociatedObject.Blocks.Add(paragraph);
            }
        }
    }
}