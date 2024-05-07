using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using static UnityEngine.Rendering.DebugUI;
using TMPro;
using SpeechLib;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
public class Gemini : MonoBehaviour
{
    public Material skyboxMaterial; // Assign this in the inspector
    public Texture2D[] newSkyboxTextures; // Assign this in the inspector
    SpVoice voice = new SpVoice();
    private List<string> options = new List<string> { "The Milky Way (Earth)", "The Great Pyramids of Giza (Egypt)", "The Eiffel Tower (France)", "The Great Wall (China)", "The Taj Mahal (India)", "Christ the Redeemer (Brazil)", "The Colliseum (Italy)", "Machu Picchu (Peru)", "Tower Bridge, London (UK)", "Historic City of Ayutthaya (Thailand)", "The Wat Phra That Doi Suthep (Thailand)", "St James's Park, London (UK)" };

    private DictationRecognizer dictationRecognizer;

    public Dropdown dropdown;
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public VideoClip[] videoClips; // Array to hold your video clips
    private int clipIndex = 0;
    public Text myText; // Assign this via the Unity Inspector
    private OpenAIApi openAI = new OpenAIApi("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", "org-Fh44kHCuPEY3G0qYRQswAAqy");
    private List<ChatMessage> messages = new List<ChatMessage>();
    public TMP_InputField inputField; // Assign in the inspector
    public Text startButtonText;
    public Text askGPTButtonText;
    public List<AudioClip> audioClips; // Assign this list in the Inspector with your audio clips
    public AudioSource audioSource;

    private int currentClipIndex = 0;

    // custom greetings
    private List<string> greetings = new List<string>
    {
        "Greetings, speck of cosmic dust. I am the Milky Way, ancient beyond measure at over 13 billion years old and spanning 100,000 light years. Ready to star-gaze and galaxy-hop with me?",
        "Marhaba, curious explorer! I am the Great Pyramids of Giza, a testament to eternity at over 4,500 years old, towering originally at 146.6 meters. Fancy uncovering the pharaohs' secrets with me? \n\nThank you for saving 2.57 Metric Tons of CO2 by visiting me virtually!",
        "Bonjour, mon ami voyageur! I'm the Eiffel Tower, Paris' iron heart for 134 years at 300 meters. Ready for some high-altitude romance and perhaps a nearby baguette? \n\nThank you for saving 1.68 Metric Tons of CO2 by visiting me virtually!",
        "Nǐ hǎo, adventurous soul! I am the Great Wall, China's dragon stretching over 21,196 km for more than 2,300 years. Dare to traverse my spine and hear the whispers of history? \n\nThank you for saving 2.96 Metric Tons of CO2 by visiting me virtually!",
        "Namaste, intrepid visitor. I am the Taj Mahal, symbolizing immortal love for over 370 years and rising majestically to 73 meters. Shall we reflect on love and marvels beyond time? \n\nThank you for saving 3.83 Metric Tons of CO2 by visiting me virtually!",
        "Olá, esteemed guest. I am Christ the Redeemer, embracing Rio from 30 meters above, with open arms for 90 years. What viewpoints and blessings are you seeking under my gaze? \n\nThank you for saving 2.38 Metric Tons of CO2 by visiting me virtually!",
        "Salve, noble traveler. I am the Colosseum, where echoes of ancient Rome reverberate within my 48-meter high walls for nearly 2,000 years. Ready for a history-packed showdown? \n\nThank you for saving 1.98 Metric Tons of CO2 by visiting me virtually!",
        "Hola, seeker of mysteries. I am Machu Picchu, Peru's hidden jewel sitting 2,430 meters above sea level for over 500 years. What ancient secrets shall we unravel together? \n\nThank you for saving 1.79 Metric Tons of CO2 by visiting me virtually!",
        "Cheerio, mate! I am Tower Bridge, London's pride spanning the Thames at 244 meters for 127 years. Care to delve into tales of engineering feats and perhaps spot a lifting moment? \n\nThank you for saving 1.59 Metric Tons of CO2 by visiting me virtually!",
        "Sawasdee, wandering spirit. I am the Historic City of Ayutthaya, bearing witness to the Siam Empire's glory for over 600 years. Ready to journey through time and uncover my stories? \n\nThank you for saving 3.80 Metric Tons of CO2 by visiting me virtually!",
        "Greetings, seeker of enlightenment. I am Wat Phra That Doi Suthep, standing serene and golden for nearly 700 years. What wisdom and tranquility do you wish to find on my mountaintop? \n\nThank you for saving 3.80 Metric Tons of CO2 by visiting me virtually!",
        "Hello there, park aficionado! I am St James's Park, London's verdant heart for over 400 years. Ready to stroll through history and maybe feed some royal ducks? \n\nThank you for saving 1.59 Metric Tons of CO2 by visiting me virtually!"
    };

    void Start()
    {
        testGemini();
        // PopulateDropdown();
        // if (dropdown != null)
        // {
        //     dropdown.onValueChanged.AddListener(delegate { DropdownIndexChanged(dropdown.value); });
        // }

        // dictationRecognizer = new DictationRecognizer();

        // dictationRecognizer.DictationResult += OnDictationResult;
        // dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        // dictationRecognizer.DictationComplete += OnDictationComplete;
        // dictationRecognizer.DictationError += OnDictationError;

        // DropdownIndexChanged(0);
    }

    async public void testGemini()
    {
        Debug.Log("gemini start");
        // URL of the API
        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        // Your API key
        var apiKey = "AIzaSyBgoeGvnFVqUsqT0P3NKw2dB-VMRRAnPA8";

        // Create a new instance of HttpClient
        using (HttpClient client = new HttpClient())
        {
            // Set the base address and add the API key to the URL
            client.BaseAddress = new Uri(url);
            var requestUri = $"?key={apiKey}";

            // JSON data to send in the POST request
            var jsonData = @"{
                    'contents': [{
                        'parts': [{
                            'text': 'Pretend you are the Sun. Tell me about yourself.'
                        }]
                    }]
                }";

            // Create HttpContent from the JSON string
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                // Send a POST request to the specified URI
                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read and output the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log(responseBody);
            }
            catch (HttpRequestException e)
            {
                Debug.Log("\nException Caught!");
                Debug.Log("Message :{0} " + e.Message);
            }
        }
    }

    private void OnDictationHypothesis(string text)
    {
        // Optionally, update the InputField with the hypothesis
    }

    public void PlayClipByIndex(int index)
    {
        if (index < 0 || index >= audioClips.Count)
        {
            Debug.LogError("PlayClipByIndex: Index out of range.");
            return;
        }

        // Update the currentClipIndex to the new index
        currentClipIndex = index;

        // Set the clip to the clip at the given index and play it
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    void speak(string text)
    {
        voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    void UpdateSkyboxTexture(int index)
    {
        var newSkyboxTexture = newSkyboxTextures[index];
        if (skyboxMaterial != null && newSkyboxTexture != null)
        {
            // Assign the new texture to the material
            skyboxMaterial.SetTexture("_MainTex", newSkyboxTexture);

            // Update the active skybox
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment(); // Update global illumination to reflect changes
        }
    }

    void PopulateDropdown()
    {
        // Ensure the dropdown is not null
        if (dropdown == null) return;

        // Optional: Clear existing options
        dropdown.ClearOptions();

        // Add the list of string options to the dropdown
        dropdown.AddOptions(options);
    }

    public async void AskChatGPT(string newText)
    {
        askGPTButtonText.text = "Asking...";
        newText = inputField.text;
        Debug.Log("AskChatGPT called with: " + newText);

        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();

        request.Messages = messages;
        request.Model = "gpt-3.5-turbo-0125";

        Debug.Log(request);

        var response = await openAI.CreateChatCompletion(request);

        Debug.Log(response);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            var textResponse = chatResponse.Content;

            Debug.Log(textResponse);

            speak(textResponse);

            if (myText != null)
            {
                myText.text = textResponse;
            }
        }
        askGPTButtonText.text = "Ask AI!";
    }

    public void pauseSpeech()
    {
        voice.Pause();
    }

    public void resumeSpeech()
    {
        voice.Resume();
    }

    void DropdownIndexChanged(int index)
    {
        clipIndex = index;
        dropdown.value = index;
        PlayClipByIndex(index);
        string siteName = options[index];
        Debug.Log("Current site: " + siteName);
        messages = new List<ChatMessage>();
        messages.Add(new ChatMessage { Content = "Pretend you are: " + siteName + ". Answer with tons of energy and fun but as concisely as possible and to the point.", Role = "system" });
        var displayText = greetings[index];
        myText.text = displayText;
        speak(displayText);
        if (index >= 0 && index < options.Count)
        {
            UpdateSkyboxTexture(index);
        }
        else
        {
            Debug.Log("Unrecognized Option");
        }
    }

    public void ChangeVideoClip(int newClipIndex)
    {
        if (newClipIndex < videoClips.Length)
        {
            clipIndex = newClipIndex; // Update the clipIndex with the new index
            videoPlayer.clip = videoClips[clipIndex];
            videoPlayer.Play();
        }
    }

    public void NextVideoClip()
    {
        clipIndex = (clipIndex + 1) % options.Count;
        DropdownIndexChanged(clipIndex);
    }

    public void StartDictation()
    {
        if (dictationRecognizer.Status == SpeechSystemStatus.Stopped)
        {
            startButtonText.text = "Recording!";
            dictationRecognizer.Start();
        }
    }

    public void StopDictation()
    {
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            startButtonText.text = "Start";
            dictationRecognizer.Stop();
        }
    }

    public void ResetDictation()
    {
        inputField.text = ""; // Clear the InputField
        StopDictation(); // Optionally, stop the dictation if it's running
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        inputField.text += text + " ";
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        if (cause != DictationCompletionCause.Complete)
            Debug.LogError("The dictation did not complete successfully.");
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError("Dictation error: " + error);
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.DictationHypothesis -= OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= OnDictationComplete;
            dictationRecognizer.DictationError -= OnDictationError;

            dictationRecognizer.Dispose();
        }
    }

}
