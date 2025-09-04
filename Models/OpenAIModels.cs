using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OpenAIModels
    {
        public class OpenAIChatRequest
        {
            public string model { get; set; }
            public List<OpenAIMessage> messages { get; set; }
        }

        public class OpenAIMessage
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        public class OpenAIChatResponse
        {
            public List<OpenAIChoice> choices { get; set; }
        }

        public class OpenAIChoice
        {
            public int index { get; set; }
            public OpenAIMessage message { get; set; }
        }
    }
}
