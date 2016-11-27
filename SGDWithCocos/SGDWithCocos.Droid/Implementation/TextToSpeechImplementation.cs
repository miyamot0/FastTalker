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
using Android.OS;
using System;

[assembly: Dependency(typeof(TextToSpeechImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    public class TextToSpeechImplementation : Java.Lang.Object, ITextToSpeech, TextToSpeech.IOnInitListener
    {
        TextToSpeech speaker;
        string toSpeak;

        public TextToSpeechImplementation() { }

        /// <summary>
        /// Public interface to Forms app, with Api-level routing
        /// </summary>
        /// <param name="text"></param>
        public void Speak(string text)
        {
            var ctx = Forms.Context;
            toSpeak = text;
            if (speaker == null)
            {
                AudioManager am = (AudioManager) ctx.GetSystemService(Context.AudioService);
                int amStreamMusicMaxVol = am.GetStreamMaxVolume(Android.Media.Stream.Music);
                am.SetStreamVolume(Stream.Music, amStreamMusicMaxVol, 0);
                speaker = new TextToSpeech(ctx, this);
            }
            else
            {
                SpeakRoute(toSpeak);
            }
        }

        #region IOnInitListener implementation

        /// <summary>
        /// After init, echo out speech
        /// </summary>
        /// <param name="status"></param>
        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                SpeakRoute(toSpeak);
            }
        }

        #endregion

        /// <summary>
        /// Route speech to suitable Api 
        /// </summary>
        /// <param name="text"></param>
        private void SpeakRoute(string text)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                ApiOver21(text);
            }
            else
            {
                ApiUnder20(text);
            }
        }

        /// <summary>
        /// Obsolete method call for TTS, necessary for legacy devices
        /// </summary>
        /// <param name="text"></param>
        [Obsolete("Message")]
        private void ApiUnder20(string text)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(TextToSpeech.Engine.KeyParamUtteranceId, "MessageId");
            speaker.Speak(text, QueueMode.Flush, map);
        }

        /// <summary>
        /// More suited TTS call, for lollipop and greater
        /// </summary>
        /// <param name="text"></param>
        private void ApiOver21(string text)
        {
            string utteranceId = this.GetHashCode() + "";
            speaker.Speak(text, QueueMode.Flush, null, utteranceId);
        }
    }
}