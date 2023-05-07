using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private GameObject lastSelected;
        
        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as a random stranger in a chat room and reply to the questions. " +
                                "Don't break character. Don't ever mention that you are an AI model. " +
                                "Reply in less than 30 words.";

        private RectTransform messageRect;
        
        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }
        
        private RectTransform AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            
            if (message.Role != "user")
            {
                messageRect = item;
            }
            
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);

            if (message.Role == "user")
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(item);
                height += item.sizeDelta.y;
                scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                scroll.verticalNormalizedPosition = 0;
            }

            return item;
        }

        private void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            //button.enabled = false;
            inputField.text = "";
            //inputField.enabled = false;
            
            foreach (var message in messages)
            {
                Debug.Log(message.Content);
            }
            
            // Complete the instruction
            openai.CreateChatCompletionAsync(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = messages
            }, OnResponse, OnComplete, new CancellationTokenSource());
        }

        private void OnComplete()
        {
            Debug.Log("Complete");
            /*
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);
            height += messageRect.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
            
            messageRect = null;
            */
            
            
            button.enabled = true;
            inputField.enabled = true;
        }

        private void OnResponse(List<CreateChatCompletionResponse> responses)
        {
            var text = string.Join("", responses.Select(r => r.Choices[0].Delta.Content));

            if (text == "") return;
            Debug.Log(text);

            var message = new ChatMessage()
            {
                Content = text.Trim()
            };
            
            if (messageRect == null)
            {
                messages.Add(message);
                messageRect = AppendMessage(message);
            }
            
            messageRect.GetChild(0).GetChild(0).GetComponent<Text>().text = text;
        }
    }
}
