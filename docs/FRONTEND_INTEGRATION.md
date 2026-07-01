# Court Manager - Tài liệu tích hợp Frontend

Tài liệu này được tạo từ source code backend hiện tại trong các project `CourtManager.APIs`, `CourtManager.Application`, `CourtManager.Domain`, `CourtManager.Infrastructure`.

Ghi chú quan trọng:

- Chỉ liệt kê endpoint thật sự được expose qua Controller/Hub.
- Các method có `[NonAction]` không phải API endpoint và được ghi riêng là "không expose".
- `AdminController` có 2 endpoint đang trả data hardcoded, được đánh dấu `Incomplete`.
- Build kiểm tra ngày 2026-06-04 bị fail do file DLL đang bị khóa bởi process `CourtManager.APIs (20192)` và Visual Studio, không phải lỗi compile source. Có warning `ReviewsController.cs` biến `ex` khai báo nhưng không dùng.

## SECTION 1 - TỔNG QUAN HỆ THỐNG

### Tên project

Court Manager API.

### Mục đích hệ thống

Hệ thống quản lý đặt sân thể thao/bóng đá, gồm: tìm sân, xem venue/field/slot, đặt lịch, thanh toán cọc/thanh toán phần còn lại, owner quản lý venue/field/slot/booking/discount, chat realtime, notification realtime, review sau khi booking hoàn tất.

### Module chính

- Authentication: đăng ký, đăng nhập, refresh token, logout.
- Venue discovery: danh sách venue, chi tiết venue, field, tiện ích, hình ảnh, search, map nearby.
- Field/slot: xem field, xem slot, lock/unlock slot, owner tạo/sửa/xóa slot.
- Booking: tạo booking từ slot, lịch sử booking, owner accept/reject/complete, customer cancel.
- Payment: deposit, final payment, refund, gateway callback, SePay webhook, SePay QR, SePay checkout payload.
- Chat: chat room, message, mark read, SignalR chat hub.
- Notification: danh sách notification, unread count, mark read, SignalR notification hub.
- Review: review venue/booking, xem review, average rating.
- Owner management: venue, image, amenity, field, slot, booking, discount, revenue, stats.
- Admin: stats/courts/test role, trong đó stats/courts đang hardcoded.

### Vai trò người dùng

Role có trong code/seed/config:

- `Guest`: enum có khai báo, nhưng không được seed trong `ApplicationDbContext`.
- `User`: user đặt sân.
- `Owner`: chủ venue/sân.
- `Admin`: quản trị.

### Mô hình phân quyền

- JWT Bearer auth được cấu hình trong `ApiServiceExtensions`.
- Header chuẩn: `Authorization: Bearer <accessToken>`.
- Hub SignalR chấp nhận token qua `access_token` query khi path bắt đầu bằng `/hubs`.
- Role authorization dùng `[Authorize(Roles = "...")]`.
- Resource authorization được xử lý trong handler/controller, ví dụ: user chỉ xem booking/payment của mình; owner chỉ quản lý venue/field/slot/discount của mình.
- `RequireRoleAttribute` tồn tại nhưng không thấy được dùng làm authorization middleware/action filter.

## LUỒNG TRUY CẬP APP THỰC TẾ CHO FRONTEND

Phần này viết theo cách người dùng đang mở app thật. Mỗi luồng ghi rõ màn hình đang đứng ở đâu, frontend gọi API nào, lấy dữ liệu gì, sau đó đi tiếp sang màn hình nào.

### 1. Khách mở app và xem danh sách sân

Mục tiêu: người chưa đăng nhập vẫn xem được danh sách venue/sân.

Người dùng mở app vào trang chủ hoặc trang danh sách sân.

Frontend cần làm:

1. Gọi `GET /api/v1/Venues?Page=1&PageSize=10`.
2. Backend trả wrapper:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "items": [],
    "page": 1,
    "pageSize": 10,
    "totalItems": 0,
    "totalPages": 0
  },
  "errors": []
}
```

3. Frontend đọc danh sách venue ở `data.items`.
4. Hiển thị card venue gồm: `venueName`, `address`, `averageRating`, `totalReviews`, `minPrice`, `maxPrice`, `openingHours`, `phoneContact`.
5. Nếu người dùng cuộn xuống hoặc bấm trang tiếp theo, tăng `Page` và gọi lại API.
6. Nếu muốn lấy toàn bộ venue một lần, frontend gọi nhiều page cho đến khi `page >= totalPages`.

Màn hình tiếp theo:

- Bấm vào một venue thì đi tới trang chi tiết venue.
- Bấm tìm kiếm thì gọi luồng search venue.
- Bấm map thì gọi luồng map nearby.

API liên quan:

- `GET /api/v1/Venues`
- `GET /api/v1/Venues/search`
- `GET /api/v1/Venues/map/nearby`

Chi tiết triển khai frontend theo từng bước:

1. Khởi tạo màn hình danh sách sân.
   - Frontend tạo state ban đầu: `page = 1`, `pageSize = 10`, `q = ""`, `sort = ""`, `filters = {}`.
   - Nếu người dùng chưa đăng nhập, vẫn cho vào màn hình này vì venue, field, slot và review public đều cho đọc không cần Bearer token.
   - Hiển thị loading skeleton hoặc spinner cho khu vực danh sách venue.

2. Gọi danh sách venue lần đầu.
   - Gọi `GET /api/v1/Venues?Page=1&PageSize=10`.
   - Đọc response theo wrapper: `data.items`, `data.page`, `data.pageSize`, `data.totalItems`, `data.totalPages`.
   - Nếu `data.items` rỗng, hiển thị empty state như "Chưa có sân phù hợp".
   - Nếu có dữ liệu, render mỗi venue thành card.

3. Render card venue.
   - Mỗi card dùng các field chính: `venueId`, `venueName`, `address`, `averageRating`, `totalReviews`, `minPrice`, `maxPrice`, `openingHours`, `phoneContact`.
   - Nếu card cần ảnh đại diện nhưng list venue không có image, frontend hiển thị placeholder trước hoặc chỉ gọi image khi mở detail.
   - Card cần lưu `venueId` để điều hướng sang detail.

4. Phân trang hoặc infinite scroll.
   - Khi user bấm trang tiếp theo hoặc scroll tới cuối, tăng `Page`.
   - Gọi lại `GET /api/v1/Venues?Page=<nextPage>&PageSize=<pageSize>`.
   - Với pagination thường: thay danh sách hiện tại bằng page mới.
   - Với infinite scroll: append `data.items` vào danh sách hiện tại.
   - Dừng gọi thêm khi `data.page >= data.totalPages`.

5. Tìm kiếm nhanh theo từ khóa.
   - Khi user nhập từ khóa, frontend debounce khoảng 300-500ms.
   - Gọi `GET /api/v1/Venues/search?q=<keyword>&page=1&pageSize=10`.
   - Reset page về 1 khi keyword thay đổi.
   - Render lại danh sách từ `data.items`.
   - Nếu keyword bị xóa rỗng, quay lại gọi `GET /api/v1/Venues?Page=1&PageSize=10`.

6. Lọc nâng cao trên danh sách venue.
   - Nếu UI có filter theo loại sân, tiện ích, rating, giá, sort, frontend gọi `GET /api/v1/Venues` với query tương ứng.
   - Ví dụ: `GET /api/v1/Venues?Q=saigon&FieldType=FiveASide&MinRating=4&PriceMin=100000&PriceMax=300000&Page=1&PageSize=10`.
   - Mỗi lần đổi filter nên reset `Page = 1`.
   - Nên lưu filter vào URL query của frontend để reload trang không mất filter.

7. Xem venue gần vị trí hiện tại.
   - Khi user bấm tab/map view, frontend xin quyền lấy vị trí hoặc cho user chọn điểm trên map.
   - Gọi `GET /api/v1/Venues/map/nearby?lat=<lat>&lng=<lng>&radius=<radiusKm>`.
   - Response là wrapper có `data` dạng danh sách `VenueDto`.
   - Render marker bằng `latitude`, `longitude`, label bằng `venueName`, phụ đề bằng `distance` nếu backend trả.
   - Khi user bấm marker, mở preview card hoặc điều hướng tới detail venue.

8. Mở chi tiết venue.
   - Khi user bấm card/marker, frontend điều hướng tới `/venues/{venueId}`.
   - Gọi song song các API public:
     - `GET /api/v1/Venues/{id}`
     - `GET /api/v1/Venues/{id}/fields`
     - `GET /api/v1/Venues/{id}/amenities`
     - `GET /api/v1/Venues/{id}/images`
     - `GET /api/v1/reviews/venue/{id}?page=1&pageSize=10`
   - Venue detail dùng `data` từ `GET /api/v1/Venues/{id}`.
   - Fields, amenities, images đọc từ `data` của từng wrapper.
   - Review section đọc từ `data.reviews`, `data.averageRating`, `data.totalCount`, `data.page`, `data.pageSize`.

9. Render chi tiết venue.
   - Header: `venueName`, `address`, `averageRating`, `totalReviews`.
   - Body: `description`, `openingHours`, `phoneContact`, gallery images, amenities.
   - Field list: render từng field với `id`, `fieldName`, `fieldType`, `pricePerHour`, `isActive`.
   - Field inactive nên disable nút chọn slot hoặc hiển thị trạng thái không nhận đặt.

10. Xem slot của field.
    - Khi user chọn field và chọn ngày, gọi một trong hai endpoint:
      - `GET /api/v1/fields/{fieldId}/slots?date=<YYYY-MM-DD>`
      - hoặc `GET /api/v1/slots/available?fieldId=<fieldId>&date=<YYYY-MM-DD>`
    - Response wrapper chứa danh sách `TimeSlotDto`.
    - Render slot theo `slotStatus`: `Available` cho chọn, `Locked` hiển thị đang được giữ, `Booked` hiển thị đã đặt.
    - Với khách chưa đăng nhập, vẫn cho xem slot nhưng khi bấm chọn slot thì chuyển sang login trước khi lock.

11. Xem field detail riêng nếu cần.
    - Nếu UI có màn hình field detail riêng, gọi `GET /api/v1/fields/{id}`.
    - Dùng response `data` để render `fieldName`, `fieldType`, `pricePerHour`, `description`, `isActive`.
    - Sau đó gọi slot endpoint theo ngày như bước 10.

12. Xem review venue.
    - Review có thể load cùng lúc với detail venue hoặc lazy-load khi user mở tab Review.
    - Gọi `GET /api/v1/reviews/venue/{venueId}?page=1&pageSize=10`.
    - Render `reviews`, `averageRating`, `totalCount`.
    - Nếu hỗ trợ phân trang review, tăng `page` và gọi lại endpoint này.

13. Điều hướng tiếp theo từ màn hình public.
    - Bấm "Chọn sân/Đặt lịch": nếu đã chọn field và slot thì chuyển sang checkout; nếu chưa login thì chuyển sang login rồi quay lại màn hình slot.
    - Bấm "Chat với chủ sân": nếu chưa login thì chuyển sang login; nếu đã login thì gọi flow chat `POST /api/v1/chats/rooms`.
    - Bấm "Xem review": scroll tới section review hoặc mở tab review.

14. Xử lý lỗi trên màn hình public.
    - `404` ở `GET /api/v1/Venues/{id}`: hiển thị trang không tìm thấy venue.
    - `400` ở nearby/search/filter: hiển thị lỗi filter hoặc vị trí không hợp lệ.
    - Network/server error: giữ lại filter hiện tại, cho user bấm thử lại.
    - Các API public không cần Bearer token; nếu frontend lỡ gửi token hết hạn và gặp 401, có thể retry request public không kèm token.

15. Kết quả mong đợi sau flow.
    - Guest xem được danh sách venue.
    - Guest tìm kiếm/lọc/xem map được venue.
    - Guest mở được venue detail, xem fields, amenities, images, reviews.
    - Guest xem được slot theo ngày và biết slot nào có thể chọn.
    - Khi cần lock slot, booking hoặc chat, frontend chuyển người dùng sang login vì các hành động đó cần Bearer token.

### 2. Khách tìm kiếm, lọc và xem venue gần vị trí hiện tại

Mục tiêu: người dùng lọc venue theo từ khóa, loại sân, tiện ích, rating, giá hoặc vị trí.

Khi người dùng nhập ô tìm kiếm:

1. Gọi `GET /api/v1/Venues/search?q=<keyword>&page=1&pageSize=10`.
2. Backend vẫn trả `PagedResult<VenueDto>` trong `data`.
3. Frontend render lại danh sách từ `data.items`.

Khi người dùng dùng bộ lọc nâng cao:

1. Gọi `GET /api/v1/Venues?Q=...&FieldType=FiveASide&AmenityIds=...&MinRating=...&PriceMin=...&PriceMax=...&Page=1&PageSize=10`.
2. Frontend giữ state filter trên URL/query của app để reload không mất bộ lọc.

Khi người dùng mở màn hình bản đồ:

1. Lấy lat/lng từ browser hoặc vị trí người dùng chọn trên map.
2. Gọi `GET /api/v1/Venues/map/nearby?lat=<lat>&lng=<lng>&radius=5`.
3. Backend trả wrapper với `VenueDto[]`.
4. Frontend vẽ marker theo `latitude`, `longitude`.

Màn hình tiếp theo:

- Bấm marker hoặc venue card thì đi tới trang chi tiết venue.

### 3. Người dùng xem chi tiết venue

Mục tiêu: xem thông tin đầy đủ của một venue trước khi chọn sân và slot.

Người dùng đang ở trang `/venues/{venueId}`.

Frontend cần gọi:

1. `GET /api/v1/Venues/{id}` để lấy thông tin chi tiết venue.
2. `GET /api/v1/Venues/{id}/fields` để lấy danh sách sân con.
3. `GET /api/v1/Venues/{id}/amenities` để lấy tiện ích.
4. `GET /api/v1/Venues/{id}/images` để lấy ảnh.
5. `GET /api/v1/reviews/venue/{id}?page=1&pageSize=10` để lấy review.

Frontend hiển thị:

- Thông tin venue: tên, địa chỉ, mô tả, giờ mở cửa, số điện thoại.
- Gallery ảnh.
- Tiện ích.
- Danh sách field.
- Review và rating trung bình.

Màn hình tiếp theo:

- Bấm field thì đi tới màn hình chọn ngày/slot.
- Bấm chat với chủ sân thì nếu chưa login, chuyển qua login; nếu đã login, gọi luồng chat.

### 4. Người dùng đăng ký, đăng nhập và giữ phiên

Mục tiêu: lấy token để dùng các chức năng cần tài khoản.

Luồng đăng ký:

1. Người dùng mở màn hình đăng ký.
2. Frontend gửi `POST /api/v1/auth/register`.
3. Body gồm `fullName`, `email`, `phoneNumber`, `password`, `confirmPassword`.
4. Nếu thành công, backend trả `accessToken`, `refreshToken`, `user`.
5. Frontend lưu token và chuyển người dùng về màn hình trước đó hoặc trang chủ.

Luồng đăng nhập:

1. Người dùng mở màn hình đăng nhập.
2. Frontend gửi `POST /api/v1/auth/login`.
3. Nếu `success=true`, lưu `accessToken`, `refreshToken`, `user`.
4. Từ lúc này mọi API cần đăng nhập phải gửi `Authorization: Bearer <accessToken>`.

Luồng refresh token:

1. Khi API trả 401 do access token hết hạn, frontend gọi `POST /api/v1/auth/refresh-token`.
2. Header vẫn gửi access token cũ: `Authorization: Bearer <oldAccessToken>`.
3. Body gửi `refreshToken`.
4. Nếu refresh thành công, thay token cũ bằng token mới rồi gọi lại request ban đầu.
5. Nếu refresh thất bại, xóa token và đưa user về login.

Luồng logout:

1. Người dùng bấm logout.
2. Frontend gọi `POST /api/v1/auth/logout`.
3. Backend xóa refresh token.
4. Frontend xóa token local và quay về màn hình public.

### 5. Người dùng chọn slot và tạo booking

Mục tiêu: đặt lịch sân.

Người dùng đang ở màn hình field detail hoặc booking calendar.

Frontend cần làm:

1. Gọi `GET /api/v1/fields/{id}/slots?date=<date>` hoặc `GET /api/v1/slots/available?fieldId=<fieldId>&date=<date>`.
2. Render các slot theo `slotStatus`.
3. Người dùng chọn một hoặc nhiều slot.
4. Nếu chưa login, chuyển sang login trước.
5. Sau khi login, gọi `POST /api/v1/slots/{slotId}/lock` cho từng slot cần giữ.
6. Slot được lock trong 15 phút.
7. Nếu người dùng nhập mã giảm giá, gọi `POST /api/v1/discounts/validate`.
8. Khi người dùng xác nhận đặt sân, gọi `POST /api/v1/bookings`.

Body tạo booking:

```json
{
  "slotIds": ["slot-id-1", "slot-id-2"],
  "discountCode": "RIVER20",
  "note": "Cần thuê bóng"
}
```

Backend xử lý:

- Lấy `UserId` từ JWT, frontend không gửi `userId`.
- Kiểm tra slot không ở quá khứ.
- Kiểm tra tất cả slot thuộc cùng một venue.
- Kiểm tra slot còn available hoặc đang lock bởi chính user.
- Tính giảm giá nếu có.
- Tạo booking status `Pending`.
- Tạo notification cho owner.

Màn hình tiếp theo:

- Sau khi tạo booking thành công, chuyển tới trang chi tiết booking hoặc lịch sử booking.
- Hiển thị trạng thái: đang chờ chủ sân xác nhận.

### 6. Chủ sân duyệt hoặc từ chối booking

Mục tiêu: owner xử lý booking mới của venue mình.

Owner đăng nhập vào dashboard.

Frontend cần làm:

1. Gọi `GET /api/v1/owner/bookings/pending`.
2. Render danh sách booking đang chờ.
3. Owner mở chi tiết booking bằng `GET /api/v1/owner/bookings/{id}`.
4. Nếu đồng ý, gọi `PUT /api/v1/owner/bookings/{id}/accept`.
5. Nếu từ chối, gọi `PUT /api/v1/owner/bookings/{id}/reject?rejectionReason=<reason>`.

Backend xử lý:

- Chỉ owner của venue trong booking mới được accept/reject.
- Booking phải đang `Pending`.
- Accept thì booking chuyển sang `Accepted`.
- Reject thì booking chuyển sang `Rejected`, slot được trả về `Available`.

Màn hình tiếp theo:

- Nếu accept, customer có thể thanh toán cọc.
- Nếu reject, customer thấy booking bị từ chối trong lịch sử booking.

### 7. Người dùng thanh toán cọc và thanh toán phần còn lại

Mục tiêu: hoàn tất tiền đặt sân.

Sau khi booking được owner accept:

1. Customer mở màn hình booking detail.
2. Frontend thấy `bookingStatus = Accepted`.
3. Hiển thị nút thanh toán cọc.
4. Khi bấm thanh toán cọc, gọi `POST /api/v1/payments/deposit`.

Body:

```json
{
  "bookingId": "booking-id",
  "paymentMethod": "SePay",
  "transactionCode": null
}
```

Nếu chọn SePay:

1. Backend tạo payment `Pending`.
2. Frontend gọi `GET /api/v1/payments/{paymentId}/sepay-qr`.
3. Hiển thị QR, số tiền, nội dung chuyển khoản.
4. Nội dung chuyển khoản có dạng `CM{transactionCode}`.
5. Khi SePay webhook xác nhận đúng tiền, backend chuyển payment sang `Success`, booking sang `Deposited`, slot sang `Booked`.

Nếu chọn Cash:

1. Backend tạo payment `Success` ngay.
2. Booking chuyển sang `Deposited`.

Thanh toán phần còn lại:

1. Khi booking đã `Deposited`, frontend hiển thị nút thanh toán phần còn lại.
2. Gọi `POST /api/v1/payments/final`.
3. Nếu payment success, booking chuyển sang `Completed`.
4. User được cộng loyalty point theo rule trong handler.

Màn hình tiếp theo:

- Nếu booking `Completed`, frontend cho phép user review.

### 8. Người dùng chat với chủ sân realtime

Mục tiêu: người dùng hỏi chủ sân và thấy tin nhắn realtime.

Người dùng bấm nút chat ở venue detail.

Frontend cần làm:

1. Nếu chưa login, chuyển sang login.
2. Gọi `POST /api/v1/chats/rooms` với body:

```json
{
  "venueId": "venue-id"
}
```

3. Backend tạo hoặc lấy chat room giữa user và owner của venue.
4. Gọi `GET /api/v1/chats/rooms/{roomId}/messages` để lấy lịch sử tin nhắn.
5. Kết nối SignalR tới `/hubs/chat`.
6. Gọi hub method `JoinRoom(roomId)`.
7. Khi gửi tin nhắn, frontend có thể gọi REST `POST /api/v1/chats/rooms/{roomId}/messages` hoặc hub method `SendMessage(roomId, messageText)`.
8. Frontend lắng nghe:
   - `chat.messageCreated`
   - `chat.roomUpdated`
   - `chat.messagesRead`
   - `chat.error`

Backend xử lý:

- Chỉ customer hoặc host trong room được đọc/gửi tin.
- Tin nhắn rỗng bị reject.
- Khi có tin nhắn mới, backend tạo notification cho người nhận.

Màn hình tiếp theo:

- Badge notification tăng realtime.
- Chat list cập nhật last message realtime.

### 9. Người dùng nhận notification realtime

Mục tiêu: app hiển thị thông báo và số lượng chưa đọc.

Khi user đã đăng nhập:

1. Frontend gọi `GET /api/v1/notifications`.
2. Frontend gọi `GET /api/v1/notifications/unread-count`.
3. Kết nối SignalR tới `/hubs/notifications`.
4. Lắng nghe:
   - `notification.created`
   - `notification.read`
   - `notification.readAll`
   - `notification.unreadCountChanged`
   - `notification.error`

Khi user bấm một notification:

1. Gọi `PUT /api/v1/notifications/{id}/read`.
2. Cập nhật notification đó thành đã đọc.
3. Cập nhật unread badge theo event hoặc response.

Khi user bấm đánh dấu tất cả đã đọc:

1. Gọi `PUT /api/v1/notifications/read-all`.
2. Set toàn bộ notification trong UI thành đã đọc.
3. Badge unread về 0 nếu backend trả/event báo 0.

Màn hình tiếp theo:

- Nếu notification type là booking/chat/payment, frontend có thể điều hướng tới màn hình tương ứng dựa trên `type` và `refId`.

### 10. Người dùng review sau khi hoàn tất booking

Mục tiêu: chỉ booking đã hoàn thành mới được đánh giá.

Frontend đang ở booking detail.

1. Gọi `GET /api/v1/bookings/{id}/review`.
2. Nếu `data = null` và booking đã `Completed`, hiển thị nút viết review.
3. Khi submit, gọi `POST /api/v1/reviews`.

Body:

```json
{
  "venueId": "venue-id",
  "bookingId": "booking-id",
  "rating": 5,
  "comment": "Sân tốt, nhân viên hỗ trợ nhanh"
}
```

Backend xử lý:

- Rating phải từ 1 đến 5.
- Booking phải thuộc user hiện tại.
- Booking phải `Completed`.
- Một booking chỉ được review một lần.

Màn hình tiếp theo:

- Sau khi review thành công, reload review section của venue.
- Màn hình booking detail hiển thị review đã gửi.

### 11. Owner quản lý venue, field và slot

Mục tiêu: chủ sân tạo và vận hành sân.

Owner mở dashboard.

Frontend cần làm:

1. Gọi `GET /api/v1/owner/stats` để hiển thị tổng quan.
2. Gọi `GET /api/v1/owner/revenue?groupBy=day` để hiển thị doanh thu.
3. Gọi `GET /api/v1/owner/venues` để hiển thị venue của owner.

Owner tạo venue:

1. Mở form tạo venue.
2. Gọi `POST /api/v1/owner/venues`.
3. Sau khi tạo, chuyển tới màn hình venue management detail.

Owner upload ảnh:

1. Chọn ảnh.
2. Gọi `POST /api/v1/owner/venues/{id}/images`.
3. Body là multipart form field `images`.
4. Backend trả list URL ảnh.

Owner tạo field:

1. Gọi `POST /api/v1/owner/venues/{venueId}/fields`.
2. Sau khi tạo field, chuyển tới màn hình tạo slot.

Owner tạo slot hàng loạt:

1. Gọi `POST /api/v1/owner/fields/{id}/slots/bulk`.
2. Body gồm `fromDate`, `toDate`, `startTime`, `endTime`, `slotDurationMinutes`, `price`.
3. Backend trả `createdSlots`.

Owner bật/tắt venue:

1. Gọi `PUT /api/v1/owner/venues/{id}/status`.
2. Nếu còn booking active, backend trả lỗi và không cho deactivate.

### 12. Owner quản lý mã giảm giá

Mục tiêu: owner tạo mã giảm giá cho người đặt sân.

Frontend cần làm:

1. Gọi `GET /api/v1/owner/discounts`.
2. Hiển thị danh sách discount.
3. Tạo discount bằng `POST /api/v1/owner/discounts`.
4. Sửa discount bằng `PUT /api/v1/owner/discounts/{id}`.
5. Bật/tắt discount bằng `PUT /api/v1/owner/discounts/{id}/status`.
6. Xóa discount bằng `DELETE /api/v1/owner/discounts/{id}`.

Rule backend:

- `code` bắt buộc.
- `value` phải lớn hơn 0.
- `endDate` phải sau `startDate`.
- `usageLimit` không được âm.
- `discountType` chỉ nhận `Percentage` hoặc `Fixed`.

### 13. Admin diagnostic

Mục tiêu: kiểm tra quyền admin hoặc placeholder endpoint.

Frontend chỉ nên dùng các endpoint này cho màn hình dev/admin đơn giản:

1. `GET /api/Admin/stats`: Incomplete, trả hardcoded `totalUsers`, `totalBookings`.
2. `GET /api/Admin/courts`: Incomplete, trả hardcoded courts.
3. `POST /api/Admin/test-role`: chỉ OK nếu role claim là `Admin`.

Không nên xây dashboard production dựa vào `/api/Admin/stats` và `/api/Admin/courts` vì source hiện tại chưa lấy dữ liệu thật từ database.

## SECTION 2 - LUỒNG NGHIỆP VỤ

### Khách xem danh sách sân

Business Goal: khách chưa đăng nhập có thể xem sân và slot.

Actors: Guest.

Preconditions: API đang chạy.

Step-by-Step Flow:

1. GET `/api/v1/Venues` để lấy danh sách venue.
2. GET `/api/v1/Venues/search?q=...` nếu cần tìm kiếm.
3. GET `/api/v1/Venues/map/nearby?lat=...&lng=...&radius=...` nếu cần map.
4. GET `/api/v1/Venues/{id}` để xem chi tiết.
5. GET `/api/v1/Venues/{id}/fields`, `/amenities`, `/images`.
6. GET `/api/v1/fields/{id}` va GET `/api/v1/fields/{id}/slots?date=...`.
7. GET `/api/v1/reviews/venue/{id}` để xem review.

Expected Result: frontend hiển thị venue, field, slot available và review.

Related APIs: Venue, Field, Slot, Review public endpoints.

### Đăng ký và đăng nhập

Business Goal: user tạo tài khoản và nhận access/refresh token.

Actors: Guest, User.

Preconditions: email/phone chưa tồn tại.

Step-by-Step Flow:

1. POST `/api/v1/auth/register`.
2. Backend tạo user active, gán role mặc định `User`.
3. Backend trả `accessToken`, `refreshToken`, `user`.
4. Hoặc POST `/api/v1/auth/login` với email/password.

Expected Result: frontend lưu token và user info.

Related APIs: Auth endpoints.

### Refresh token và logout

Business Goal: duy trì session và đăng xuất.

Actors: User, Owner, Admin.

Preconditions: đã có access token và refresh token.

Step-by-Step Flow:

1. Khi access token hết hạn, POST `/api/v1/auth/refresh-token`.
2. Gửi access token cũ trong header `Authorization: Bearer ...`.
3. Gửi `refreshToken` trong body.
4. Backend cấp cặp token mới.
5. Khi logout, POST `/api/v1/auth/logout`.
6. Backend xóa refresh token của user.

Expected Result: frontend thay token mới hoặc logout về trang login.

Related APIs: Auth endpoints.

### Customer tạo booking

Business Goal: user đặt một hoặc nhiều slot trong cùng venue.

Actors: User.

Preconditions: đã login; slot còn `Available` hoặc locked bởi chính user.

Step-by-Step Flow:

1. Xem venue/field/slot public.
2. POST `/api/v1/slots/{id}/lock` để giữ slot 15 phút.
3. POST `/api/v1/discounts/validate` nếu có mã giảm giá.
4. POST `/api/v1/bookings` với `slotIds`, `discountCode`, `note`.
5. Backend set booking `Pending`, khóa slot 15 phút, tạo notification cho owner.
6. User xem lịch sử tại GET `/api/v1/bookings/history`.

Expected Result: booking được tạo với status `Pending`.

Related APIs: Slots, Discounts, Bookings, Notifications.

### Owner duyệt booking

Business Goal: owner xác nhận hoặc từ chối booking.

Actors: Owner, Admin với owner routes.

Preconditions: booking thuộc venue của owner và đang `Pending`.

Step-by-Step Flow:

1. GET `/api/v1/owner/bookings/pending`.
2. GET `/api/v1/owner/bookings/{id}` nếu cần xem chi tiết.
3. PUT `/api/v1/owner/bookings/{id}/accept` để chuyển sang `Accepted`.
4. Hoặc PUT `/api/v1/owner/bookings/{id}/reject?rejectionReason=...` để chuyển sang `Rejected` và trả slot về `Available`.

Expected Result: booking được accept/reject.

Related APIs: Owner booking endpoints.

### Customer Payment

Business Goal: user thanh toán cọc sau khi owner accept, sau đó thanh toán phần còn lại.

Actors: User, Owner, Admin.

Preconditions: booking status `Accepted` cho deposit; booking status `Deposited` cho final payment.

Step-by-Step Flow:

1. POST `/api/v1/payments/deposit` với `bookingId`, `paymentMethod`, `transactionCode`.
2. Nếu `paymentMethod = Cash`, backend set payment `Success`, booking `Deposited`, slot `Booked`.
3. Nếu SePay/VNPay/MoMo, payment tạo ở `Pending`.
4. GET `/api/v1/payments/{paymentId}/sepay-qr`.
5. Payment gateway callback/webhook xac nhan payment.
6. POST `/api/v1/payments/final` sau khi booking `Deposited`.

Expected Result: payment được tạo/cập nhật, booking sang `Deposited` hoặc `Completed`.

Related APIs: Payments.

### Cancel Booking

Business Goal: user huy booking.

Actors: User.

Preconditions: booking của chính user; status `Pending` hoặc `Accepted`.

Step-by-Step Flow:

1. PUT `/api/v1/bookings/{id}/cancel?cancellationReason=...`.
2. Backend set booking `Cancelled`, trả slot về `Available`, tạo notification cho owner.

Expected Result: booking bi huy.

Related APIs: Bookings, Notifications.

### Review After Completed Booking

Business Goal: user đánh giá venue sau booking.

Actors: User.

Preconditions: booking cua user da `Completed`, chua co review.

Step-by-Step Flow:

1. GET `/api/v1/bookings/{id}/review` để kiểm tra đã review chưa.
2. POST `/api/v1/reviews` với `venueId`, `bookingId`, `rating`, `comment`.
3. GET `/api/v1/reviews/my-reviews` để xem review của mình.
4. PUT `/api/v1/reviews/{id}` để sửa review của chính mình.

Expected Result: review được tạo/cập nhật, rating 1-5.

Related APIs: Reviews, Bookings.

### Chat Realtime

Business Goal: user va owner chat realtime.

Actors: User, Owner.

Preconditions: đã login.

Step-by-Step Flow:

1. POST `/api/v1/chats/rooms` với `venueId` hoặc `customerId`/`ownerId`.
2. GET `/api/v1/chats/rooms/{roomId}/messages`.
3. Connect SignalR `/hubs/chat`.
4. Hub `JoinRoom(roomId)`.
5. Gửi message bằng REST POST `/api/v1/chats/rooms/{roomId}/messages` hoặc Hub `SendMessage(roomId,messageText)`.
6. Lang nghe event `chat.messageCreated`, `chat.roomUpdated`, `chat.messagesRead`, `chat.error`.

Expected Result: màn hình chat cập nhật realtime.

Related APIs: Chats, ChatHub, Notifications.

### Notification Realtime

Business Goal: user nhan notification va unread count realtime.

Actors: User, Owner, Admin.

Preconditions: đã login.

Step-by-Step Flow:

1. GET `/api/v1/notifications`.
2. GET `/api/v1/notifications/unread-count`.
3. Connect SignalR `/hubs/notifications`.
4. Lang nghe event `notification.created`, `notification.read`, `notification.readAll`, `notification.unreadCountChanged`, `notification.error`.
5. PUT `/api/v1/notifications/{id}/read` hoặc `/read-all`.

Expected Result: badge và danh sách notification cập nhật realtime.

Related APIs: Notifications, NotificationHub.

### Owner Venue Management

Business Goal: owner quản lý venue, hình ảnh, amenities, field, slot.

Actors: Owner.

Preconditions: đã login với role `Owner`.

Step-by-Step Flow:

1. GET `/api/v1/owner/venues`.
2. POST `/api/v1/owner/venues` tạo venue.
3. PUT `/api/v1/owner/venues/{id}` sửa venue.
4. POST `/api/v1/owner/venues/{id}/images` upload images.
5. POST `/api/v1/owner/venues/{id}/amenities` gắn amenity.
6. POST `/api/v1/owner/venues/{venueId}/fields` tạo field.
7. POST `/api/v1/owner/fields/{id}/slots/bulk` tạo slot hàng loạt.

Expected Result: owner quản lý được toàn bộ dữ liệu venue của mình.

Related APIs: Owner, OwnerVenues, OwnerFields.

### Owner Discount Management

Business Goal: owner tạo và quản lý mã giảm giá.

Actors: Owner, Admin qua `api/v1/owner`.

Preconditions: đã login với role `Owner` hoặc `Admin`.

Step-by-Step Flow:

1. GET `/api/v1/owner/discounts`.
2. POST `/api/v1/owner/discounts`.
3. PUT `/api/v1/owner/discounts/{id}`.
4. PUT `/api/v1/owner/discounts/{id}/status`.
5. DELETE `/api/v1/owner/discounts/{id}`.

Expected Result: discount được tạo/cập nhật/xóa.

Related APIs: Owner discounts, Discounts validate.

### Admin Diagnostic

Business Goal: kiểm tra quyền admin và xem placeholder data.

Actors: Admin, Owner cho courts.

Preconditions: đã login với role phù hợp.

Step-by-Step Flow:

1. GET `/api/Admin/stats`.
2. GET `/api/Admin/courts`.
3. POST `/api/Admin/test-role`.

Expected Result: stats/courts trả hardcoded data; test-role chỉ OK nếu claim role là `Admin`.

Related APIs: Admin endpoints.

## SECTION 3 - MA TRẬN PHÂN QUYỀN

| Feature | Guest | User | Owner | Admin |
| --- | --- | --- | --- | --- |
| Register/Login | Yes | Yes | Yes | Yes |
| View venues/search/map | Yes | Yes | Yes | Yes |
| View fields/slots/reviews public | Yes | Yes | Yes | Yes |
| Lock/unlock slot for checkout | No | Yes | Yes | Yes |
| Create booking | No | Yes | Yes | Yes |
| View own booking history | No | Yes | Yes | Yes |
| Cancel own booking | No | Yes | Yes | Yes |
| Validate discount | No | Yes | Yes | Yes |
| Deposit payment | No | Booking customer | Booking customer | Booking customer |
| Final payment | No | Booking customer | Booking owner | Yes |
| Refund payment | No | Own payment | Yes | Yes |
| Chat | No | Yes | Yes | Yes |
| Notifications | No | Yes | Yes | Yes |
| Create/update own review | No | Review author | Review author | Review author |
| Delete review | No | No | Yes | Yes |
| Owner stats/revenue/bookings/discounts | No | No | Yes | Yes |
| Owner venue CRUD in OwnerVenuesController | No | No | Yes | No |
| Owner field endpoints in OwnerFieldsController | No | No | Yes | No |
| TimeSlot create/update/delete via `/api/v1/slots` | No | No | Yes | Yes |
| Admin stats | No | No | No | Yes |
| Admin courts placeholder | No | No | Yes | Yes |
| Admin test-role | No | No | No | Yes by programmatic check |
| Payment gateway callback/webhook | Public | Public | Public | Public |

## SECTION 4 - DANH MỤC API

### Header request chung

- Endpoint public: không cần auth header.
- Endpoint cần đăng nhập: `Authorization: Bearer <accessToken>`.
- Endpoint dùng JSON body: `Content-Type: application/json`.
- File upload: `Content-Type: multipart/form-data`.
- SePay webhook: `X-API-Key: <configured webhook api key>`.

### Dạng response

Backend trả hai dạng response:

- Raw DTO/list: ví dụ `PaymentDto`, `ChatRoomDto[]`.
- Wrapper object: `{ success, message, data, errors }`.
- Một số success simple: `{ success, message }`, `{ unreadCount }`.
- Global exception middleware: `{ message, details, timestamp }`.

### Admin

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/Admin/stats` | Bearer, role `Admin` | None | None | Incomplete: hardcoded data | `{ message, totalUsers, totalBookings }` | 401/403 | `GET /api/Admin/stats` |
| GET `/api/Admin/courts` | Bearer, role `Admin,Owner` | None | None | Incomplete: hardcoded data | `{ message, courts }` | 401/403 | `GET /api/Admin/courts` |
| POST `/api/Admin/test-role` | Bearer, programmatic role claim must equal `Admin` | None | None | Checks claim `http://schemas.microsoft.com/ws/2008/06/identity/claims/role` | `{ message: "You are an Admin!" }` | 401/403 | `POST /api/Admin/test-role` |

### Amenities

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/amenities` | Public | None | None | None in controller | Wrapper with `AmenityDto[]` | Global errors | `GET /api/v1/amenities` |

### Auth

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| POST `/api/v1/auth/register` | Public, rate limit `AuthPolicy` 5/min | None | `RegisterRequestDto` | FullName bắt buộc max 200; Email bắt buộc và đúng định dạng; PhoneNumber bắt buộc theo regex `^\+?[1-9]\d{1,14}$`; Password min 8, có chữ hoa/chữ thường/số/ký tự đặc biệt; ConfirmPassword phải bằng Password; email/phone phải unique; role mặc định `User` | `AuthResponseDto` có tokens | 400 nếu invalid/exists | Body `{ "fullName":"An Dang","email":"an@example.com","phoneNumber":"0902311007","password":"Password@123","confirmPassword":"Password@123" }` |
| POST `/api/v1/auth/login` | Public, rate limit `AuthPolicy` 5/min | None | `LoginRequestDto` | Email required valid; Password required; user must active; password must match | `AuthResponseDto` | 400/401 | Body `{ "email":"an@example.com","password":"Password@123" }` |
| POST `/api/v1/auth/refresh-token` | Public but requires old access token header | Header old `Authorization` | `RefreshTokenRequestDto` | AccessToken required from header; refresh token must match user and not expired | `AuthResponseDto` with new tokens | 400 missing access token; 401 invalid | Body `{ "refreshToken":"..." }` |
| POST `/api/v1/auth/logout` | Bearer | None | None | Current token must map to user | `AuthResponseDto` | 400/401 | `POST /api/v1/auth/logout` |

Không expose qua API: `GetMe`, `ChangePassword`, `ForgotPassword`, `ResetPassword` có code/handler nhưng bị `[NonAction]`.

### Bookings

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| POST `/api/v1/bookings` | Bearer | None | `CreateBookingCommand`: `slotIds`, `discountCode?`, `note?`; `userId` ignored from JSON | SlotIds not empty; slots not past; all slots same venue; slot available or locked by current user; discount valid if supplied; booking starts `Pending`; deposit is 50 percent | 201 wrapper with `BookingDto` | 400/404/401 | `{ "slotIds":["..."],"discountCode":"RIVER20","note":"Need bibs" }` |
| GET `/api/v1/bookings/{id}` | Bearer | Route `id` | None | User can view own booking; Owner/Admin can view through handler flags | `BookingDto` | 401/403/404 | `GET /api/v1/bookings/{id}` |
| GET `/api/v1/bookings/history` | Bearer | Query `status?`, `from?`, `to?`, `page=1`, `pageSize=20` | None | Current user's bookings | `BookingDto[]` | 401/global errors | `GET /api/v1/bookings/history?status=Pending&page=1&pageSize=20` |
| GET `/api/v1/bookings/{id}/review` | Bearer | Route `id` | None | Returns review for booking or null in wrapper data | Wrapper with `ReviewDto?` | 401/global errors | `GET /api/v1/bookings/{id}/review` |
| PUT `/api/v1/bookings/{id}/cancel` | Bearer | Route `id`; query `cancellationReason?` max 500 | None | Only customer unless command says owner/admin; status must `Pending` or `Accepted`; slots become `Available`; notification to owner | `{ success, message }` | 400/401/404 | `PUT /api/v1/bookings/{id}/cancel?cancellationReason=Changed%20plan` |
| PUT `/api/v1/bookings/slots/{slotId}/lock` | Bearer | Route `slotId`; query `bookingId` required | None | Slot must be `Available`; locks 15 min for current user | `{ success, message }` | 400/401/404 | `PUT /api/v1/bookings/slots/{slotId}/lock?bookingId={bookingId}` |
| PUT `/api/v1/bookings/slots/{slotId}/unlock` | Bearer | Route `slotId`; query `unlockReason=ManualUnlock` max 100 | None | Slot must be `Locked`; sets available | `{ success, message }` | 400/401/404 | `PUT /api/v1/bookings/slots/{slotId}/unlock?unlockReason=PaymentTimeout` |
| GET `/api/v1/bookings/health` | Public | None | None | Health endpoint | `{ status: "API is running" }` | None in controller | `GET /api/v1/bookings/health` |

### Chats

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/chats/rooms` | Bearer | Query `pageNumber=1`, `pageSize=10` | None | Current user's rooms | `ChatRoomDto[]` | 401/global errors | `GET /api/v1/chats/rooms` |
| POST `/api/v1/chats/rooms` | Bearer | None | `CreateChatRoomRequestDto` | If `venueId` present, room with venue owner; otherwise compute other user from `customerId`/`ownerId` | `ChatRoomDto` | 401/global errors | `{ "venueId":"..." }` |
| GET `/api/v1/chats/rooms/{roomId}/messages` | Bearer | Route `roomId`; query `pageNumber=1`, `pageSize=20` | None | Caller must be customer or host; marks room messages read | `MessageDto[]` | 404/401/global errors | `GET /api/v1/chats/rooms/{roomId}/messages` |
| POST `/api/v1/chats/rooms/{roomId}/messages` | Bearer | Route `roomId` | `MessageDto`; only `messageText` is used | Message text required; caller must be participant; creates notification to recipient; publishes realtime events | 201 `MessageDto` | 400/401/404 | `{ "messageText":"Hello" }` |
| PUT `/api/v1/chats/rooms/{roomId}/read` | Bearer | Route `roomId` | None | Caller must be participant | `{ unreadCount }` | 401/global errors | `PUT /api/v1/chats/rooms/{roomId}/read` |

Không expose qua API: delete message, close chat room, send message by body, get/create room by direct route là `[NonAction]`.

### Discounts

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| POST `/api/v1/discounts/validate` | Bearer | None | `ValidateDiscountRequestDto` | If slotIds supplied, backend calculates total from slots; discount must exist, active, within dates, under usage limit, min booking amount met | `ValidateDiscountResponseDto` | 401/global errors | `{ "code":"RIVER20","slotIds":["..."],"totalAmount":0 }` |

Không expose trong `DiscountsController`: list/get/create/update/delete, vì các method này là `[NonAction]`. CRUD discount thật sự nằm ở `/api/v1/owner/discounts`.

### Fields

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/fields/{id}` | Public | Route `id` | None | Returns wrapper, catch all as not found | Wrapper with field dto | 404 wrapper | `GET /api/v1/fields/{id}` |
| GET `/api/v1/fields/{id}/slots` | Public | Route `id`; query `date?` | None | Defaults date to `DateTime.UtcNow.Date`; returns available slots query | Wrapper with `TimeSlotDto[]` | 400 wrapper | `GET /api/v1/fields/{id}/slots?date=2026-01-07` |

### Notifications

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/notifications` | Bearer | Query `unreadOnly=false`, `pageNumber=1`, `pageSize=10` | None | Current user's notifications | `NotificationDto[]` | 401/global errors | `GET /api/v1/notifications?unreadOnly=true` |
| GET `/api/v1/notifications/unread-count` | Bearer | None | None | Current user | `{ unreadCount }` | 401/global errors | `GET /api/v1/notifications/unread-count` |
| PUT `/api/v1/notifications/{id}/read` | Bearer | Route `id` | None | Marks notification read for current user; publishes realtime read and unread count | `{ success }` | 401/404/global errors | `PUT /api/v1/notifications/{id}/read` |
| PUT `/api/v1/notifications/read-all` | Bearer | None | None | Marks all notifications read for current user; publishes realtime read all and unread count | `{ success }` | 401/global errors | `PUT /api/v1/notifications/read-all` |

Không expose qua API: get notification by id, delete notification.

### Owner

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/owner/stats` | Bearer role `Owner,Admin` | None | None | Stats for current user id as owner | `OwnerStatsDto` | 401/403/global errors | `GET /api/v1/owner/stats` |
| GET `/api/v1/owner/revenue` | Bearer role `Owner,Admin` | Query `from?`, `to?`, `groupBy=day|month|venue` | None | Uses successful payments only | `OwnerRevenueDto[]` | 401/403/global errors | `GET /api/v1/owner/revenue?groupBy=month` |
| POST `/api/v1/owner/venues/{id}/amenities` | Bearer role `Owner,Admin` | Route `id` | `VenueAmenityRequestDto` | Owner must own venue; amenity ids must exist | `string[]` amenity names | 401/403/404/global errors | `{ "amenityIds":["..."] }` |
| DELETE `/api/v1/owner/venues/{id}/amenities/{amenityId}` | Bearer role `Owner,Admin` | Route `id`, `amenityId` | None | Owner must own venue | `{ success }` | 401/403/404/global errors | `DELETE /api/v1/owner/venues/{id}/amenities/{amenityId}` |
| GET `/api/v1/owner/venues/{venueId}/fields` | Bearer role `Owner,Admin` | Route `venueId` | None | No ownership check visible in controller | `FootballFieldDto[]` | 401/403/global errors | `GET /api/v1/owner/venues/{venueId}/fields` |
| POST `/api/v1/owner/venues/{venueId}/fields` | Bearer role `Owner,Admin` | Route `venueId` | `FootballFieldDto` | Handler creates field for current owner/venue | 201 `FootballFieldDto` | 401/403/global errors | `{ "fieldName":"Field A","fieldType":"FiveASide","pricePerHour":180000 }` |
| POST `/api/v1/owner/fields/{id}/slots/bulk` | Bearer role `Owner,Admin` | Route field `id` | `BulkCreateSlotsDto` | Owner must own field venue; `StartTime`/`EndTime` must parse; duration > 0 | `BulkCreateSlotsResultDto` | 401/403/400/404 | `{ "fromDate":"2026-01-07","toDate":"2026-01-07","startTime":"18:00","endTime":"22:00","slotDurationMinutes":60,"price":200000 }` |
| PUT `/api/v1/owner/slots/{id}` | Bearer role `Owner,Admin` | Route `id` | `TimeSlotDto` | Owner must own slot field venue; endTime > startTime; status valid | `TimeSlotDto` | 401/403/400/404 | `{ "fieldId":"...","startTime":"2026-01-07T18:00:00Z","endTime":"2026-01-07T19:00:00Z","price":200000,"slotStatus":"Available" }` |
| PUT `/api/v1/owner/slots/{id}/status` | Bearer role `Owner,Admin` | Route `id` | `UpdateSlotStatusDto` | Owner must own slot field venue; slotStatus `Available`, `Locked`, `Booked` | `StatusResultDto` | 401/403/400/404 | `{ "slotStatus":"Available" }` |
| DELETE `/api/v1/owner/slots/{id}` | Bearer role `Owner,Admin` | Route `id` | None | Slot id required validator | `{ success }` | 401/403/404 | `DELETE /api/v1/owner/slots/{id}` |
| GET `/api/v1/owner/bookings/pending` | Bearer role `Owner,Admin` | None | None | Current owner pending bookings | `BookingDto[]` | 401/403/global errors | `GET /api/v1/owner/bookings/pending` |
| GET `/api/v1/owner/bookings` | Bearer role `Owner,Admin` | None | None | Current owner bookings | `BookingDto[]` | 401/403/global errors | `GET /api/v1/owner/bookings` |
| GET `/api/v1/owner/bookings/{id}` | Bearer role `Owner,Admin` | Route `id` | None | Controller filters from owner bookings; returns 404 if not found | `BookingDto` | 401/403/404 | `GET /api/v1/owner/bookings/{id}` |
| PUT `/api/v1/owner/bookings/{id}/accept` | Bearer role `Owner,Admin` | Route `id` | None | Owner must own booked venue; booking must be `Pending`; status becomes `Accepted` | `{ success }` | 401/403/400/404 | `PUT /api/v1/owner/bookings/{id}/accept` |
| PUT `/api/v1/owner/bookings/{id}/reject` | Bearer role `Owner,Admin` | Route `id`; query `rejectionReason?` max 500 | None | Owner must own booked venue; booking must be `Pending`; status `Rejected`; slots `Available` | `{ success }` | 401/403/400/404 | `PUT /api/v1/owner/bookings/{id}/reject?rejectionReason=Maintenance` |
| PUT `/api/v1/owner/bookings/{id}/complete` | Bearer role `Owner,Admin` | Route `id` | None | Owner must own booked venue; status becomes `Completed`; slots `Booked` | `StatusResultDto` | 401/403/400/404 | `PUT /api/v1/owner/bookings/{id}/complete` |
| GET `/api/v1/owner/discounts` | Bearer role `Owner,Admin` | None | None | Current owner discounts | `DiscountDto[]` | 401/403/global errors | `GET /api/v1/owner/discounts` |
| POST `/api/v1/owner/discounts` | Bearer role `Owner,Admin` | None | `DiscountDto` | Code required; value > 0; endDate > startDate; usageLimit >= 0; type `Percentage` or `Fixed` | 201 `DiscountDto` | 401/403/400/404 | `{ "code":"SAVE10","name":"Save 10","discountType":"Percentage","value":10,"startDate":"2026-01-01","endDate":"2026-02-01","isActive":true }` |
| PUT `/api/v1/owner/discounts/{id}` | Bearer role `Owner,Admin` | Route `id` | `DiscountDto` | Owner must own discount; same validation as create | `DiscountDto` | 401/403/400/404 | `PUT /api/v1/owner/discounts/{id}` |
| PUT `/api/v1/owner/discounts/{id}/status` | Bearer role `Owner,Admin` | Route `id` | `UpdateStatusDto` | Owner must own discount | `StatusResultDto` | 401/403/404 | `{ "isActive": false }` |
| DELETE `/api/v1/owner/discounts/{id}` | Bearer role `Owner,Admin` | Route `id` | None | Owner must own discount | `{ success }` | 401/403/404 | `DELETE /api/v1/owner/discounts/{id}` |

### OwnerFields

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| PUT `/api/v1/owner/fields/{id}` | Bearer role `Owner` | Route `id` | `UpdateFieldRequestDto` | Handler throws NotFound/Forbidden; fieldName, fieldType, price per request DTO | Wrapper with updated field | 401/403/404/global errors | `{ "fieldName":"Field A","fieldType":"FiveASide","pricePerHour":180000 }` |
| PUT `/api/v1/owner/fields/{id}/status` | Bearer role `Owner` | Route `id` | `UpdateFieldStatusRequestDto` | Handler throws NotFound/Forbidden | Wrapper with `{ isActive }` | 401/403/404/global errors | `{ "isActive": true }` |

### OwnerVenues

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/owner/venues` | Bearer role `Owner` | Query `isActive?`, `page=1`, `pageSize=10` | None | Chỉ lấy venue của owner hiện tại | Wrapper với `PagedResult<VenueDto>` | 401/403/global errors | `GET /api/v1/owner/venues?isActive=true` |
| POST `/api/v1/owner/venues` | Bearer role `Owner` | None | `CreateVenueRequestDto` | Created owner is current user; `IsActive=true` | Wrapper with `VenueDto` | 401/403/global errors | `{ "venueName":"New Venue","address":"...","latitude":10.1,"longitude":106.1,"description":"","openingHours":"06:00-23:00","phoneContact":"090..." }` |
| POST `/api/v1/owner/venues/{id}/images` | Bearer role `Owner` | Route `id` | Multipart form field `images` list | Phải gửi ít nhất một file; owner phải sở hữu venue; source không khai báo accepted file type/max size | Wrapper với danh sách URL đã upload | 400/401/403/global errors | `multipart/form-data: images=<file1>, images=<file2>` |
| PUT `/api/v1/owner/venues/{id}` | Bearer role `Owner` | Route `id` | `UpdateVenueRequestDto` | Owner must own venue | Wrapper with `VenueDto` | 401/403/404/global errors | Same fields as create |
| PUT `/api/v1/owner/venues/{id}/status` | Bearer role `Owner` | Route `id` | `UpdateVenueStatusRequestDto` | Owner must own venue; cannot deactivate if active bookings exist | Wrapper with `{ isActive }` | 400/401/403/404 | `{ "isActive": false }` |
| DELETE `/api/v1/owner/venues/{id}/images/{imageId}` | Bearer role `Owner` | Route `id`, `imageId` | None | Owner must own venue/image | Wrapper success | 401/403/404 | `DELETE /api/v1/owner/venues/{id}/images/{imageId}` |

### Payments

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/payments/booking/{bookingId}` | Bearer | Route `bookingId` | None | User can view own booking payments; Owner/Admin can view through flag | `PaymentDto[]` | 401/404/global errors | `GET /api/v1/payments/booking/{bookingId}` |
| GET `/api/v1/payments/{id}` | Bearer | Route `id` | None | User can view own payment; Owner/Admin can view through flag | `PaymentDto` | 401/404/global errors | `GET /api/v1/payments/{id}` |
| GET `/api/v1/payments/history` | Bearer | Query `pageNumber=1`, `pageSize=10` | None | Current user payment history | `PaymentDto[]` | 401/global errors | `GET /api/v1/payments/history` |
| POST `/api/v1/payments/deposit` | Bearer | None | `ProcessPaymentRequestDto` | Only booking customer; booking must be `Accepted`; deposit not already successful | 201 `PaymentDto` | 400/401/404 | `{ "bookingId":"...","paymentMethod":"SePay","transactionCode":null }` |
| POST `/api/v1/payments/final` | Bearer | None | `ProcessPaymentRequestDto` | Customer, booking owner, or admin; booking must be `Deposited`; final not already successful | 201 `PaymentDto` | 400/401/404 | `{ "bookingId":"...","paymentMethod":"Cash" }` |
| POST `/api/v1/payments/{id}/refund` | Bearer | Route `id` | None | Owner/Admin or payment booking user; payment status must `Success`; sets `Refunded` | `PaymentDto` | 400/401/404 | `POST /api/v1/payments/{id}/refund` |
| POST `/api/v1/payments/webhook/sepay` | Public with `X-API-Key` | Header `X-API-Key` | `SePayWebhookDto` | Invalid/missing API key returns 401; ignores non-income; content must contain `CM{TransactionCode}`; amount must match | `{"success": true}` (SePay requires exact format) | 401, 400 | `{ "id":1,"transferType":"in","transferAmount":180000,"content":"CMDEP-2026-0009" }` |
| GET `/api/v1/payments/{paymentId}/sepay-qr` | Bearer | Route `paymentId` | None | Uses public payment query internally; builds QR URL from SePay settings | `SePayQrResponseDto` | 401/404 | `GET /api/v1/payments/{paymentId}/sepay-qr` |

### Reviews

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/reviews/venue/{id}` | Public | Route venue `id`; query `page=1`, `pageSize=10` | None | Venue reviews query | Wrapper with `VenueReviewsResponseDto` | 404/global errors | `GET /api/v1/reviews/venue/{id}` |
| GET `/api/v1/reviews/{id}` | Public | Route `id` | None | NotFound caught in controller | Wrapper with `ReviewDto` | 404 wrapper | `GET /api/v1/reviews/{id}` |
| GET `/api/v1/reviews/field/{fieldId}/average-rating` | Public | Route `fieldId` | None | Fetches up to 1000 field reviews and calculates average in controller | Wrapper with `{ fieldId, averageRating, totalReviews }` | 404/global errors | `GET /api/v1/reviews/field/{fieldId}/average-rating` |
| GET `/api/v1/reviews/my-reviews` | Bearer | None | None | Current user's reviews | Wrapper with `ReviewDto[]` | 401/global errors | `GET /api/v1/reviews/my-reviews` |
| POST `/api/v1/reviews` | Bearer | None | `CreateReviewRequestDto` | Rating 1-5; booking must belong to user; booking must `Completed`; one review per booking | 201 wrapper with `ReviewDto` | 400/401 | `{ "venueId":"...","bookingId":"...","rating":5,"comment":"Good" }` |
| PUT `/api/v1/reviews/{id}` | Bearer | Route `id` | `UpdateReviewRequestDto` | Rating 1-5; only review author | Wrapper with `ReviewDto` | 400/401/403/404 | `{ "rating":4,"comment":"Updated" }` |
| DELETE `/api/v1/reviews/{id}` | Bearer role `Admin,Owner` | Route `id` | None | Soft delete command | `{ success, message }` | 401/404 | `DELETE /api/v1/reviews/{id}` |

### TimeSlots

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/slots` | Public | Query `fieldId` | None | Gets slots by field | Wrapper with `TimeSlotDto[]` | Global errors | `GET /api/v1/slots?fieldId={fieldId}` |
| GET `/api/v1/slots/{id}` | Public | Route `id` | None | NotFound caught | Wrapper with `TimeSlotDto` | 404 wrapper | `GET /api/v1/slots/{id}` |
| GET `/api/v1/slots/available` | Public | Query `fieldId`, `date` | None | Date specified as UTC kind; returns available slots | Wrapper with `TimeSlotDto[]` | Global errors | `GET /api/v1/slots/available?fieldId={fieldId}&date=2026-01-07` |
| POST `/api/v1/slots/{id}/lock` | Bearer | Route `id` | None | Slot must exist, not past, available or expired lock; locks 15 min by user | Wrapper success | 400/401/404 | `POST /api/v1/slots/{id}/lock` |
| POST `/api/v1/slots/{id}/unlock` | Bearer | Route `id` | None | Only user who locked slot can unlock | Wrapper success | 400/401/404 | `POST /api/v1/slots/{id}/unlock` |
| POST `/api/v1/slots` | Bearer role `Owner,Admin` | None | `TimeSlotDto` | Owner must own field venue; endTime > startTime; status valid; price defaults to field price if <= 0 | 201 wrapper with `TimeSlotDto` | 400/401 | `TimeSlotDto` body |
| PUT `/api/v1/slots/{id}` | Bearer role `Owner,Admin` | Route `id` | `TimeSlotDto` | Owner must own slot; endTime > startTime; status valid | Wrapper with `TimeSlotDto` | 400/401/404 | `PUT /api/v1/slots/{id}` |
| DELETE `/api/v1/slots/{id}` | Bearer role `Owner,Admin` | Route `id` | None | Slot id required | `{ success, message }` | 401/404 | `DELETE /api/v1/slots/{id}` |

### Users

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| PUT `/api/v1/users/profile` | Bearer | None | `UpdateProfileRequestDto` | FullName bắt buộc max 200; Phone bắt buộc theo regex gần E.164; AvatarUrl max 500 khi có gửi; email/password không được cập nhật tại đây | Wrapper với `UserDto` | 401/global validation errors | `{ "fullName":"An Dang","phone":"0902311007","avatarUrl":"https://..." }` |

Không có endpoint GET current profile expose hiện tại; `AuthController.GetMe` là `[NonAction]`.

### Venues

| Endpoint | Auth | Params | Body | Validation/Rules | Success Response | Error Responses | Example |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GET `/api/v1/Venues` | Public | Query `Q?`, `FieldType?`, `AmenityIds?`, `MinRating?`, `PriceMin?`, `PriceMax?`, `Sort?`, `Page=1`, `PageSize=10` | None | Returns paged venues | Wrapper with `PagedResult<VenueDto>` | Global errors | `GET /api/v1/Venues?Page=1&PageSize=10` |
| GET `/api/v1/Venues/{id}` | Public | Route `id` | None | 404 wrapper if null | Wrapper with `VenueDetailDto` | 404 wrapper | `GET /api/v1/Venues/{id}` |
| GET `/api/v1/Venues/{id}/fields` | Public | Route `id` | None | Gets fields of venue | Wrapper with `FootballFieldDto[]` | Global errors | `GET /api/v1/Venues/{id}/fields` |
| GET `/api/v1/Venues/{id}/amenities` | Public | Route `id` | None | Gets amenities of venue | Wrapper with `AmenityDto[]` | Global errors | `GET /api/v1/Venues/{id}/amenities` |
| GET `/api/v1/Venues/{id}/images` | Public | Route `id` | None | Gets venue images | Wrapper with `VenueImageDto[]` | Global errors | `GET /api/v1/Venues/{id}/images` |
| GET `/api/v1/Venues/search` | Public | Query `q`, `page=1`, `pageSize=10` | None | Internally maps to `GetVenuesQuery.Q` | Wrapper with `PagedResult<VenueDto>` | Global errors | `GET /api/v1/Venues/search?q=saigon` |
| GET `/api/v1/Venues/map/nearby` | Public | Query `lat`, `lng`, `radius=5.0` | None | Nearby query in km | Wrapper with `VenueDto[]` | Global errors | `GET /api/v1/Venues/map/nearby?lat=10.79&lng=106.72&radius=5` |

#### Frontend: lấy tất cả venues

Endpoint dùng để lấy tất cả venue cho màn hình danh sách là:

```http
GET /api/v1/Venues?Page=1&PageSize=10
```

Auth: Public, không cần `Authorization` header.

Response shape:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "items": [
      {
        "venueId": "00000000-0000-0000-0000-000000000000",
        "venueName": "Saigon Riverside Sports Park",
        "address": "12 Nguyen Huu Canh, Binh Thanh, Ho Chi Minh City",
        "latitude": 10.791054,
        "longitude": 106.719809,
        "distance": null,
        "description": "Riverside venue with four compact football fields.",
        "openingHours": "06:00-23:00",
        "phoneContact": "02873010001",
        "ownerName": "",
        "averageRating": 0,
        "totalReviews": 0,
        "minPrice": 0,
        "maxPrice": 0
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalItems": 12,
    "totalPages": 2
  },
  "errors": []
}
```

Do backend trả `PagedResult<VenueDto>`, hiện tại không có endpoint unpaged "get all" riêng. Frontend muốn lấy hết venues thì dùng một trong hai cách:

1. Gọi theo từng page đến khi `page >= data.totalPages`.
2. Gọi `GET /api/v1/Venues?Page=1&PageSize=<pageSizeLon>` nếu UI chỉ cần load một lần và backend/database hiện tại chấp nhận `pageSize` đó.

Pseudo flow:

```ts
let page = 1;
const pageSize = 50;
const allVenues = [];

while (true) {
  const res = await api.get(`/api/v1/Venues?Page=${page}&PageSize=${pageSize}`);
  const pageData = res.data.data;
  allVenues.push(...pageData.items);

  if (page >= pageData.totalPages) break;
  page += 1;
}
```

Query filters dùng chung cho endpoint này:

- `Q`: từ khóa tìm kiếm.
- `FieldType`: `FiveASide`, `SevenASide`, `ElevenASide`.
- `AmenityIds`: chuỗi amenity ids theo cách handler parse trong backend.
- `MinRating`: rating tối thiểu.
- `PriceMin`, `PriceMax`: khoảng giá.
- `Sort`: cách sắp xếp, source có field query nhưng giá trị sort hợp lệ cần theo handler/repository hiện tại.
- `Page`, `PageSize`: phân trang.

Frontend list screen nên đọc venues từ `response.data.items` nếu API client đã unwrap wrapper, hoặc `response.data.data.items` nếu dùng raw HTTP response.

### SignalR Hubs

| Hub | Auth | Client Methods | Server Events | Notes |
| --- | --- | --- | --- | --- |
| `/hubs/chat` | Bearer or `access_token` query | `JoinRoom(Guid roomId)`, `LeaveRoom(Guid roomId)`, `SendMessage(Guid roomId,string messageText)`, `MarkRoomAsRead(Guid roomId)` | `chat.roomJoined`, `chat.messageCreated`, `chat.roomUpdated`, `chat.messagesRead`, `chat.error` | On connect joins user group `user:{userId}`. JoinRoom validates room participant. |
| `/hubs/notifications` | Bearer or `access_token` query | `GetUnreadCount()`, `MarkNotificationAsRead(Guid notificationId)`, `MarkAllNotificationsAsRead()` | `notification.created`, `notification.read`, `notification.readAll`, `notification.unreadCountChanged`, `notification.error` | On connect joins user group `user:{userId}`. |

## SECTION 5 - THAM CHIẾU DTO

### Auth DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `RegisterRequestDto` | `fullName:string`, `email:string`, `phoneNumber:string`, `password:string`, `confirmPassword:string` | Bắt buộc theo validator | `{ "fullName":"An Dang","email":"an@example.com","phoneNumber":"0902311007","password":"Password@123","confirmPassword":"Password@123" }` |
| `LoginRequestDto` | `email:string`, `password:string` | Required by validator | `{ "email":"an@example.com","password":"Password@123" }` |
| `RefreshTokenRequestDto` | `refreshToken:string` | Required by validator | `{ "refreshToken":"..." }` |
| `AuthResponseDto` | `success:bool`, `message:string`, `accessToken:string?`, `refreshToken:string?`, `user:UserAuthDto?` | Tokens/user nullable | `{ "success":true,"message":"Login successful","accessToken":"...","refreshToken":"...","user":{} }` |
| `UserAuthDto` | `id:Guid`, `fullName:string`, `email:string`, `phoneNumber:string`, `roles:string[]` | Code có default non-null | `{ "id":"...","fullName":"An Dang","roles":["User"] }` |
| `ChangePasswordRequestDto` | `currentPassword`, `newPassword`, `confirmNewPassword` | DTO exists but endpoint NonAction | Not callable |
| `ForgotPasswordRequestDto` | `email` | DTO exists but endpoint NonAction | Not callable |
| `ResetPasswordRequestDto` | `email`, `token`, `newPassword`, `confirmNewPassword` | DTO exists but endpoint NonAction | Not callable |

### User DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `UserDto` | `id`, `fullName`, `email`, `phoneNumber`, `avatarUrl?`, `loyaltyPoints`, `isActive`, `roles` | `avatarUrl` nullable | `{ "id":"...","fullName":"An","loyaltyPoints":120,"roles":["User"] }` |
| `UpdateProfileRequestDto` | `fullName:string`, `phone:string`, `avatarUrl:string?` | FullName/Phone required by validator; avatarUrl nullable max 500 | `{ "fullName":"An","phone":"0902311007","avatarUrl":null }` |

### Venue and Field DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `VenueDto` | `venueId`, `venueName`, `address`, `latitude`, `longitude`, `distance?`, `description`, `openingHours`, `phoneContact`, `ownerName`, `averageRating`, `totalReviews`, `minPrice`, `maxPrice` | `distance` nullable | `{ "venueId":"...","venueName":"Saigon Riverside Sports Park","averageRating":4.5 }` |
| `VenueDetailDto` | all `VenueDto` fields plus `images`, `amenities`, `fields` | Lists default empty | `{ "venueId":"...","images":[],"amenities":[],"fields":[] }` |
| `CreateVenueRequestDto` | `venueName`, `address`, `latitude`, `longitude`, `description`, `openingHours`, `phoneContact` | No explicit validator in source | `{ "venueName":"New Venue","address":"...","latitude":10.1,"longitude":106.1 }` |
| `UpdateVenueRequestDto` | same as create | No explicit validator in source | Same as create |
| `UpdateVenueStatusRequestDto` | `isActive:bool` | bool | `{ "isActive":true }` |
| `VenueImageDto` | `imageId`, `imageUrl`, `isPrimary` | Non-null defaults | `{ "imageId":"...","imageUrl":"https://...","isPrimary":true }` |
| `AmenityDto` | `amenityId`, `name`, `icon` | Non-null defaults | `{ "amenityId":"...","name":"Free wifi","icon":"wifi" }` |
| `FootballFieldDto` | `id`, `ownerId`, `fieldName`, `description`, `fieldType`, `location`, `latitude`, `longitude`, `pricePerHour`, `isActive` | No explicit validator in source | `{ "fieldName":"Field A","fieldType":"FiveASide","pricePerHour":180000 }` |
| `UpdateFieldRequestDto` | `fieldName`, `fieldType:FieldType`, `pricePerHour` | No explicit validator in source | `{ "fieldName":"Field A","fieldType":"SevenASide","pricePerHour":220000 }` |
| `UpdateFieldStatusRequestDto` | `isActive:bool` | bool | `{ "isActive":false }` |

Enums:

- `FieldType`: `FiveASide`, `SevenASide`, `ElevenASide`.

### Slot DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `TimeSlotDto` | `slotId`, `fieldId`, `startTime`, `endTime`, `price`, `slotStatus`, `createdAt`, `updatedAt?` | `updatedAt` nullable | `{ "fieldId":"...","startTime":"2026-01-07T18:00:00Z","endTime":"2026-01-07T19:00:00Z","price":180000,"slotStatus":"Available" }` |
| `BulkCreateSlotsDto` | `fromDate`, `toDate`, `startTime`, `endTime`, `slotDurationMinutes`, `price` | `startTime` default `06:00`; `endTime` default `23:00`; duration default 60 | `{ "fromDate":"2026-01-07","toDate":"2026-01-14","startTime":"18:00","endTime":"22:00","slotDurationMinutes":60,"price":200000 }` |
| `UpdateSlotStatusDto` | `slotStatus:string` | Must parse to slot enum | `{ "slotStatus":"Booked" }` |

Enums:

- `SlotStatus`: `Available`, `Locked`, `Booked`.

### Booking DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `CreateBookingCommand` | `slotIds:Guid[]`, `discountCode:string?`, `note:string?` | `slotIds` required/not empty; `userId` ignored from JSON | `{ "slotIds":["..."],"discountCode":"RIVER20","note":"Need water" }` |
| `BookingDto` | `id`, `userId`, `fieldId`, `startTime`, `endTime`, `totalPrice`, `depositAmount`, `discountAmount`, `bookingStatus`, `note?`, `createdAt`, `items`, `payments` | `note` nullable | `{ "id":"...","bookingStatus":"Pending","items":[],"payments":[] }` |
| `BookingItemDto` | `bookingItemId`, `slotId`, `fieldId`, `fieldName?`, `venueId`, `venueName?`, `startTime`, `endTime`, `price` | names nullable | `{ "slotId":"...","fieldName":"Field A","price":180000 }` |
| `BookingHistoryDto` | `BookingDto` plus `fieldName?`, `fieldLocation?`, `ownerName?`, `statusDisplay?`, `canCancel`, `canPayment`, `timeSlots?` | optional display fields | Response subtype possible |

Enums:

- `BookingStatus`: `Pending`, `Accepted`, `Rejected`, `Deposited`, `Completed`, `Cancelled`.

### Payment DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `ProcessPaymentRequestDto` | `bookingId:Guid`, `paymentMethod:PaymentMethod`, `transactionCode:string?` | `transactionCode` nullable | `{ "bookingId":"...","paymentMethod":"SePay","transactionCode":null }` |
| `PaymentDto` | `id`, `bookingId`, `amount`, `paymentStatus`, `paymentType`, `paymentMethod`, `transactionCode`, `paidAt?`, `paymentUrl?`, `bookingStatus?` | nullable payment fields in response | `{ "id":"...","amount":175000,"paymentStatus":"Pending","paymentMethod":"SePay" }` |
| `PaymentGatewayCallbackResultDto` | `statusCode:int`, `success:bool`, `message:string`, `paymentId:Guid?`, `paymentStatus:string?` | payment fields nullable | `{ "statusCode":200,"success":true,"message":"Payment confirmed" }` |
| `SePayWebhookDto` | `id`, `gateway?`, `transactionDate?`, `accountNumber?`, `subAccount?`, `transferType?`, `transferAmount`, `accumulated`, `content`, `referenceCode?`, `description?` | nullable provider fields | `{ "id":1,"transferType":"in","transferAmount":180000,"content":"CMDEP-2026-0009" }` |
| `SePayQrResponseDto` | `vietQrUrl`, `amount`, `description`, `paymentId`, `status`, `bankInfo` | bankInfo object | `{ "vietQrUrl":"https://vietqr.app/img?...","description":"CMDEP-..." }` |
| `BankInfoDto` | `bankId`, `accountNo`, `accountName` | strings | `{ "bankId":"...","accountNo":"...","accountName":"..." }` |

Enums:

- `PaymentMethod`: `Cash`, `MoMo`, `VNPay`, `SePay`.
- `PaymentStatus`: `Pending`, `Success`, `Failed`, `Refunded`.
- `PaymentType`: `Deposit`, `Final`, `Refund`.

### Chat and Notification DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `CreateChatRoomRequestDto` | `customerId`, `ownerId`, `venueId?`, `bookingId?` | `venueId`/`bookingId` nullable | `{ "venueId":"..." }` |
| `ChatRoomDto` | `roomId`, `customerId`, `hostId`, `createdAt`, `customerName?`, `hostName?`, `lastMessagePreview?`, `lastMessageTime?` | display fields nullable | `{ "roomId":"...","lastMessagePreview":"Hello" }` |
| `MessageDto` | `messageId`, `roomId`, `senderId`, `senderName?`, `messageText`, `isRead`, `sentAt` | `senderName` nullable; POST uses `messageText` | `{ "messageText":"Hello" }` |
| `NotificationDto` | `notificationId`, `userId`, `title`, `message`, `isRead`, `type`, `refId`, `readAt?`, `createdAt` | `readAt` nullable | `{ "title":"New message","isRead":false,"type":"Chat" }` |

### Discount DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `DiscountDto` | `discountId`, `ownerId`, `fieldId?`, `code`, `name`, `discountType`, `value`, `minBookingAmount`, `maxDiscountAmount`, `usageLimit`, `usedCount`, `startDate`, `endDate`, `isActive` | `fieldId` nullable | `{ "code":"SAVE10","discountType":"Percentage","value":10,"isActive":true }` |
| `ValidateDiscountRequestDto` | `code`, `fieldId?`, `slotIds:Guid[]`, `totalAmount` | `fieldId` nullable; slotIds default empty | `{ "code":"SAVE10","slotIds":["..."],"totalAmount":0 }` |
| `ValidateDiscountResponseDto` | `isValid`, `message`, `discountId?`, `discountAmount`, `finalAmount` | `discountId` nullable when invalid | `{ "isValid":true,"discountAmount":20000,"finalAmount":180000 }` |
| `UpdateStatusDto` | `isActive` | bool | `{ "isActive":false }` |

Enums:

- `DiscountType`: `Percentage`, `Fixed`.

### Review DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `CreateReviewRequestDto` | `venueId`, `bookingId`, `rating`, `comment?` | comment nullable; rating 1-5 | `{ "venueId":"...","bookingId":"...","rating":5,"comment":"Good" }` |
| `UpdateReviewRequestDto` | `rating`, `comment?` | comment nullable; rating 1-5 | `{ "rating":4,"comment":"Updated" }` |
| `ReviewDto` | `reviewId`, `userId`, `userName?`, `venueId`, `bookingId`, `rating`, `comment?`, `venueName?`, `createdAt` | display/comment nullable | `{ "reviewId":"...","rating":5,"comment":"Good" }` |
| `VenueReviewsResponseDto` | `reviews`, `totalCount`, `averageRating`, `page`, `pageSize` | reviews default empty | `{ "reviews":[],"totalCount":0,"averageRating":0 }` |

### Owner/Admin DTOs

| DTO | Fields | Required/Nullable | Example |
| --- | --- | --- | --- |
| `OwnerStatsDto` | `totalVenues`, `totalFields`, `pendingBookings`, `acceptedBookings`, `completedBookings`, `totalRevenue`, `depositRevenue`, `finalPaymentRevenue` | numbers | `{ "totalVenues":2,"totalRevenue":500000 }` |
| `OwnerRevenueDto` | `key`, `revenue`, `payments` | strings/numbers | `{ "key":"2026-01","revenue":1000000,"payments":4 }` |
| `StatusResultDto` | `id`, `status?`, `isActive?` | status/isActive nullable | `{ "id":"...","isActive":true }` |
| `BulkCreateSlotsResultDto` | `createdSlots` | int | `{ "createdSlots":12 }` |
| `VenueAmenityRequestDto` | `amenityId?`, `amenityIds:Guid[]` | supports single or list | `{ "amenityIds":["..."] }` |
| `AdminUserDto` | `id`, `fullName`, `email?`, `phone?`, `isActive`, `loyaltyPoints` | not exposed by controller currently | Handler only |
| `UpdateUserRoleDto` | `role` | not exposed by controller currently | Handler only |
| `BroadcastNotificationDto` | `title`, `message`, `refId?` | not exposed by controller currently | Handler only |

## SECTION 6 - XÁC THỰC

### Login flow

1. POST `/api/v1/auth/login`.
2. On `success=true`, store `accessToken`, `refreshToken`, and `user`.
3. Add `Authorization: Bearer <accessToken>` for protected REST calls.
4. For SignalR hubs, pass token as access token query or configured SignalR accessTokenFactory.

### Refresh token flow

1. Keep old/expired access token.
2. POST `/api/v1/auth/refresh-token`.
3. Header: `Authorization: Bearer <oldAccessToken>`.
4. Body: `{ "refreshToken": "<refreshToken>" }`.
5. Replace both access and refresh token from response.

### Logout flow

1. POST `/api/v1/auth/logout` with Bearer token.
2. Backend xóa refresh token đã lưu.
3. Frontend xóa local/session storage.

### Password reset flow

Handler và DTO có tồn tại, nhưng các method controller đang là `[NonAction]`. Frontend không thể gọi reset password/change password qua route API hiện tại.

### JWT structure

Access token claims from `JwtTokenService`:

- `ClaimTypes.NameIdentifier`: user id.
- `ClaimTypes.Email`: email.
- `ClaimTypes.Name`: full name.
- `PhoneNumber`: phone number.
- `ClaimTypes.Role`: one claim per role.

Refresh token JWT claims:

- `ClaimTypes.NameIdentifier`.
- `ClaimTypes.Email`.
- `ClaimTypes.Name`.
- `token_type = refresh`.
- `ClaimTypes.Role`.

### Authorization rules

- Public: venues, fields, public slots, public reviews, auth login/register/refresh, payment callbacks/webhook, booking health.
- Bearer required: bookings, chat, notifications, payments except callbacks, discounts validate, user profile.
- Owner/Admin role: owner controller, time slot create/update/delete, review delete.
- Chỉ Owner: owner venues controller và owner fields controller.
- Chỉ Admin: `/api/Admin/stats`; `/api/Admin/test-role` kiểm tra admin claim bằng code.

### Ghi chú triển khai frontend

- Không gọi `GET /api/v1/auth/me` vì không expose.
- Sau login/register, response auth da co user info.
- Sau login/register, response auth đã có user info.
- Nếu cần reload profile từ server, current API chỉ có `PUT /api/v1/users/profile`, không có GET profile.
- Cần handle 401 bằng refresh token flow; nếu refresh fail thì logout.
- SignalR reconnect nên gọi lại unread count và join lại room đang mở.

## SECTION 7 - XỬ LÝ LỖI

### Format lỗi global

Global middleware trả:

```json
{
  "message": "Invalid input provided.",
  "details": "Specific exception message",
  "timestamp": "2026-06-04T00:00:00Z"
}
```

| HTTP Status | Nguồn | Format message | Cách frontend xử lý |
| --- | --- | --- | --- |
| 400 | `ArgumentException`, `InvalidOperationException`, `ValidationException` | `{ message, details, timestamp }` hoặc controller wrapper | Hiển thị lỗi validation/nghiệp vụ gần form hoặc action vừa gọi |
| 401 | `UnauthorizedAccessException`, auth middleware, invalid token | `{ message, details, timestamp }`, `ProblemDetails`, or auth DTO | Try refresh once, then logout |
| 403 | `ForbiddenException`, role authorization, `Forbid()` | `{ message, details, timestamp }` hoặc `ProblemDetails` | Hiển thị trạng thái không có quyền |
| 404 | `NotFoundException`, `KeyNotFoundException`, controller catch | `{ message, details, timestamp }` hoặc wrapper | Hiển thị trạng thái không tìm thấy hoặc empty state |
| 429 | Rate limiter | source không khai báo custom body | Hiển thị "too many requests", cho phép thử lại sau |
| 500 | unhandled exception | `{ message:"An internal server error occurred.", details, timestamp }` | Hiển thị lỗi chung và log trace phía client nếu cần |

### Controller wrapper error examples

```json
{ "success": false, "message": "Rating must be between 1 and 5." }
```

```json
{
  "success": false,
  "message": "Failed to create booking",
  "errors": ["..."]
}
```

### ASP.NET ProblemDetails

Auth/authorization/model binding can return:

```json
{
  "type": "...",
  "title": "...",
  "status": 401,
  "detail": "...",
  "instance": "..."
}
```

Frontend nên normalize tất cả error shape thành:

- `status`
- `message`
- `details/errors`
- `raw`

## SECTION 8 - UPLOAD FILE

### Upload ảnh venue

- Endpoint: POST `/api/v1/owner/venues/{id}/images`.
- Auth: Bearer role `Owner`.
- Content-Type: `multipart/form-data`.
- Form field: `images`.
- Type: `List<IFormFile>`.
- Accepted file types: không khai báo trong source.
- Maximum file size: không khai báo trong source.
- Empty images list: trả 400 `{ success:false, message:"No images provided" }`.
- Storage service nhận stream, file name, content type, folder `venues/{venueId}`.
- Response:

```json
{
  "success": true,
  "message": "Images uploaded successfully",
  "data": ["https://..."],
  "errors": []
}
```

## SECTION 9 - CHECKLIST FRONTEND THỰC TẾ

### Trang chủ public / Danh sách venue

- APIs: GET `/api/v1/Venues`, GET `/api/v1/amenities`.
- Permissions: Public.
- Dữ liệu: danh sách venue, bộ lọc, amenities.
- Hành động: tìm kiếm/lọc/sắp xếp/phân trang.
- Response mong đợi: wrapper với `PagedResult<VenueDto>`.

### Chi tiết venue

- APIs: GET `/api/v1/Venues/{id}`, GET `/api/v1/Venues/{id}/fields`, GET `/api/v1/Venues/{id}/amenities`, GET `/api/v1/Venues/{id}/images`, GET `/api/v1/reviews/venue/{id}`.
- Permissions: Public.
- Dữ liệu: chi tiết venue, images, amenities, fields, reviews.
- Hành động: chọn field, mở chat nếu đã đăng nhập.
- Response mong đợi: wrapper data objects/lists.

### Map Nearby

- APIs: GET `/api/v1/Venues/map/nearby`.
- Permissions: Public.
- Dữ liệu: lat/lng/radius.
- Hành động: load venues quanh điểm hiện tại trên map.
- Response mong đợi: wrapper với `VenueDto[]`.

### Field / Slot Selection

- APIs: GET `/api/v1/fields/{id}`, GET `/api/v1/fields/{id}/slots`, GET `/api/v1/slots/available`, POST `/api/v1/slots/{id}/lock`, POST `/api/v1/slots/{id}/unlock`.
- Permissions: public read; lock/unlock require login.
- Dữ liệu: field, ngày, slot status.
- Hành động: chọn slot, lock slot, thả slot.
- Response mong đợi: wrapper data và lock success.

### Register Page

- APIs: POST `/api/v1/auth/register`.
- Permissions: Public.
- Dữ liệu: fullName, email, phoneNumber, password, confirmPassword.
- Hành động: gửi form đăng ký.
- Response mong đợi: `AuthResponseDto`.

### Login Page

- APIs: POST `/api/v1/auth/login`.
- Permissions: Public.
- Dữ liệu: email, password.
- Hành động: gửi form đăng nhập.
- Response mong đợi: `AuthResponseDto`.

### Profile Page

- APIs: PUT `/api/v1/users/profile`.
- Permissions: Bearer.
- Dữ liệu: current auth user và các field cho phép sửa: fullName, phone, avatarUrl.
- Hành động: cập nhật profile.
- Response mong đợi: wrapper với `UserDto`.

### Booking Checkout

- APIs: POST `/api/v1/discounts/validate`, POST `/api/v1/bookings`, GET `/api/v1/bookings/{id}`.
- Permissions: Bearer.
- Dữ liệu: `slotIds` đã chọn, discount tùy chọn, note.
- Hành động: validate discount, tạo booking.
- Response mong đợi: booking wrapper với status `Pending`.

### Booking History

- APIs: GET `/api/v1/bookings/history`, GET `/api/v1/bookings/{id}`, PUT `/api/v1/bookings/{id}/cancel`, GET `/api/v1/bookings/{id}/review`.
- Permissions: Bearer.
- Dữ liệu: danh sách booking, chi tiết booking, trạng thái review.
- Hành động: lọc lịch sử, hủy booking hợp lệ, đi tới payment/review.
- Response mong đợi: `BookingDto[]`, `BookingDto`, success wrapper.

### Payment Page

- APIs: POST `/api/v1/payments/deposit`, POST `/api/v1/payments/final`, GET `/api/v1/payments/{id}`, GET `/api/v1/payments/{id}/sepay-qr`.
- Permissions: Bearer.
- Dữ liệu: bookingId, paymentMethod, transactionCode.
- Hành động: tạo payment, hiển thị QR/checkout, poll payment detail nếu cần.
- Response mong đợi: `PaymentDto`, `SePayQrResponseDto`.

### Review Page

- APIs: POST `/api/v1/reviews`, PUT `/api/v1/reviews/{id}`, GET `/api/v1/reviews/my-reviews`.
- Permissions: Bearer.
- Dữ liệu: bookingId, venueId, rating, comment.
- Hành động: tạo/cập nhật review.
- Response mong đợi: review wrapper.

### Chat Page

- APIs: GET `/api/v1/chats/rooms`, POST `/api/v1/chats/rooms`, GET `/api/v1/chats/rooms/{roomId}/messages`, POST `/api/v1/chats/rooms/{roomId}/messages`, PUT `/api/v1/chats/rooms/{roomId}/read`.
- SignalR: `/hubs/chat`.
- Permissions: Bearer.
- Dữ liệu: rooms, messages, active room.
- Hành động: tạo/mở room, join hub room, gửi message, mark read.
- Response mong đợi: room/message DTOs và realtime events.

### Notifications

- APIs: GET `/api/v1/notifications`, GET `/api/v1/notifications/unread-count`, PUT `/api/v1/notifications/{id}/read`, PUT `/api/v1/notifications/read-all`.
- SignalR: `/hubs/notifications`.
- Permissions: Bearer.
- Dữ liệu: danh sách notification, unread count.
- Hành động: đánh dấu một/tất cả là đã đọc.
- Response mong đợi: notification DTOs, unread count, realtime events.

### Owner Dashboard

- APIs: GET `/api/v1/owner/stats`, GET `/api/v1/owner/revenue`.
- Permissions: Owner/Admin for `OwnerController`.
- Dữ liệu: stats và revenue group.
- Hành động: lọc doanh thu theo ngày/group.
- Response mong đợi: `OwnerStatsDto`, `OwnerRevenueDto[]`.

### Owner Venue Management

- APIs: GET `/api/v1/owner/venues`, POST `/api/v1/owner/venues`, PUT `/api/v1/owner/venues/{id}`, PUT `/api/v1/owner/venues/{id}/status`, POST `/api/v1/owner/venues/{id}/images`, DELETE `/api/v1/owner/venues/{id}/images/{imageId}`.
- Permissions: Owner.
- Dữ liệu: owner venues, form data, images.
- Hành động: tạo/cập nhật/bật tắt/upload/xóa image.
- Response mong đợi: wrappers với venue/image data.

### Owner Field and Slot Management

- APIs: GET `/api/v1/owner/venues/{venueId}/fields`, POST `/api/v1/owner/venues/{venueId}/fields`, PUT `/api/v1/owner/fields/{id}`, PUT `/api/v1/owner/fields/{id}/status`, POST `/api/v1/owner/fields/{id}/slots/bulk`, PUT `/api/v1/owner/slots/{id}`, PUT `/api/v1/owner/slots/{id}/status`, DELETE `/api/v1/owner/slots/{id}`.
- Permissions: Owner routes and Owner/Admin for `OwnerController`.
- Dữ liệu: fields, slots, bulk slot form.
- Hành động: field CRUD/status, slot bulk/create/update/status/delete.
- Response mong đợi: field/slot DTOs và status results.

### Owner Booking Management

- APIs: GET `/api/v1/owner/bookings/pending`, GET `/api/v1/owner/bookings`, GET `/api/v1/owner/bookings/{id}`, PUT `/api/v1/owner/bookings/{id}/accept`, PUT `/api/v1/owner/bookings/{id}/reject`, PUT `/api/v1/owner/bookings/{id}/complete`.
- Permissions: Owner/Admin.
- Dữ liệu: danh sách booking, chi tiết booking.
- Hành động: accept/reject/complete.
- Response mong đợi: booking DTOs và success/status result.

### Owner Discount Management

- APIs: GET `/api/v1/owner/discounts`, POST `/api/v1/owner/discounts`, PUT `/api/v1/owner/discounts/{id}`, PUT `/api/v1/owner/discounts/{id}/status`, DELETE `/api/v1/owner/discounts/{id}`.
- Permissions: Owner/Admin.
- Dữ liệu: form/list discount.
- Hành động: tạo/cập nhật/bật tắt/xóa.
- Response mong đợi: discount DTOs/status.

### Admin Diagnostic

- APIs: GET `/api/Admin/stats`, GET `/api/Admin/courts`, POST `/api/Admin/test-role`.
- Permissions: Admin for stats/test-role, Admin/Owner for courts.
- Dữ liệu: placeholder data.
- Hành động: chỉ dùng để diagnostic.
- Response mong đợi: hardcoded objects. Đánh dấu Incomplete.

### Payment Gateway / Webhook

- APIs: POST `/api/v1/payments/callback/{gateway}`, POST `/api/v1/payments/webhook/sepay`.
- Permissions: Public; SePay webhook requires `X-API-Key`.
- Dữ liệu: provider callback/webhook payload.
- Hành động người dùng: không có UI trực tiếp cho user; dùng cho gateway/server integration.
- Response mong đợi: `PaymentGatewayCallbackResultDto`.

## SECTION 10 - THỨ TỰ IMPLEMENT FRONTEND

### Phase 1

- API client wrapper with auth header, error normalization, refresh token flow.
- Login/register/logout.
- Public venue list/search/detail.
- Field and slot read screens.

### Phase 2

- Slot lock/unlock.
- Booking checkout.
- Booking history/detail/cancel.
- Discount validation.

### Phase 3

- Payment deposit/final.
- SePay QR/checkout UI.
- Payment history/detail.
- Review create/update/list.

### Phase 4

- Chat UI with SignalR `/hubs/chat`.
- Notification bell/list with SignalR `/hubs/notifications`.
- Reconnect behavior and unread count sync.

### Phase 5

- Owner dashboard stats/revenue.
- Owner venue CRUD, image upload.
- Owner field/slot management.
- Owner booking approval.
- Owner discount management.

### Phase 6

- Admin diagnostic pages only if needed.
- Payment gateway/webhook testing tools only for dev/admin environments.
