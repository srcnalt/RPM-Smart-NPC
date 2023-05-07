using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace OpenAI
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private ChatTest chatTest; 
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 5;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();

        private void Start()
        {
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private async void StartRecording()
        {
            if (isRecording)
            {
                Debug.Log("Stop recording...");
                recordButton.GetComponent<Image>().color = Color.red;
                    
                Microphone.End(null);
                byte[] data = SaveWav.Save(fileName, clip);
            
                var req = new CreateAudioTranscriptionsRequest
                {
                    FileData = new FileData() {Data = data, Name = "audio.wav"},
                    // File = Application.persistentDataPath + "/" + fileName,
                    Model = "whisper-1",
                    Language = "en"
                };
                var res = await openai.CreateAudioTranscription(req);

                chatTest.SendReply(res.Text);
                isRecording = false;
            }
            else
            {
                Debug.Log("Start recording...");
                isRecording = true;
                recordButton.GetComponent<Image>().color = Color.green;
    
                var index = PlayerPrefs.GetInt("user-mic-device-index");
                clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            }
        }
    }
}
