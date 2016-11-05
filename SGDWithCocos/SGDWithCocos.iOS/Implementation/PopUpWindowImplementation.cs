/*
 *  Copyright July 1, 2016 Shawn Gilroy 
 *  HybridWebApp - Selection based communication aide
 *  File="EntryPopupLoader.cs"
 *    
 *  ===========================================
 *  
 *  Credit to Peter Brachwitz  
 *  https://forums.xamarin.com/profile/241540/PeterBrachwitz
 *  Originally shared publicly at https://forums.xamarin.com/discussion/35838/how-to-do-a-simple-inputbox-dialog
 *  
 */

using SGDWithCocos.Interface;
using SGDWithCocos.iOS.Implementation;
using SGDWithCocos.Views;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PopUpWindowImplementation))]
namespace SGDWithCocos.iOS.Implementation
{
    public class PopUpWindowImplementation : PopUpWindowInterface
    {
        public void ShowPopup(PopUpWindow popup)
        {
            var alert = new UIAlertView
            {
                Title = popup.Title,
                Message = popup.Text,
                AlertViewStyle = UIAlertViewStyle.PlainTextInput
            };
            foreach (var b in popup.Buttons)
                alert.AddButton(b);

            alert.Clicked += (s, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = popup.Buttons.ElementAt(Convert.ToInt32(args.ButtonIndex)),
                    Text = alert.GetTextField(0).Text
                });
            };
            alert.Show();
        }
    }
}