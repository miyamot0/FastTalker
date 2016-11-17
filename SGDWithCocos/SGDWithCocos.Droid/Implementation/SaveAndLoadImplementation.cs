/*
 *  Copyright July 1, 2016 Shawn Gilroy 
 *  HybridWebApp - Selection based communication aide
 *  File="SaveAndLoadImplementation.cs"
 *  
 *  ===================================
 *  
 *  Based on original code shared by Craig Dunn
 *  https://github.com/xamarin/xamarin-forms-samples/tree/master/WorkingWithFiles
 *  Released alongside Xamarin form samples
 *  
 */

using SGDWithCocos.Droid.Implementation;
using SGDWithCocos.Interface;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoadImplementation))]
namespace SGDWithCocos.Droid.Implementation
{
    public class SaveAndLoadImplementation : ISaveAndLoad
    {
        public void SaveJSON(string boardName, string text)
        {
            var filename = boardName + ".json";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            File.WriteAllText(filePath, text);
        }

        public string LoadJSON(string boardName)
        {
            var filename = boardName + ".json";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);

            return (File.Exists(filePath)) ? File.ReadAllText(filePath) : "";
        }

        public string GetDirectory(string assetName)
        {
            var filename = assetName + ".png";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);

            return filePath;
        }
    }
}