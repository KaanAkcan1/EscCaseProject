# Esc Case Projesi

Merhabalar.

Case'de verilen istekleri yerine getirmek için yapılmış bir projedir.

## Kurulum Adımları
=> "EscCase.Web.sln" dosyasını açarak projemizi açıyoruz.

=> Öncelikle projeyi açtıktan sonra startup projesi olarak "EscCase.Web" projesi seçilmelidir.

=> Ardından Package Manager Console açılarak Default Proje Olarak "EscCase.Data" seçeneğini seçmeniz gerekiyor.

=> "appsettings.json" dosyası içerisindeki "ConnectionStrings" içerisinde bulunan "MSSQL" değişkenindeki connection string'imizi kendi database'inize göre düzenlemeniz gerekmektedir.

=> Package Manager Console üzerinde "add-migration initial" komutunu yazmalısınız.

=> Done yazısını gördükten sonra "update-database" yazarak Done yazısını görünce Db'mizi hazır hale getiriyoruz.

=> Artık projemizi çalıştırabiliriz.

=> Proje çalıştıktan sonra "admin" kullanıcı adı ve "password" şifresi ile giriş yapabilirsiniz.

## Linkler ve Sayfalar
=> Ana sayfamızda ürünlerin listesi bulunmaktadır. Buradan yeni ürün eklemesi, düzenleme ve silme işlemleri yapılabilir.

=> Veri Al sayfasında projede istenildiği gibi verilerin Json formatında çıktısı gelmektedir.

=> Veri Yükle sayfasında önce rastgele veriler oluşturularak ekrandaki inputta görünüyor. Eğer istenilirse buradan formata uygun şekilde değişiklikler yapılabilir. Ardından sağ alttaki butona basıldığında bu veriler sisteme yüklenecektir.
