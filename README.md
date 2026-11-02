# 🚀 ASP.NET Core MVC (.NET 8) StayFinder — Otel Arama Uygulaması

M&Y Yazılım Eğitim Akademi Danışmanlık bünyesinde aldığım eğitim sürecinde, öğrendiklerimi pratiğe dökerek RapidAPI üzerindeki Booking.com 15 servisini tüketen uçtan uca bir otel arama uygulaması geliştirdim.

Amacım kendi veritabanında otel tutan statik bir liste yapmak değil; şehir, tarih, misafir ve para birimiyle canlı arama yapan, sonuçları kartlar halinde sunan ve seçilen otelin detayını gösteren sürdürülebilir bir MVC mimarisi inşa etmekti.

---

## 🛠️ Mimari ve Teknik Detaylar

* **MVC Mimarisi:** ASP.NET Core MVC (.NET 8) standartlarına uygun Controller, View ve ViewModel ayrımıyla temiz bir yapı kurgulandı.
* **Harici REST API:** Kendi veritabanı yerine RapidAPI / Booking.com 15 servisi `HttpClient` ile tüketildi.
* **DTO & ViewModel:** JSON yanıtları DTO’lara, ekran ihtiyaçları ise tip güvenli ViewModel’lere dönüştürüldü.
* **Konfigürasyon:** RapidAPI anahtarı `appsettings.Local.json` üzerinden yönetildi; host bilgisi `appsettings.json` içinde tutuldu.
* **Kimlik Doğrulama:** Dış servis çağrılarında `x-rapidapi-key` ve `x-rapidapi-host` HTTP header’ları kullanıldı.
* **Arama Akışı:** Şehir adından `dest_id` çözme, otel listesi çekme ve `hotel_id` ile detay alma üç adımlı olarak modellendi.
* **Modüler Arayüz:** StayFinder için özel layout, responsive kartlar, arama özeti paneli ve yeniden kullanılabilir Razor view’lar tercih edildi.

---

## 📌 Uygulama Modülleri

### ▫️ Arama & Keşif
* 🏨 **Ana Sayfa Arama Formu** — destinasyon, giriş/çıkış tarihi, misafir, oda ve para birimi
* 🗺️ **Popüler Şehir Kartları** — tek tıkla arama başlatma
* 🌍 **Destinasyon Çözümleme** — şehir adından `dest_id` alma

### ▫️ Otel Listesi & Filtreleme
* 🛏️ **Otel Kartları** — fotoğraf, yıldız, puan ve fiyat
* 🔎 **İstemci Tarafı Filtreleme** — yıldız, puan ve fiyat aralığı
* ↕️ **Sıralama** — liste ekranında JavaScript ile sıralama
* 💶 **Para Birimi** — EUR, USD, TRY ve GBP desteği

### ▫️ Otel Detay
* 🖼️ **Fotoğraf Galerisi**
* ⚙️ **Olanaklar ve Oda Seçenekleri**
* ⭐ **Puan, Fiyat ve Rezervasyon Bilgisi**

---

## 💻 Kullanılan Teknoloji Yığını

* **Backend & Framework:** ASP.NET Core MVC (.NET 8) │ C# │ `HttpClient`
* **Dış Servis:** RapidAPI │ Booking.com 15 │ JSON (Newtonsoft.Json)
* **Konfigürasyon:** `appsettings.json` │ `appsettings.Local.json`
* **Frontend & UI Components:** Razor Views │ Bootstrap 5 │ Bootstrap Icons │ HTML5 / CSS3 │ JavaScript

---

## 💡 Kazanımlar & Deneyim

M&Y Yazılım Eğitim Akademi Danışmanlık bünyesinde aldığım eğitim sürecinde, öğrendiklerimi pratiğe dökerek StayFinder otel arama uygulamasını geliştirdim.
Bu çalışma sayesinde yalnızca bir web arayüzü ortaya çıkarmış olmadım; aynı zamanda harici REST API tüketimi, JSON’dan DTO ve ViewModel’e veri dönüşümü, HTTP header tabanlı kimlik doğrulama (RapidAPI Key) ve Razor tabanlı arayüz geliştirme konularında da önemli deneyimler kazandım.

---


Eğitim sürecime katkı sağlayan, bilgi ve tecrübeleriyle gelişimime destek olan **Murat Yücedağ** ve **Erhan Gündüz** hocalarıma emekleri için teşekkür ederim.

<br/>

<img width="1229" height="510" alt="1" src="https://github.com/user-attachments/assets/0a30dd85-b078-4408-9459-762e20a3ac30" />


<br/>

<img width="982" height="533" alt="2" src="https://github.com/user-attachments/assets/1d052b0e-f01f-47f5-9981-875e5323f30f" />


<br/>
<img width="1016" height="886" alt="6" src="https://github.com/user-attachments/assets/513d4ec2-1970-4756-9ca6-c06d46e3b1b7" />
<br/>
<img width="1016" height="886" alt="7" src="https://github.com/user-attachments/assets/4adba295-7ff3-4ef3-ba88-1ef4e1d179f3" />

<br/>

<img width="899" height="305" alt="8" src="https://github.com/user-attachments/assets/7abb9c3f-254d-4d2e-982d-252bc0cc201b" />

<br/>
<img width="987" height="840" alt="4" src="https://github.com/user-attachments/assets/5edb5444-36ae-4d24-bcbf-90b3837e6af6" />




