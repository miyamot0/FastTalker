/*
 *  Copyright July 1, 2016 Shawn Gilroy 
 *  HybridWebApp - Selection based communication aide
 *  File="PopUpWindowInterface.cs"
 *  
 *  ===================================
 *  
 *  Based on original code shared by Craig Dunn
 *  https://github.com/xamarin/xamarin-forms-samples/tree/master/WorkingWithFiles
 *  Released alongside Xamarin form samples
 *  
 */

using SGDWithCocos.Views;

namespace SGDWithCocos.Interface
{
    /// <summary>
    /// Interface for native pop-up dialogs
    /// </summary>
    public interface PopUpWindowInterface
    {
        void ShowPopup(PopUpWindow reference);
    }
}
