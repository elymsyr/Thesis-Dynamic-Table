[Türkçe kendime notlar]

# İçindekiler

1. [Dosyalar](#docs)
2. [Versiyonlar](#versions)
3. [Notlar](#notes)


# Dosyalar <a name = "docs"></a>
[1.png](1.png) -> Normal çalışma durumu 1793. bölüm <br>
[1.mp4](1.mp4) -> 2100+ bölümlük kısa kayıt <br>

[2.png](2.png) -> 2300+ bölümlük çalışma durumu <br>
[2.mp4](2.mp4) -> Sadece yakın parçaların çalıştığı durum kaydı<br>

[3.png](3.png) -> [2.png](2.png) durumunun sonuçları<br>
[3.mp4](3.mp4) -> [2.mp4](2.mp4) durumunda 2000. bölüme yaklaşıldığında gerçekleşen durum (Product başlangıç noktasında kalıyor ve sadece nadiren ilerleyebiliyor.)<br>


# Versiyonlar <a name = "versions"></a>

0.1 : İlk versiyon (Silindi)<br>
0.1.1 : Güncel orjinal versiyon ([2.png](2.png)'de görünen)<br>
0.1.2 : Sadece yakın parçaların çalıştığı versiyon (Eğitimde başarısız duruyor)<br>

## Notlar <a name = "notes"></a>

v0.1.1 şuan stabil çalışabiliyor ve eğitim yapıldığında daha iyi sonuçlar alınabiliyor. Decision Period değeri 5-10 arasında oynadım. Tam sayıları kaydetmedim ancak 8-10 arasında bir değer 5'ten daha hızlı sonuç alıyor gibi duruyor. actionLimit değerini 1000-1600 arası tutnak yeterli gibi duruyor. 2000 değerine çekerek deneme yapılabilir.<br>

v0.1.2, 2000 bölümden sonra tamamen işe yaramaz bir hale geliyor. Şimdilik bu versiyonu rafa kaldırdım. Aşağıda bu versiyonun son halinde yaptığım en uzun çalıştırmanın sonuçları var:<br>
\[INFO] AgentBehavior. Step: 50000. Time Elapsed: 1179.078 s. Mean Reward: -12.308. Std of Reward: 6.898. Training.<br>
\[INFO] AgentBehavior. Step: 100000. Time Elapsed: 2294.535 s. Mean Reward: -11.139. Std of Reward: 6.070. Training.<br>
\[INFO] AgentBehavior. Step: 150000. Time Elapsed: 3354.897 s. Mean Reward: -9.908. Std of Reward: 6.382. Training.<br>
\[INFO] AgentBehavior. Step: 200000. Time Elapsed: 4416.659 s. Mean Reward: -9.606. Std of Reward: 4.884. Training.<br>
\[INFO] AgentBehavior. Step: 250000. Time Elapsed: 5553.368 s. Mean Reward: -10.071. Std of Reward: 4.416. Training.<br>
\[INFO] AgentBehavior. Step: 300000. Time Elapsed: 6633.012 s. Mean Reward: -9.847. Std of Reward: 3.743. Training.<br>

Std of Reward değeri: Modelin aldığı ödüllerin standart sapması. (Tamamlanmış bir modelde beklediğim Std değişimi -> düşük, yüksek, düşük)

