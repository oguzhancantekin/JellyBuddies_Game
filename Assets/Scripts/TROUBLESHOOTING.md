# Jelly Buddies - Star Collection Troubleshooting Guide

## 🔍 Problem: Yıldız toplandığında skor artmıyor

### ✅ Kontrol Listesi

#### 1. **JellyBuddy GameObject Ayarları**
- [ ] `Rigidbody2D` component'i ekli mi?
- [ ] `Collider2D` (CircleCollider2D veya BoxCollider2D) ekli mi?
- [ ] JellyBuddy'nin Collider'ında **"Is Trigger" KAPALI** olmalı (fizik için)
- [ ] `JellyBuddy.cs` script'i ekli mi?
- [ ] Inspector'da "Enable Star Collection" seçeneği **AÇIK** mı?

#### 2. **Star GameObject Ayarları**
- [ ] Star objesinin Tag'i **"Star"** olarak ayarlanmış mı?
  - Unity'de: Inspector → Tag → "Star"
  - Eğer "Star" tag'i yoksa: Tags & Layers → Tags → "+" → "Star" ekleyin
- [ ] Star'da `Collider2D` (CircleCollider2D veya BoxCollider2D) var mı?
- [ ] Star'ın Collider'ında **"Is Trigger" AÇIK** olmalı ✓
- [ ] Star'ın Layer'ı JellyBuddy ile çarpışabilir durumda mı?

#### 3. **ScoreManager Ayarları**
- [ ] Scene'de "GameManager" veya "ScoreManager" adında bir GameObject var mı?
- [ ] Bu GameObject'e `ScoreManager.cs` script'i ekli mi?
- [ ] Canvas'ta bir Text veya TextMeshPro Text var mı?
- [ ] ScoreManager Inspector'ında bu Text component'i atanmış mı?
  - "Score Text TMP" (TextMeshPro için) VEYA
  - "Score Text Legacy" (Legacy Text için)

#### 4. **Physics 2D Settings**
- [ ] Edit → Project Settings → Physics 2D
- [ ] Layer Collision Matrix'te JellyBuddy ve Star layer'ları birbirleriyle çarpışabilir mi?

---

## 🐛 Debug Loglarını Kontrol Edin

Oyunu Play modunda çalıştırıp Console'u açın (Window → General → Console).

### Beklenen Log Mesajları:

#### ✅ Başarılı Toplama:
```
[JellyBuddy] Trigger detected with: Star | Tag: Star
[JellyBuddy] ✓ Star tag matched! Collecting star: Star
[JellyBuddy] CollectStar() called for: Star
[JellyBuddy] ScoreManager found! Adding score...
Score added: +1 | Total: 1
[JellyBuddy] Score added! Current score: 1
[JellyBuddy] Triggering star collection visual effect...
[JellyBuddy] Destroying star: Star
[JellyBuddy] ✓ Star collection complete!
```

#### ❌ Sorun Senaryoları:

**1. Hiç log görünmüyorsa:**
- Trigger çalışmıyor
- Star'ın "Is Trigger" seçeneği AÇIK değil
- JellyBuddy'nin Collider'ı yok
- Layer collision matrix'te çarpışma kapalı

**2. "Tag mismatch" mesajı görünüyorsa:**
```
[JellyBuddy] ✗ Tag mismatch. Expected: 'Star', Got: 'Untagged'
```
- Star objesinin Tag'i "Star" olarak ayarlanmamış

**3. "ScoreManager.Instance is NULL" mesajı görünüyorsa:**
```
[JellyBuddy] ✗ ScoreManager.Instance is NULL!
```
- ScoreManager GameObject'i scene'de yok
- ScoreManager.cs script'i ekli değil
- Script'te hata var ve Awake() çalışmadı

**4. "Star collection is DISABLED" mesajı görünüyorsa:**
```
[JellyBuddy] Star collection is DISABLED in Inspector!
```
- JellyBuddy Inspector'ında "Enable Star Collection" kapalı

---

## 🔧 Hızlı Çözümler

### Çözüm 1: Star Tag'ini Kontrol Et
1. Star GameObject'ini seçin
2. Inspector'ın en üstünde "Tag" dropdown'ını açın
3. "Star" seçin
4. Eğer "Star" yoksa:
   - Tags & Layers → Tags → "+" tıklayın
   - "Star" yazıp kaydedin
   - Tekrar Star GameObject'ine dönüp Tag'i ayarlayın

### Çözüm 2: Collider Ayarlarını Düzelt
**JellyBuddy:**
- Collider2D ekleyin
- "Is Trigger" → KAPALI (unchecked)

**Star:**
- Collider2D ekleyin
- "Is Trigger" → AÇIK (checked) ✓

### Çözüm 3: ScoreManager Kurulumu
1. Hierarchy'de sağ tık → Create Empty
2. Adını "GameManager" yapın
3. ScoreManager.cs script'ini sürükleyin
4. Canvas → UI → Text - TextMeshPro oluşturun
5. GameManager'ı seçin
6. Inspector'da "Score Text TMP" alanına Text'i sürükleyin

### Çözüm 4: Rigidbody2D Ayarları
JellyBuddy'de:
- Body Type: **Dynamic**
- Simulated: **✓ Checked**
- Gravity Scale: 0 (GravityController yönetiyor)

---

## 📊 Test Senaryosu

1. **Play'e basın**
2. **Console'u açın** (Ctrl+Shift+C)
3. **JellyBuddy'yi Star'a doğru hareket ettirin**
4. **Console'da logları kontrol edin**

### Başarılı Test:
- ✓ Trigger log'u görünür
- ✓ Tag matched log'u görünür
- ✓ Score added log'u görünür
- ✓ Star yok olur
- ✓ UI'da skor artar
- ✓ JellyBuddy büyür/parlar

---

## 🎯 Minimum Gereksinimler

### JellyBuddy GameObject:
```
JellyBuddy
├─ Transform
├─ Sprite Renderer (görsel için)
├─ Rigidbody2D (Body Type: Dynamic)
├─ CircleCollider2D (Is Trigger: OFF)
├─ GravityController.cs
└─ JellyBuddy.cs (Enable Star Collection: ON)
```

### Star GameObject:
```
Star
├─ Transform
├─ Sprite Renderer (görsel için)
├─ CircleCollider2D (Is Trigger: ON)
└─ Tag: "Star"
```

### GameManager GameObject:
```
GameManager
└─ ScoreManager.cs
    └─ Score Text TMP: [Canvas/ScoreText reference]
```

---

## 💡 Ek İpuçları

1. **Layer Collision Matrix**: Edit → Project Settings → Physics 2D → Layer Collision Matrix'te tüm layer'lar birbirleriyle çarpışabilir durumda olmalı (varsayılan ayar).

2. **Rigidbody2D Sleeping Mode**: JellyBuddy'nin Rigidbody2D'sinde "Sleeping Mode" → "Never Sleep" yapabilirsiniz.

3. **Multiple Stars**: Birden fazla yıldız varsa, hepsinin Tag'i "Star" olmalı ve hepsinde "Is Trigger" açık olmalı.

4. **Prefab Kullanımı**: Star'ı prefab yaparsanız, bir kez ayarlayıp çoğaltabilirsiniz.

---

## 🆘 Hala Çalışmıyorsa

Console'daki TÜÜM log mesajlarını kontrol edin ve hangi adımda takıldığını belirleyin:
- Trigger algılanıyor mu? → Collider sorunu
- Tag eşleşiyor mu? → Tag sorunu  
- ScoreManager bulunuyor mu? → ScoreManager kurulum sorunu
- Skor artıyor ama UI güncellenmiyor mu? → UI Text reference sorunu
