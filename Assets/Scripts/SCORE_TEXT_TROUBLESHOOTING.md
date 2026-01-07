# Score Text Disappearing - Troubleshooting Guide

## 🔴 Problem: "Score: 0" yazısı oyun başladığında kayboluyor

## 🔍 Debug Adımları

### Adım 1: Console Loglarını Kontrol Edin

Oyunu Play modunda çalıştırın ve Console'da şu mesajları arayın:

#### ✅ Başarılı Durum:
```
[ScoreManager] Awake() called
[ScoreManager] ✓ Singleton instance created
[ScoreManager] TextMeshPro found: ScoreText | Active: True | Scale: (1.0, 1.0, 1.0)
[ScoreManager] Start() called - Initializing score display...
[ScoreManager] TextMeshPro activated: ScoreText
[ScoreManager] UpdateScoreDisplay() - Setting text to: 'Score: 0'
[ScoreManager] ✓ TextMeshPro updated: 'Score: 0' | Color: RGBA(1.000, 0.000, 0.000, 1.000) | Alpha: 1
[ScoreManager] ✓ Start() complete - Score display initialized
```

#### ❌ Sorunlu Durumlar:

**A) "NO TEXT COMPONENT ASSIGNED" hatası:**
```
[ScoreManager] ✗ NO TEXT COMPONENT ASSIGNED!
```
**Çözüm**: ScoreManager Inspector'ında "Score Text TMP" alanına TextMeshPro component'ini sürükleyin.

**B) "Active: False" görünüyorsa:**
```
[ScoreManager] TextMeshPro found: ScoreText | Active: False
```
**Çözüm**: Hierarchy'de ScoreText GameObject'ini seçin ve Inspector'da aktif edin (checkbox işaretli olmalı).

**C) "Alpha: 0" görünüyorsa:**
```
[ScoreManager] ✓ TextMeshPro updated: 'Score: 0' | Color: RGBA(1.000, 0.000, 0.000, 0.000) | Alpha: 0
```
**Çözüm**: TextMeshPro component'inin Color ayarında Alpha değerini 255 (veya 1.0) yapın.

---

## 🛠️ Olası Sorunlar ve Çözümleri

### 1. **Text GameObject Deaktif**

**Belirti**: Yazı hiç görünmüyor.

**Kontrol**:
- Hierarchy'de ScoreText GameObject'ini seçin
- Inspector'da en üstteki checkbox işaretli mi?

**Çözüm**:
- Checkbox'ı işaretleyin (GameObject aktif olmalı)
- Kod artık otomatik olarak aktif ediyor ama yine de kontrol edin

### 2. **Canvas Render Mode Sorunu**

**Belirti**: Yazı var ama görünmüyor.

**Kontrol**:
- Canvas'ı seçin
- Inspector → Canvas component
- Render Mode: "Screen Space - Overlay" olmalı

**Çözüm**:
- Render Mode'u "Screen Space - Overlay" yapın
- Veya "Screen Space - Camera" kullanıyorsanız, Render Camera'yı atayın

### 3. **Text Rengi veya Alpha Sorunu**

**Belirti**: Yazı var ama görünmüyor (transparan).

**Kontrol**:
- ScoreText GameObject'ini seçin
- TextMeshPro - Text (UI) component
- Vertex Color → Alpha değeri

**Çözüm**:
- Alpha değerini 255 yapın
- Rengin RGB değerlerini kontrol edin (0,0,0 siyah olur, arka plan siyahsa görünmez)

### 4. **Text Z-Order / Sorting Sorunu**

**Belirti**: Yazı başka bir UI element'in arkasında kalıyor.

**Kontrol**:
- Hierarchy'de ScoreText'in pozisyonu
- Canvas altındaki sıralama

**Çözüm**:
- ScoreText'i Hierarchy'de en alta (en son child) sürükleyin
- Veya Canvas → Additional Settings → Sort Order değerini artırın

### 5. **Text Scale Sıfır**

**Belirti**: Yazı var ama görünmüyor (scale 0).

**Kontrol**:
- ScoreText GameObject → Transform → Scale

**Çözüm**:
- Scale değerlerini (1, 1, 1) yapın
- Kod artık invalid scale'leri otomatik düzeltiyor

### 6. **Text Rect Transform Sorunu**

**Belirti**: Yazı ekran dışında.

**Kontrol**:
- ScoreText → Rect Transform
- Pos X, Pos Y, Width, Height değerleri

**Çözüm**:
- Anchor Presets kullanın (Top-Left, Top-Center, vb.)
- Pos X ve Pos Y'yi sıfırlayın
- Width ve Height'i uygun değerlere ayarlayın (örn: 200x50)

---

## ✅ Doğru Kurulum Kontrol Listesi

### ScoreManager GameObject:
- [ ] GameObject aktif (checkbox işaretli)
- [ ] ScoreManager.cs script ekli
- [ ] Inspector'da "Score Text TMP" alanı DOLU (TextMeshPro atanmış)

### ScoreText (TextMeshPro) GameObject:
- [ ] GameObject aktif (checkbox işaretli)
- [ ] Canvas'ın child'ı
- [ ] TextMeshPro - Text (UI) component ekli
- [ ] Vertex Color → Alpha: 255 (veya 1.0)
- [ ] Vertex Color → RGB: Görünür bir renk (örn: Kırmızı, Beyaz)
- [ ] Rect Transform → Scale: (1, 1, 1)
- [ ] Rect Transform → Position: Ekran içinde

### Canvas:
- [ ] Canvas component ekli
- [ ] Render Mode: "Screen Space - Overlay" (veya Camera atanmış)
- [ ] Canvas Scaler ekli (opsiyonel ama önerilen)

---

## 🎯 Hızlı Test

1. **Play'e basın**
2. **Console'u açın** (Ctrl+Shift+C)
3. **İlk log mesajlarını okuyun**:
   - "TextMeshPro found" görüyor musunuz?
   - "Active: True" mi?
   - "Alpha: 1" mi?
4. **Scene view'da kontrol edin**:
   - ScoreText GameObject seçiliyken "F" tuşuna basın (focus)
   - Yazı görünüyor mu?

---

## 🔧 Manuel Düzeltme Adımları

Eğer hala sorun varsa, şu adımları izleyin:

### Adım 1: Text Component'i Yeniden Oluşturun
1. Hierarchy'de sağ tık → UI → Text - TextMeshPro
2. Adını "ScoreText" yapın
3. Inspector'da:
   - Text: "Score: 0"
   - Font Size: 36
   - Color: Kırmızı (veya istediğiniz renk)
   - Alpha: 255
   - Alignment: Center/Top

### Adım 2: ScoreManager'a Atayın
1. Hierarchy'de GameManager (veya ScoreManager GameObject'i) seçin
2. Inspector'da ScoreManager component'ini bulun
3. "Score Text TMP" alanına ScoreText'i sürükleyin

### Adım 3: Pozisyonu Ayarlayın
1. ScoreText'i seçin
2. Rect Transform → Anchor Presets → Top-Center
3. Pos X: 0, Pos Y: -50
4. Width: 200, Height: 50

### Adım 4: Test Edin
1. Play'e basın
2. Console'da logları kontrol edin
3. "Score: 0" yazısını görmelisiniz

---

## 💡 Ek İpuçları

1. **Scene View vs Game View**: Scene view'da görünüyor ama Game view'da görünmüyorsa, Canvas Render Mode sorunudur.

2. **Multiple Canvases**: Birden fazla Canvas varsa, ScoreText doğru Canvas'ın altında olmalı.

3. **Safe Area**: Mobil cihazlarda Safe Area component'i kullanıyorsanız, text bunun içinde olmalı.

4. **Camera Culling**: Eğer "Screen Space - Camera" kullanıyorsanız, Camera'nın Culling Mask'inde UI layer'ı aktif olmalı.

---

## 🆘 Hala Çalışmıyorsa

Console'daki TÜM log mesajlarını kopyalayıp paylaşın. Özellikle şunları arayın:
- `[ScoreManager]` ile başlayan tüm mesajlar
- Kırmızı hata mesajları
- Sarı uyarı mesajları

Bu bilgilerle sorunu kesin olarak tespit edebiliriz!
