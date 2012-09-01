using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Reactive;

using VChat.Services.Vkontakte;

namespace VChat.ViewModels
{
    public static class ExceptionHelper
    {
        public static IObservable<T> Attach<T>(IObservable<T> source)
        {
            return source.Catch<T, Exception>(Handle<T>);
        }

        private static IObservable<T> Handle<T>(Exception exception)
        {
            return Observable.Throw<T>(exception);
        }

        public static void HandleException(Exception exception)
        {
            var vkException = exception as VkException;
            var webException = exception as WebException;

            if (vkException != null)
            {
                switch (vkException.Code)
                {
                    // Unknown error occurred
                    case 1:

                    // Application is disabled. Enable your application or use test mode
                    case 2:

                    // Incorrect signature
                    case 4:

                    // User authorization failed
                    case 5:

                    // Too many requests per second
                    case 6:

                    // Permission to perform this action is denied by user
                    case 7:

                    // Flood control enabled for this action
                    case 9:

                    // Internal server error
                    case 10:

                    // 	Captcha is needed
                    case 14:

                    // Access denied {you have no messages from this user}
                    case 15:

                    // Permission to perform this action is denied for non-standalone applications
                    case 20:

                    // One of the parameters specified was missing or invalid
                    case 100:

                    // Invalid user ids
                    case 113:

                    // 	Invalid server
                    case 118:

                    // Invalid hash
                    case 121:

                    // Invalid photos list
                    case 122:

                    // Invalid group id
                    case 125:

                    // Cannot add yourself to friends
                    case 174:

                    // Cannot add this user to friends as they have put you on their blacklist
                    case 175:

                    // Cannot add this user to list of friends due to privacy settings
                    case 176:

                    // This album is full
                    case 300:

                    // User already invited: message already sended, you can resend message in 300 seconds
                    case 1003:

                    // This phone used by another user
                    case 1004:

                    // Processing.. Try later
                    case 1112:

                        MessageBox.Show(vkException.Message);
                        break;

                    default:

                        MessageBox.Show(vkException.Message);
                        break;
                }
            }
            else if (webException != null)
            {
                MessageBox.Show(webException.Message);
            }
            else
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}