/*
 *  Copyright July 1, 2016 Shawn Gilroy 
 *  HybridWebApp - Selection based communication aide
 *  File="SaveAndLoadInterface.cs"
 *  
 *  ===================================
 *  
 *  Based on original code shared by Craig Dunn
 *  https://github.com/xamarin/xamarin-forms-samples/tree/master/WorkingWithFiles
 *  Released alongside Xamarin form samples
 *  
 */
 
namespace SGDWithCocos.Interface
{
    /// <summary>
    /// Native interface to saving/loading to device
    /// </summary>
    public interface ISaveAndLoad
    {
        void SaveJSON(string boardName, string text);
        string LoadJSON(string boardName);

        string GetDatabaseFilePath(string dbName);
    }
}
