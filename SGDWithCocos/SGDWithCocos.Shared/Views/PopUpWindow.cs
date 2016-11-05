/*
 *  Copyright July 1, 2016 Shawn Gilroy 
 *  HybridWebApp - Selection based communication aide
 *  File="PopUpWindow.cs"
 *  
 *  ===========================================
 *  
 *  Credit to Peter Brachwitz  
 *  https://forums.xamarin.com/profile/241540/PeterBrachwitz
 *  Originally shared publicly at https://forums.xamarin.com/discussion/35838/how-to-do-a-simple-inputbox-dialog
 *  
 */

using SGDWithCocos.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SGDWithCocos.Views
{
    public class PopUpWindowArgs : EventArgs
    {
        public string Text { get; set; }
        public string Button { get; set; }
    }

    public class PopUpWindow
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public List<string> Buttons { get; set; }

        public PopUpWindow(string title, string text, params string[] buttons)
        {
            Title = title;
            Text = text;
            Buttons = buttons.ToList();
        }

        public PopUpWindow(string title, string text) : this(title, text, "OK", "Cancel") { }

        public event EventHandler<PopUpWindowArgs> PopupClosed;
        public void OnPopupClosed(PopUpWindowArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        public void Show()
        {
            DependencyService.Get<PopUpWindowInterface>().ShowPopup(this);
        }
    }
}
