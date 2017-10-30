using SGDWithCocos.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SGDWithCocos.Data
{
    public class BoardDatabase
    {
        private SQLiteAsyncConnection database;

        public BoardDatabase(string dbFilePath)
        {
            database = new SQLiteAsyncConnection(dbFilePath);
            database.CreateTableAsync<TableIcons>().Wait();
            database.CreateTableAsync<TableStoredIcons>().Wait();
            database.CreateTableAsync<TableFolders>().Wait();
            database.CreateTableAsync<TableSettings>().Wait();
        }

        public Task<List<TableIcons>> GetIconsAsync()
        {
            return database.Table<TableIcons>().ToListAsync();
        }

        public Task<List<TableStoredIcons>> GetStoredIconsAsync()
        {
            return database.Table<TableStoredIcons>().ToListAsync();
        }

        public Task<List<TableFolders>> GetFolderIconsAsync()
        {
            return database.Table<TableFolders>().ToListAsync();
        }

        public Task<TableSettings> GetSettingsAsync()
        {
            return database.Table<TableSettings>().FirstAsync();
        }

        public Task<int> SaveIconsAsync(List<TableIcons> icons)
        {
            database.DropTableAsync<TableIcons>().Wait();
            database.CreateTableAsync<TableIcons>().Wait();

            return database.InsertAllAsync(icons);
        }

        public Task<int> SaveStoredIconsAsync(List<TableStoredIcons> icons)
        {
            database.DropTableAsync<TableStoredIcons>().Wait();
            database.CreateTableAsync<TableStoredIcons>().Wait();

            return database.InsertAllAsync(icons);
        }

        public Task<int> SaveFoldersAsync(List<TableFolders> icons)
        {
            database.DropTableAsync<TableFolders>().Wait();
            database.CreateTableAsync<TableFolders>().Wait();

            return database.InsertAllAsync(icons);
        }

        public Task<int> SaveSettingsAsync(TableSettings settings)
        {
            database.DropTableAsync<TableSettings>().Wait();
            database.CreateTableAsync<TableSettings>().Wait();

            return database.InsertAsync(settings);
        }

        //public Task<int> SaveItemAsync(TodoItem item)
        //{
        //    if (item.ID != 0)
        //    {
        //        return database.UpdateAsync(item);
        //    }
        //    else
        //    {
        //        return database.InsertAsync(item);
        //    }
        //}

        //public Task<int> DeleteItemAsync(TodoItem item)
        //{
        //    return database.DeleteAsync(item);
        //}
    }
}
