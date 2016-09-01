using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Java.IO;
using MyUtil;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Android.Views.View;

namespace FactoryBarcode
{
    public interface ICommucatable
    {
        void DeleteItem(Item item);
    }

    [Activity(Label = "HTML5 BarCoder", MainLauncher = true, Icon = "@drawable/icon512")]
    public class Index1Activity : Activity, ICommucatable
    {
        public ItemDB _itemdb;
        private MyList adapter;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_add:
                    View v = this.LayoutInflater.Inflate(Resource.Layout.Dialog, null);

                    EventHandler<DialogClickEventArgs> ok = new EventHandler<DialogClickEventArgs>((s2, e2) =>
                    {
                        String descrip = v.FindViewById<EditText>(Resource.Id.editDescrip).Text;
                        String uri = v.FindViewById<EditText>(Resource.Id.editUri).Text;

                        Item itemNew = new Item() { Descrip = descrip, Link = uri };

                        this._itemdb.InsertItem(itemNew);
                        adapter.List.Add(itemNew);
                        adapter.List.Sort();
                        adapter.NotifyDataSetChanged();
                        //Toast.MakeText(this, descrip, ToastLength.Short).Show();
                    });

                    Util.InputDialog(this, v, ok, null);

                    return true;

                case Resource.Id.action_settings:

                    View viewAppsetting = this.LayoutInflater.Inflate(Resource.Layout.AppSetting, null);

                    Util.InputDialog(this, viewAppsetting, null, null);

                    //Toast.MakeText(this, "setting", ToastLength.Short).Show();
                    return true;

                default:
                    // If we got here, the user's action was not recognized.
                    // Invoke the superclass to handle it.
                    return base.OnOptionsItemSelected(item);
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Index1);

            ColorDrawable color = new ColorDrawable(Color.OrangeRed);

            this.ActionBar.SetBackgroundDrawable(color);

            List<Item> list = new List<Item>();
            list.Add(new Item() { Descrip = "DevExpress", Link = "https://www.devexpress.com/" });
            list.Add(new Item() { Descrip = "Xamarin", Link = "https://www.xamarin.com/" });
            list.Add(new Item() { Descrip = "奇摩", Link = "http://www.yahoo.com.tw" });
            list.Add(new Item() { Descrip = "Google", Link = "http://www.google.com" });
            list.Add(new Item() { Descrip = "facebook", Link = "http://www.facebook.com" });
            list.Add(new Item() { Descrip = "YouTube", Link = "https://www.youtube.com/?hl=zh-TW&gl=TW" });
            list.Add(new Item() { Descrip = "PCHOME", Link = "http://www.pchome.com.tw/" });
            list.Add(new Item() { Descrip = "Roger測試報表", Link = "http://eggeggss.ddns.net/notesbarcode/notesservice1.aspx" });

            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            _itemdb = new ItemDB(folder);

            //_itemdb.DeleteAllItem();
            _itemdb.InsertAllItem(list);

            List<Item> listFromDb = _itemdb.SelectItem();

            var listview = this.FindViewById<ListView>(Resource.Id.listview);

            adapter = new MyList();

            adapter.Context = this;
            adapter.List = listFromDb;
            listview.Adapter = adapter;

            var refresh = this.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher1);

            refresh.Refresh += (s1, e1) =>
            {
                var items = _itemdb.SelectItem();
                adapter.List = items;
                adapter.NotifyDataSetChanged();
                refresh.Refreshing = false;
            };
            // Create your application here
        }

        public void DeleteItem(Item item)
        {
            _itemdb.DeleteItem(item);
            // throw new NotImplementedException();
        }
    }

    public class MyList : BaseAdapter<Item>
    {
        public Index1Activity Context;
        public List<Item> List;

        public override Item this[int position]
        {
            get
            {
                return this.List[position];
                //throw new NotImplementedException();
            }
        }

        public override int Count
        {
            get
            {
                return this.List.Count;
                //throw new NotImplementedException();
            }
        }

        public void DeleteItem(Item item)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return position;
            //throw new NotImplementedException();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //前面幾筆進來時回收區還是空的
            //註冊在這邊是因為後面的view都用這個回收的所以初始註冊一次即可
            if (convertView == null)
            {
                convertView = this.Context.LayoutInflater.Inflate(Resource.Layout.ItemRow, null);

                var btn_open = convertView.FindViewById<Button>(Resource.Id.btn_open);
                //opent
                btn_open.Click += (s1, e1) =>
                {
                    var thisbtn = s1 as Button;
                    Int32 ll_position = Convert.ToInt32(thisbtn.Tag);

                    var item_row = this.List[ll_position];
                    String itemRowJson = JsonConvert.SerializeObject(item_row);

                    Intent intent = new Intent();
                    intent.SetClass(this.Context, typeof(MainActivity));
                    intent.PutExtra("ItemRow", itemRowJson);

                    this.Context.StartActivity(intent);
                };
                //delete
                var btn_delete = convertView.FindViewById<Button>(Resource.Id.btn_delete);

                btn_delete.SetTextColor(Android.Graphics.Color.Red);

                btn_delete.Click += (s1, e1) =>
               {
                   EventHandler<DialogClickEventArgs> okDelegate = new EventHandler<DialogClickEventArgs>((s2, e2) =>
                   {
                       var delbtn = s1 as Button;
                       Int32 ll_position = Convert.ToInt32(delbtn.Tag);

                       var item_row = this.List[ll_position];

                       List.Remove(item_row);
                       ICommucatable icomucate = this.Context as ICommucatable;

                       icomucate.DeleteItem(item_row);

                       List.Sort();
                       this.NotifyDataSetChanged();
                   });

                   Util.Dialog(this.Context, "Information", "Are You Sure To Delete?", okDelegate, null);
               };
            }
            Item item = List[position];
            var btn = convertView.FindViewById<Button>(Resource.Id.btn_open);
            var btnDel = convertView.FindViewById<Button>(Resource.Id.btn_delete);
            btn.Tag = position;
            btnDel.Tag = position;
            convertView.FindViewById<TextView>(Resource.Id.descrip).Text = item.Descrip;

            return convertView;

            //throw new NotImplementedException();
        }
    }
}