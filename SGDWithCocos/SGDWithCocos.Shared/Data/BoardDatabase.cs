//----------------------------------------------------------------------------------------------
// <copyright file="BoardDatabase.cs" 
// Copyright November 6, 2016 Shawn Gilroy
//
// This file is part of Cross Platform Communication App
//
// Cross Platform Communication App is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Cross Platform Communication App is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Cross Platform Communication App.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Cross Platform Communication App is a tool to assist clinicans and researchers in the treatment of communication disorders.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

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

        /// <summary>
        /// Construct database for icons
        /// </summary>
        /// <param name="dbFilePath"></param>
        public BoardDatabase(string dbFilePath)
        {
            database = new SQLiteAsyncConnection(dbFilePath);
            database.CreateTableAsync<TableIcons>().Wait();
            database.CreateTableAsync<TableStoredIcons>().Wait();
            database.CreateTableAsync<TableFolders>().Wait();
            database.CreateTableAsync<TableSettings>().Wait();
        }

        public void Init() { }


        /// <summary>
        /// Get icons
        /// </summary>
        /// <returns></returns>
        public Task<List<TableIcons>> GetIconsAsync()
        {
            return database.Table<TableIcons>().ToListAsync();
        }

        /// <summary>
        /// Get stored icons
        /// </summary>
        /// <returns></returns>
        public Task<List<TableStoredIcons>> GetStoredIconsAsync()
        {
            return database.Table<TableStoredIcons>().ToListAsync();
        }

        /// <summary>
        /// Get folders
        /// </summary>
        /// <returns></returns>
        public Task<List<TableFolders>> GetFolderIconsAsync()
        {
            return database.Table<TableFolders>().ToListAsync();
        }

        /// <summary>
        /// Get settings
        /// </summary>
        /// <returns></returns>
        public Task<TableSettings> GetSettingsAsync()
        {
            return database.Table<TableSettings>().FirstAsync();
        }

        /// <summary>
        /// Save icons
        /// </summary>
        /// <param name="icons"></param>
        /// <returns></returns>
        public Task<int> SaveIconsAsync(List<TableIcons> icons)
        {
            database.DropTableAsync<TableIcons>().Wait();
            database.CreateTableAsync<TableIcons>().Wait();

            return database.InsertAllAsync(icons);
        }

        /// <summary>
        /// Save stored icons
        /// </summary>
        /// <param name="icons"></param>
        /// <returns></returns>
        public Task<int> SaveStoredIconsAsync(List<TableStoredIcons> storedIcons)
        {
            database.DropTableAsync<TableStoredIcons>().Wait();
            database.CreateTableAsync<TableStoredIcons>().Wait();

            return database.InsertAllAsync(storedIcons);
        }

        /// <summary>
        /// Save folders
        /// </summary>
        /// <param name="icons"></param>
        /// <returns></returns>
        public Task<int> SaveFoldersAsync(List<TableFolders> folders)
        {
            database.DropTableAsync<TableFolders>().Wait();
            database.CreateTableAsync<TableFolders>().Wait();

            return database.InsertAllAsync(folders);
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public Task<int> SaveSettingsAsync(TableSettings settings)
        {
            database.DropTableAsync<TableSettings>().Wait();
            database.CreateTableAsync<TableSettings>().Wait();

            return database.InsertAsync(settings);
        }
    }
}
