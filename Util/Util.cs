using Android.App;
using Android.Content;
using Android.Views;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PublicStruct.cs;
namespace MyUtil
{
   
    public class ItemDB
    {
        private SQLiteConnection _conn;

        public ItemDB(String folder)
        {
            _conn = new SQLiteConnection(System.IO.Path.Combine(folder, "droid.db"));

            _conn.CreateTable<Item>();
            _conn.CreateTable<AppSetting>();
        }

        public List<Item> SelectItem()
        {
            List<Item> items = _conn.Table<Item>().ToList<Item>();
            items.Sort();
            return items;
        }

        public List<AppSetting> SelectAppSetting()
        {
            return _conn.Table<AppSetting>().ToList<AppSetting>();
        }

        #region InsertItem

        public void InsertItem(Item item)
        {
            if (!(this.SelectItem().Any((e) => { return e.Link == item.Link; })))
                _conn.Insert(item);
        }

        public void InsertAllItem(List<Item> items)
        {
            foreach (var item in items)
            {
                InsertItem(item);
            }

            // _conn.InsertAll(list);
        }

        public void UpdateOrInsertAppSetting(AppSetting appsetting)
        {
            if (this.SelectAppSetting().Count == 0)
            {
                _conn.Insert(appsetting);
            }
            else
            {
                _conn.Update(appsetting);
            }
        }

        #endregion InsertItem

        #region UpdateItem
        public void UpdateItem(Item item)
        {
            _conn.Update(item);
        }

        #endregion

        #region Delete

        public void DeleteItem(Item item)
        {
            _conn.Delete(item);
        }

        public void DeleteAllItem()
        {
            _conn.DeleteAll<Item>();
        }

        #endregion Delete
    }

    public class Util
    {
        public static void CreateTable()
        {
        }

        public static void Dialog(Context context, String tile, String message, EventHandler<DialogClickEventArgs> ok, EventHandler<DialogClickEventArgs> cancel)
        {
            var dialog = new AlertDialog.Builder(context);
            dialog.SetTitle(tile);
            dialog.SetMessage(message);
            dialog.SetPositiveButton("OK", ok);
            dialog.SetNegativeButton("Cancel", cancel);

            dialog.Create();
            dialog.Show();
        }

        public static void InputDialog(Context context, View view, EventHandler<DialogClickEventArgs> ok, EventHandler<DialogClickEventArgs> cancel)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);

            builder.SetView(view);

            builder.SetPositiveButton("OK", ok);
            builder.SetNegativeButton("Cancel", cancel);
            builder.Create();
            builder.Show();
        }
    }
}