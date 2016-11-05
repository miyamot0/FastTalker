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
 
using System.Collections.Generic;
using SGDWithCocos.Interface;
using Android.Speech.Tts;
using SGDWithCocos.Droid.Implementation;
using Xamarin.Forms;
using Android.Media;
using Android.Content;

[assembly: Dependency(typeof(TextToSpeechImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    public class TextToSpeechImplementation : Java.Lang.Object, ITextToSpeech, TextToSpeech.IOnInitListener
    {
        TextToSpeech speaker;
        string toSpeak;

        public TextToSpeechImplementation() { }

        public void Speak(string text)
        {
            var ctx = Forms.Context; // useful for many Android SDK features
            toSpeak = text;
            if (speaker == null)
            {
                AudioManager am = (AudioManager) ctx.GetSystemService(Context.AudioService);
                int amStreamMusicMaxVol = am.GetStreamMaxVolume(Android.Media.Stream.Music);
                am.SetStreamVolume(Android.Media.Stream.Music, amStreamMusicMaxVol, 0);
                speaker = new TextToSpeech(ctx, this);
            }
            else
            {
                var p = new Dictionary<string, string>();
                speaker.Speak(toSpeak, QueueMode.Flush, p);
            }
        }

        #region IOnInitListener implementation
        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                var p = new Dictionary<string, string>();
                speaker.Speak(toSpeak, QueueMode.Flush, p);
            }
        }
        #endregion
    }
}