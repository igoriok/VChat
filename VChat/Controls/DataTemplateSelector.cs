using System.Windows;
using System.Windows.Controls;

namespace VChat.Controls
{
    public class DataTemplateSelector : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null)
            {
                var name = newContent.GetType().Name;
                ContentTemplate = Resources[name] as DataTemplate;
            }
        }
    }
}