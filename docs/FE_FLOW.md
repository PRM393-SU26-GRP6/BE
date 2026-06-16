# 📋 FE Flow Giải Thích (Không Technical)

**Dành cho:** Frontend Team  
**Mục đích:** Hiểu User Flow (Step by step user làm gì)  

---

## 🎯 Tóm tắt 1 câu

**Customer đặt sân → thanh toán → Owner thấy tiền trong ví → Owner rút tiền qua ngân hàng.**

---

## 👨‍💼 CUSTOMER Flow

```
1. Vào app → Chọn sân → Chọn slot
   (Cái này đã có rồi)

2. Bấm "Đặt sân" → Tạo booking
   → System báo: "Booking của bạn đã tạo. Chờ chủ sân xác nhận"
   → Status: "Chờ chủ sân xác nhận"

3. Nhận thông báo: "Chủ sân đã chấp nhận booking!"
   → System báo: "Thanh toán coc 50% để giữ sân"
   → Status: "Chờ thanh toán coc"
   → Hiện nút "Thanh toán coc 50%"

4. Bấm "Thanh toán coc 50%"
   → Thấy QR code hoặc form thanh toán
   → Quét QR / Nhập info
   → Thanh toán

5. Thanh toán thành công ✅
   → Status: "Đã coc, chờ thanh toán phần còn lại"
   → Tiền của Owner tăng trong ví

6. Gần ngày đặt sân
   → Thấy nút "Thanh toán phần còn lại"
   → Bấm → Quét QR / Nhập info
   → Thanh toán 50% còn lại

7. Booking hoàn tất ✅
   → Status: "Hoàn tất"
   → Có thể review sân
```

**FE cần thêm:**
- Nút "Thanh toán coc 50%"
- Nút "Thanh toán phần còn lại"
- Hiển thị status booking

---

## 🏪 OWNER Flow

```
1. Vào dashboard → Thấy "Yêu cầu mới: 2"
   → Bấm vào xem danh sách booking chưa xác nhận

2. Xem booking từ Customer A
   → 2 lựa chọn:
      a) Bấm "Chấp nhận"
      b) Bấm "Từ chối" + nhập lý do
   → Chọn chấp nhận

3. Thông báo gửi cho Customer: "Chủ sân đã chấp nhận"
   → Chờ Customer thanh toán
   → (Owner không cần làm gì)

4. Customer thanh toán deposit
   → Owner thấy tiền xuất hiện trong "Ví"
   → Ví tăng: +450,000 VND
   → (Tự động, không cần owner xác nhận)

5. Customer thanh toán phần 2
   → Ví tăng thêm: +450,000 VND
   → Tổng ví: 900,000 VND

6. Owner quyết định rút tiền
   → Vào "Ví của tôi" tab
   → Thấy: "Số dư: 900,000 VND"
   → Bấm "Rút tiền"

7. Điền form rút tiền:
   - Số tiền: 900,000
   - Ngân hàng: Chọn (Vietcombank, TPBank, ...)
   - Số tài khoản: 1234567890
   - Tên chủ tài khoản: Nguyễn Văn A
   → Bấm "Gửi yêu cầu"

8. Yêu cầu gửi thành công
   → Status: "Chờ admin duyệt"
   → Ví vẫn còn 900,000 (chưa trừ)

9. Admin duyệt yêu cầu
   → Owner nhận thông báo: "Yêu cầu rút tiền đã được duyệt!"
   → Ví: 0 VND (đã bị trừ)
   → Status: "Đã duyệt"

10. Admin chuyển khoản thủ công
    → Tiền vào tài khoản ngân hàng của Owner (24-48h)
```

**FE cần thêm:**
- Tab "Ví của tôi"
  - Hiển thị: Số dư ví, lịch sử giao dịch
- Nút "Rút tiền" → Modal form
  - Nhập số tiền
  - Chọn ngân hàng
  - Nhập số tài khoản
  - Nhập tên chủ tài khoản
  - Bấm "Gửi yêu cầu"
- Danh sách yêu cầu rút tiền
  - Status: "Chờ duyệt", "Đã duyệt", "Từ chối"

---

## 🔐 ADMIN Flow

```
1. Vào dashboard → Thấy "Yêu cầu rút tiền: 5"
   → Bấm vào tab "Yêu cầu rút tiền"

2. Thấy danh sách:
   - Owner A | 900k | Vietcombank | 1234567890 | Chờ duyệt
   - Owner B | 1.5M | TPBank | 0987654321 | Chờ duyệt
   - ...

3. Bấm vào Owner A request
   → Thấy chi tiết:
     * Tên Owner: Nguyễn Văn A
     * Số tiền: 900,000 VND
     * Ngân hàng: Vietcombank
     * Số tài khoản: 1234567890
     * Tên tài khoản: NGUYEN VAN A

4. Admin có 2 lựa chọn:

   a) DUYỆT:
      - Bấm "Duyệt"
      - Status: "Đã duyệt"
      - Owner nhận notification
      - Admin phải tự chuyển khoản qua ngân hàng
      - Sau khi chuyển xong → request hoàn thành

   b) TỪ CHỐI:
      - Bấm "Từ chối"
      - Nhập lý do: "Số tài khoản sai"
      - Bấm "Xác nhận"
      - Status: "Từ chối"
      - Tiền quay lại ví Owner
      - Owner nhận notification + lý do từ chối

5. Owner có thể:
   - Nếu duyệt: Chờ tiền vào bank (24-48h)
   - Nếu từ chối: Rút tiền lại với thông tin đúng
```

**FE cần thêm:**
- Tab "Yêu cầu rút tiền" trong admin dashboard
- Danh sách requests với status filter
- Nút "Duyệt" / "Từ chối"
- Modal chi tiết request

---

## 💰 Tiền Tệ - Công Thức

```
Customer thanh toán TOTAL: 1,000,000 VND

├─ Coc (50%): 500,000 VND
│  ├─ Owner nhận: 450,000 VND (90%)
│  └─ Admin giữ: 50,000 VND (10% fee)
│
└─ Phần còn (50%): 500,000 VND
   ├─ Owner nhận: 450,000 VND (90%)
   └─ Admin giữ: 50,000 VND (10% fee)

TỔNG OWNER WALLET: 900,000 VND
TỔNG ADMIN FEE: 100,000 VND
```

---

## 📋 FE Cần Thêm Cái Gì?

### **Customer:**
1. Nút "Thanh toán coc 50%" (trên booking detail)
2. Nút "Thanh toán phần còn lại"
3. Hiển thị status booking rõ ràng

### **Owner:**
1. Tab "Ví của tôi"
   - Số dư ví
   - Lịch sử giao dịch (xem tiền đến từ booking nào)
2. Nút "Rút tiền" → Form:
   - Nhập số tiền
   - Chọn ngân hàng
   - Nhập số tài khoản
   - Nhập tên chủ tài khoản
3. Danh sách yêu cầu rút tiền:
   - Status: Chờ duyệt / Đã duyệt / Từ chối

### **Admin:**
1. Tab "Yêu cầu rút tiền"
   - Danh sách tất cả requests
   - Filter: Chờ duyệt / Đã duyệt / Từ chối
2. Bấm vào request → Thấy chi tiết
3. Nút "Duyệt" → Admin chuyển khoản thủ công
4. Nút "Từ chối" → Nhập lý do

---

## ⚠️ Điều cần biết

**Customer:**
- Phải thanh toán coc trước, mới giữ được sân
- Nếu hủy booking sau khi coc → Hoàn tiền

**Owner:**
- Tiền trong ví là tiền ảo, chưa là tiền thật
- Phải "Rút tiền" để convert thành tiền thật
- Sau khi rút, chờ admin duyệt
- Sau khi duyệt, tiền về bank trong 24-48h
- Không được rút tiền nếu ví không đủ

**Admin:**
- Bấm "Duyệt" chỉ là thay đổi status trong app
- Phải tự chuyển khoản thủ công qua ngân hàng
- Không có tự động rút tiền từ app

**Tóm tắt (Tao viết chứ đéo phải AI)**
- Là sau khi người dùng thanh toán xong xuôi rồi , thì tiền sẽ vào ví của owner nên là mỗi owner đều phải có trường để hiển thị , sau đó khi mà owner muốn rút tiền thì gửi yêu cầu lên admin , còn các phần như là deposit hay là tiền mặt thì sẽ không liên quan đến cái ví (Wallet) này , vì ở đây , mình làm việc ở API là ví = một cái prop trong API là WalletBalance để mà có thể rút tiền á.