/*
 *  Copyright July 1, 2016 Shawn Gilroy 
 *  HybridWebApp - Selection based communication aide
 *  File="TextToSpeechImplementation.cs"
 *  
 *  ===================================================
 *  
 *  Based on code samples shared by Xamarin
 *  https://developer.xamarin.com/guides/xamarin-forms/dependency-service/text-to-speech/
 *    
 */
 
using SGDWithCocos.Interface;
using AVFoundation;
using SGDWithCocos.iOS.Implementation;

[assembly: Xamarin.Forms.Dependency(typeof(TextToSpeechImplementation))]
namespace SGDWithCocos.iOS.Implementation
{
    public class TextToSpeechImplementation : ITextToSpeech
    {
        public TextToSpeechImplementation() { }

        public void Speak(string text)
        {
            var speechSynthesizer = new AVSpeechSynthesizer();

            var speechUtterance = new AVSpeechUtterance(text)
            {
                Rate = AVSpeechUtterance.MaximumSpeechRate / 3,
                Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
                Volume = 0.85f,
                PitchMultiplier = 1.0f
            };

            speechSynthesizer.SpeakUtterance(speechUtterance);
        }
    }
}