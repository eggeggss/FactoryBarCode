The Barcode scanner is aim to provide a scanner tool to send data into html web through android webview.
Your html need to implement these javascript.
 <script>

 function msg(content) 
 {
 
            document.getElementById('textarea1').value = content;
            var json = JSON.parse(content);
            
            document.getElementById('txt_id').value = json.user_id;
            document.getElementById('txt_name').value = json.user_name;
  }
  </script>
  
  After this implementing, 
  The barcode scanner will scan the qrcode and fetch the text result into your web if your html have been implement "function msg(content)".
  Then you can use DOM or JQuery selector to analysis data into your web application.
  You can find the app in Google Play Store "HTML5 Barcoder".
  The tool is Xamarin.android.classic.
