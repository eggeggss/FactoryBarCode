# FactoryBarcode
Xamarin.Android
barcode scanner to Web
your html need to implement the javascript
 
 function msg(content) {
            document.getElementById('textarea1').value = content;

            var json = JSON.parse(content);
            
            document.getElementById('txt_id').value = json.user_id;
            document.getElementById('txt_name').value = json.user_name;
  }
  
  After the implementing, 
  The barcode scanner will scan the qrcode and fetch the text result into your web through if your html have the "function msg(content)".
  Then you can use DOM or JQuery selector to analysis data to your application
   
