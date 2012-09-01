using System;
using System.Collections.Generic;
using Microsoft.Phone.Reactive;
using Microsoft.Xna.Framework.GamerServices;

namespace VChat.Mvvm
{
    public static class MessageBox
    {
        public static IObservable<int?> Show(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            var async = new MessageBoxAsync(title, text, buttons, focusButton, icon);

            return Observable.FromAsyncPattern<int?>(async.BeginShow, async.EndShow)();
        }

        private class MessageBoxAsync
        {
            private readonly string _title;
            private readonly string _text;
            private readonly IEnumerable<string> _buttons;
            private readonly int _focusButton;
            private readonly MessageBoxIcon _icon;

            public MessageBoxAsync(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon)
            {
                _title = title;
                _text = text;
                _buttons = buttons;
                _focusButton = focusButton;
                _icon = icon;
            }

            public IAsyncResult BeginShow(AsyncCallback callback, object state)
            {
                return Guide.BeginShowMessageBox(_title, _text, _buttons, _focusButton, _icon, callback, state);
            }

            public int? EndShow(IAsyncResult result)
            {
                return Guide.EndShowMessageBox(result);
            }

        }
    }
}