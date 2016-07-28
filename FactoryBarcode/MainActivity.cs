﻿using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using MyUtil;
using Newtonsoft.Json;
using System;
using ZXing.Mobile;

namespace FactoryBarcode
{
    public interface IBarCodeToHtmlBehavior
    {
        void CallWebViewSendData();
    }

    [Activity(Label = "FactoryBarcode", MainLauncher = false, Icon = "@drawable/Barcode", ConfigurationChanges = Android.Content.PM.ConfigChanges.Keyboard | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity, IBarCodeToHtmlBehavior
    {
        private int count = 1;
        private WebView wv;
        private SwipeRefreshLayout refresher;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var j = this.Intent.GetStringExtra("ItemRow");
            Item item = JsonConvert.DeserializeObject<Item>(j);

            FindViewById<TextView>(Resource.Id.txtTitle).Text = item.Descrip;
            //back
            this.FindViewById<Button>(Resource.Id.btnBack).Click += (s1, e1) =>
            {
                this.Finish();
                // wv.GoBack();
            };
            //scan
            ImageButton btnScan = this.FindViewById<ImageButton>(Resource.Id.btnScan);

            btnScan.Click += (s1, e1) =>
           {
               CallWebViewSendData();
           };

            btnScan.SetBackgroundResource(Resource.Drawable.Barcode2);

            wv = this.FindViewById<WebView>(Resource.Id.webview);
            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

            refresher.SetColorScheme(Resource.Color.xam_dark_blue,
                                      Resource.Color.xam_purple,
                                      Resource.Color.xam_gray,
                                      Resource.Color.xam_green);

            wv.Settings.JavaScriptEnabled = true;

            wv.AddJavascriptInterface(new JsInteration(this), "control");

            wv.Settings.SetSupportZoom(true);
            wv.Settings.BuiltInZoomControls = true;
            wv.Settings.UseWideViewPort = true;
            wv.Settings.LoadWithOverviewMode = true;

            WebChromeClient wc = new WebChromeClient();

            wv.SetWebChromeClient(wc);

            MyWebViewClient wvc = new MyWebViewClient(refresher);

            wv.SetWebViewClient(wvc);

            wv.LoadUrl(item.Link);

            Information info = new Information() { user_id = "A110018", user_name = "Roger Roan" };
            String content = JsonConvert.SerializeObject(info, Formatting.None);

            MobileBarcodeScanner.Initialize(Application);

            refresher.Refresh += delegate
           {
               wv.Reload();
           };
        }

        public async void CallWebViewSendData()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            {
                wv.LoadUrl("javascript:msg(" + @"'" + result.Text + @"'" + ");");
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            //base.OnConfigurationChanged(newConfig);
        }

        public static implicit operator MainActivity(Index1Activity v)
        {
            throw new NotImplementedException();
        }
    }

    public class MyWebViewClient : WebViewClient
    {
        public SwipeRefreshLayout _refresher;

        public MyWebViewClient(SwipeRefreshLayout refresher)
        {
            this._refresher = refresher;
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            this._refresher.Refreshing = true;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            this._refresher.Refreshing = false;
        }
    }

    public class Information
    {
        public String user_name { set; get; }
        public String user_id { set; get; }
    }

    public class JsInteration : Java.Lang.Object
    {
        private Context context;

        public JsInteration(Context context)
        {
            this.context = context;
        }

        [Export]
        [JavascriptInterface]
        public void HtmlMessage(String msg)
        {
            IBarCodeToHtmlBehavior Ibarcodebehavior = (MainActivity)this.context;
            Ibarcodebehavior.CallWebViewSendData();
        }
    }
}