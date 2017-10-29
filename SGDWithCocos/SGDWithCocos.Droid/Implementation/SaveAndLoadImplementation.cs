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
        /// <summary>
        /// Saves Json to disk
        /// </summary>
        /// <param name="boardName">Board name.</param>
        /// <param name="text">Text.</param>
        public void SaveJSON(string boardName, string text)
        {
            var filename = boardName + ".json";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Loads Json from disks
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="boardName">Board name.</param>
        public string LoadJSON(string boardName)
        {
            var filename = boardName + ".json";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);

            return (File.Exists(filePath)) ? File.ReadAllText(filePath) : "";
        }
    }
}