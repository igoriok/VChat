using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace VChat.Services.Vkontakte.Data
{
    [DataContract]
    public class LongPollHistory
    {
        [DataMember(Name = "history")]
        public JArray[] History { get; set; }

        [DataMember(Name = "messages")]
        public VkMessage[] Messages { get; set; }

        public static LongPollHistory Parse(JToken response)
        {
            var history = response["history"] as JArray;
            var messages = response["messages"] as JArray;

            return new LongPollHistory
            {
                Messages = GetMessages(messages)
            };
        }

        private static VkMessage[] GetMessages(JArray messages)
        {
            var items = new List<VkMessage>();

            foreach (var item in messages.Skip(1))
            {
                try
                {
                    items.Add(item.ToObject<VkMessage>());
                }
                catch { }
            }

            return items.ToArray();
        }
    }
}