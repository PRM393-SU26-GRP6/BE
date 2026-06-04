# Tài Liệu Tích Hợp Frontend

Tài liệu này được tạo từ source code trong `CourtManager.APIs`, `CourtManager.Application`, `CourtManager.Domain`, `CourtManager.Infrastructure`.

Quy ước quan trọng:

- Tên route, DTO, field JSON, enum và method SignalR được giữ nguyên vì frontend phải gọi đúng contract.
- Không thấy Minimal API hoặc Route Group trong source; các REST API đang expose qua Controller.
- SignalR hub được map trong `Program.cs`: `/hubs/chat` và `/hubs/notifications`.
- Nếu code có handler nhưng action controller là `[NonAction]`, tài liệu ghi rõ là chưa expose HTTP endpoint.

## PHẦN 1 - TỔNG QUAN HỆ THỐNG

### Tên dự án

Court Manager API

### Mục đích hệ thống

Backend quản lý đặt sân thể thao, gồm tìm kiếm địa điểm, sân bóng, khung giờ, đặt lịch, thanh toán, đánh giá, chat realtime và thông báo realtime.

### Module chính

| Module | Source | Chức năng |
| --- | --- | --- |
| Authentication | `AuthController`, `Features/Auth` | Đăng ký, đăng nhập, refresh token, logout, cập nhật profile. Change password/forgot/reset password có code nhưng action là `[NonAction]`. |
| Venues | `VenuesController`, `OwnerVenuesController`, `Features/Venues` | Khách xem venue công khai; owner quản lý venue của mình. |
| Fields | `FieldsController`, `OwnerFieldsController`, `OwnerController`, `Features/Fields`, `Features/FootballFields` | Xem sân, quản lý sân. |
| Time Slots | `TimeSlotsController`, `OwnerController`, `Features/TimeSlots` | Xem slot, lock/unlock, tạo/sửa/xóa slot. |
| Bookings | `BookingsController`, `OwnerController`, `Features/Bookings` | User đặt sân; owner accept/reject/complete booking. |
| Payments | `PaymentsController`, `Features/Payments` | Tạo thanh toán deposit/final, refund, callback, SePay webhook/QR/checkout. |
| Discounts | `DiscountsController`, `OwnerController`, `Features/Discounts` | Validate mã giảm giá và owner quản lý discount. |
| Reviews | `ReviewsController`, `Features/Reviews` | Xem review công khai, user tạo/sửa review, owner/admin xóa review. |
| Chats | `ChatsController`, `ChatHub`, `Features/Chats` | Chat room, message, typing, realtime message/read event. |
| Notifications | `NotificationsController`, `NotificationHub`, `Features/Notifications` | Danh sách thông báo, unread count, mark read, realtime notification. |
| Admin | `AdminController`, `Features/Admin` | Controller hiện expose các endpoint placeholder; một số admin handler chưa có route. |

### Role người dùng

Role seed trong `RoleConfiguration`:

| Role | Mô tả trong source |
| --- | --- |
| Admin | Administrator with full access |
| Owner | Venue and field owner |
| User | Regular booking user |

`RoleType` enum có thêm `Guest`, nhưng seed mặc định chỉ có `Admin`, `Owner`, `User`.

### Mô hình phân quyền

| Loại phân quyền | Cách source đang xử lý |
| --- | --- |
| Công khai | Không có `[Authorize]` hoặc có `[AllowAnonymous]`. |
| Đã đăng nhập | Có `[Authorize]`; cần JWT hợp lệ. |
| Theo role | `[Authorize(Roles = "Admin")]`, `[Authorize(Roles = "Owner")]`, `[Authorize(Roles = "Owner,Admin")]`. |
| Theo ownership | Handler kiểm tra user có sở hữu booking, venue, field, slot, discount, chat room, payment, review hay không. |
| SePay webhook | `[AllowAnonymous]` nhưng bắt buộc header `X-API-Key`. |
| Admin test role | `POST /api/Admin/test-role` tự kiểm tra role claim phải là `Admin`. |

## PHẦN 2 - LUỒNG NGHIỆP VỤ

### Đăng Ký Và Đăng Nhập

Mục tiêu: Guest tạo tài khoản và lấy JWT.

Actor: Guest.

Điều kiện trước:

- Email chưa tồn tại.
- Số điện thoại chưa tồn tại.
- Password đúng rule.

Luồng:

1. Frontend gọi `POST /api/v1/auth/register`.
2. Backend kiểm tra password confirm, email, phone, tạo user active, gán role `User`.
3. Backend trả `accessToken`, `refreshToken`, `user`.
4. Khi login, frontend gọi `POST /api/v1/auth/login`.
5. Backend kiểm tra account active và password.
6. Frontend lưu token và role.

API liên quan:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`

### Refresh Token Và Logout

Mục tiêu: Duy trì session và revoke refresh token khi logout.

Actor: User đã đăng nhập.

Luồng:

1. Khi access token hết hạn, gọi `POST /api/v1/auth/refresh-token`.
2. Header phải có `Authorization: Bearer <oldAccessToken>`.
3. Body gửi `{ "refreshToken": "..." }`.
4. Backend validate access token hết hạn, refresh token đang lưu trong DB và hạn refresh token.
5. Backend trả token mới.
6. Khi logout, gọi `POST /api/v1/auth/logout`.
7. Backend xóa refresh token trong user.

API liên quan:

- `POST /api/v1/auth/refresh-token`
- `POST /api/v1/auth/logout`

### Xem Venue Công Khai

Mục tiêu: Guest hoặc user xem danh sách sân/venue trước khi booking.

Actor: Guest, User, Owner, Admin.

Luồng:

1. Gọi `GET /api/v1/Venues` để lấy danh sách venue phân trang.
2. Gọi `GET /api/v1/Venues/search` để search theo keyword.
3. Gọi `GET /api/v1/Venues/map/nearby` để tìm quanh vị trí.
4. Gọi `GET /api/v1/Venues/{id}` để xem chi tiết venue.
5. Gọi các endpoint fields, amenities, images của venue.
6. Gọi endpoint field/slot để xem lịch trống.

API liên quan:

- `GET /api/v1/Venues`
- `GET /api/v1/Venues/{id}`
- `GET /api/v1/Venues/{id}/fields`
- `GET /api/v1/Venues/{id}/amenities`
- `GET /api/v1/Venues/{id}/images`
- `GET /api/v1/Venues/search`
- `GET /api/v1/Venues/map/nearby`
- `GET /api/v1/fields/{id}`
- `GET /api/v1/fields/{id}/slots`
- `GET /api/v1/slots`
- `GET /api/v1/slots/available`
- `GET /api/v1/amenities`

### Lấy Tất Cả Venues Cho Frontend

Mục tiêu: Frontend cần load toàn bộ venue từ API hiện có.

Actor: Guest hoặc bất kỳ user nào.

Kết luận từ source:

- Không có endpoint riêng kiểu `GET /api/v1/Venues/all`.
- Endpoint thật là `GET /api/v1/Venues` và response là `PagedResult<VenueDto>`.
- Muốn lấy hết venue thì frontend phải gọi phân trang từ `page = 1` tới `data.totalPages`, sau đó gộp `data.items`.

Ví dụ request:

```http
GET /api/v1/Venues?page=1&pageSize=10
GET /api/v1/Venues?page=2&pageSize=10
GET /api/v1/Venues?page=3&pageSize=10
```

Ví dụ TypeScript:

```ts
type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
};

type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

async function getAllVenues(apiBaseUrl: string) {
  const pageSize = 10;
  let page = 1;
  let totalPages = 1;
  const venues = [];

  do {
    const res = await fetch(`${apiBaseUrl}/api/v1/Venues?page=${page}&pageSize=${pageSize}`);
    const payload = await res.json() as ApiResponse<PagedResult<unknown>>;

    if (!res.ok || !payload.success) {
      throw new Error(payload.message || "Không tải được danh sách venue");
    }

    venues.push(...payload.data.items);
    totalPages = payload.data.totalPages;
    page += 1;
  } while (page <= totalPages);

  return venues;
}
```

Nếu có filter/search, frontend phải truyền cùng query cho mọi page:

```http
GET /api/v1/Venues?q=abc&priceMin=100000&priceMax=300000&page=1&pageSize=10
GET /api/v1/Venues?q=abc&priceMin=100000&priceMax=300000&page=2&pageSize=10
```

### Booking Của Customer

Mục tiêu: User chọn slot và tạo booking.

Actor: User đã đăng nhập.

Điều kiện trước:

- Slot tồn tại.
- Slot chưa bắt đầu hoặc không ở quá khứ.
- Slot available hoặc đang locked bởi chính user.
- Tất cả slot trong một booking thuộc cùng một venue.

Luồng:

1. Xem slot trống.
2. Nếu có mã giảm giá, gọi `POST /api/v1/discounts/validate`.
3. Tạo booking bằng `POST /api/v1/bookings`.
4. Backend ép `UserId` theo JWT, không lấy từ body.
5. Backend tính total, deposit, discount.
6. Backend tạo booking status `Pending`, lock slot 15 phút, gửi notification cho owner.

API liên quan:

- `GET /api/v1/slots/available`
- `POST /api/v1/discounts/validate`
- `POST /api/v1/bookings`
- `GET /api/v1/bookings/{id}`
- `GET /api/v1/bookings/history`

### Owner Duyệt Booking

Mục tiêu: Owner accept hoặc reject booking.

Actor: Owner, Admin theo route.

Điều kiện trước:

- Booking thuộc venue của owner.
- Booking status là `Pending`.

Luồng:

1. Owner xem dashboard và booking pending.
2. Owner accept bằng `PUT /api/v1/owner/bookings/{id}/accept`.
3. Hoặc reject bằng `PUT /api/v1/owner/bookings/{id}/reject`.
4. Accept đổi status thành `Accepted`.
5. Reject đổi status thành `Rejected` và trả slot về `Available`.

API liên quan:

- `GET /api/v1/owner/stats`
- `GET /api/v1/owner/bookings/pending`
- `GET /api/v1/owner/bookings`
- `GET /api/v1/owner/bookings/{id}`
- `PUT /api/v1/owner/bookings/{id}/accept`
- `PUT /api/v1/owner/bookings/{id}/reject`
- `PUT /api/v1/owner/bookings/{id}/complete`

### Thanh Toán

Mục tiêu: Thanh toán deposit và phần còn lại của booking.

Actor: User, Owner, Admin, payment gateway, SePay.

Luồng:

1. Sau khi owner accept booking, customer gọi `POST /api/v1/payments/deposit`.
2. Nếu payment method là `Cash`, backend set payment `Success`, booking `Deposited`, slot `Booked`.
3. Nếu không phải cash, backend tạo payment `Pending`.
4. Frontend có thể lấy QR bằng `GET /api/v1/payments/{paymentId}/sepay-qr`.
5. Hoặc lấy checkout payload bằng `GET /api/v1/payments/{paymentId}/sepay-checkout`.
6. Gateway callback hoặc SePay webhook xác nhận payment.
7. Sau deposit thành công, gọi `POST /api/v1/payments/final`.
8. Cash final payment sẽ complete booking và cộng loyalty points.

API liên quan:

- `POST /api/v1/payments/deposit`
- `POST /api/v1/payments/final`
- `GET /api/v1/payments/{id}`
- `GET /api/v1/payments/history`
- `GET /api/v1/payments/{paymentId}/sepay-qr`
- `GET /api/v1/payments/{paymentId}/sepay-checkout`
- `POST /api/v1/payments/callback/{gateway}`
- `POST /api/v1/payments/webhook/sepay`

### Review

Mục tiêu: User review booking đã hoàn tất, guest xem review.

Actor:

- Guest xem review.
- User tạo/sửa review của mình.
- Owner/Admin xóa review.

Điều kiện tạo review:

- Rating từ 1 tới 5.
- Booking đã `Completed`.
- Booking thuộc user hiện tại.
- Booking thuộc venue được review.
- Booking chưa từng được review.

API liên quan:

- `GET /api/v1/reviews/venue/{id}`
- `GET /api/v1/reviews/{id}`
- `GET /api/v1/reviews/field/{fieldId}/average-rating`
- `GET /api/v1/reviews/my-reviews`
- `GET /api/v1/bookings/{id}/review`
- `POST /api/v1/reviews`
- `PUT /api/v1/reviews/{id}`
- `DELETE /api/v1/reviews/{id}`

### Owner Quản Lý Venue, Field, Slot, Discount

Mục tiêu: Owner quản lý dữ liệu sân của mình.

Actor: Owner.

Luồng:

1. Xem venue của owner.
2. Tạo/sửa venue.
3. Upload/xóa ảnh venue.
4. Bật/tắt venue; nếu còn booking active thì không cho deactivate.
5. Thêm/xóa amenities.
6. Tạo/sửa/bật tắt field.
7. Tạo/sửa/bật tắt/xóa slot hoặc bulk create slot.
8. Tạo/sửa/bật tắt/xóa discount.

API liên quan:

- `/api/v1/owner/venues`
- `/api/v1/owner/venues/{id}/images`
- `/api/v1/owner/venues/{id}/status`
- `/api/v1/owner/venues/{id}/amenities`
- `/api/v1/owner/venues/{venueId}/fields`
- `/api/v1/owner/fields/{id}`
- `/api/v1/owner/slots/{id}`
- `/api/v1/owner/discounts`

### Chat Và Notification Realtime

Mục tiêu: Chat thật và badge notification cập nhật realtime.

Actor: User đã đăng nhập và là participant của chat room.

Luồng:

1. Tạo hoặc lấy chat room bằng REST.
2. Connect `/hubs/chat?access_token=<jwt>`.
3. Connect `/hubs/notifications?access_token=<jwt>`.
4. Gọi hub `JoinRoom(roomId)`.
5. Gửi message qua REST hoặc SignalR.
6. Lắng nghe event `chat.messageCreated`, `chat.roomUpdated`, `notification.unreadCountChanged`.
7. Dùng typing event khi user nhập.
8. Mark read để cập nhật unread count.

API liên quan:

- `GET /api/v1/chats/rooms`
- `POST /api/v1/chats/rooms`
- `GET /api/v1/chats/rooms/{roomId}/messages`
- `POST /api/v1/chats/rooms/{roomId}/messages`
- `PUT /api/v1/chats/rooms/{roomId}/read`
- `GET /api/v1/notifications`
- `GET /api/v1/notifications/unread-count`
- `PUT /api/v1/notifications/{id}/read`
- `PUT /api/v1/notifications/read-all`
- `/hubs/chat`
- `/hubs/notifications`

## PHẦN 3 - MA TRẬN PHÂN QUYỀN

Ký hiệu:

- `✓`: được gọi theo route authorization.
- `Own`: phải đúng owner/customer/participant theo handler.
- `Key`: cần API key.
- `-`: code có thể tồn tại nhưng không expose route.

| Feature | Guest | User | Owner | Admin |
| --- | --- | --- | --- | --- |
| Register/Login/Refresh | ✓ | ✓ | ✓ | ✓ |
| Logout | ✗ | ✓ | ✓ | ✓ |
| Update profile | ✗ | ✓ | ✓ | ✓ |
| Xem venue/search/map | ✓ | ✓ | ✓ | ✓ |
| Xem fields/slots/amenities công khai | ✓ | ✓ | ✓ | ✓ |
| Lock/unlock slot bằng `/api/v1/slots/{id}` | ✗ | Own | Own | Own |
| Tạo booking | ✗ | ✓ | ✓ | ✓ |
| Xem booking | ✗ | Own | Own | ✓ |
| Booking history | ✗ | ✓ | ✓ | ✓ |
| Cancel booking | ✗ | Own | Own nếu là customer booking | Own nếu là customer booking |
| Owner dashboard/bookings | ✗ | ✗ | ✓ | ✓ theo route, handler có rule owner |
| Owner venues `/api/v1/owner/venues` | ✗ | ✗ | ✓ | ✗ |
| Owner fields `/api/v1/owner/fields` | ✗ | ✗ | ✓ | ✗ |
| Owner aggregate `/api/v1/owner` | ✗ | ✗ | ✓ | ✓ |
| Payment history | ✗ | ✓ | ✓ | ✓ |
| Xem payment | ✗ | Own | ✓ | ✓ |
| Deposit payment | ✗ | Own booking customer | Own booking customer | Own booking customer |
| Final payment | ✗ | Own booking customer | Booking owner | ✓ |
| Refund payment | ✗ | Own hoặc theo handler | ✓ | ✓ |
| Gateway callback | ✓ | ✓ | ✓ | ✓ |
| SePay webhook | Key | Key | Key | Key |
| Validate discount | ✗ | ✓ | ✓ | ✓ |
| Owner discount CRUD | ✗ | ✗ | ✓ | ✓ |
| Xem review công khai | ✓ | ✓ | ✓ | ✓ |
| Tạo/sửa review | ✗ | Own | Own | Own |
| Xóa review | ✗ | ✗ | ✓ | ✓ |
| Chat | ✗ | Participant | Participant | Participant |
| Notifications | ✗ | ✓ | ✓ | ✓ |
| Admin stats | ✗ | ✗ | ✗ | ✓ |
| Admin courts | ✗ | ✗ | ✓ | ✓ |
| Admin handlers chưa expose | - | - | - | - |

## PHẦN 4 - DANH MỤC API

Header dùng chung:

| Header | Khi nào cần | Giá trị |
| --- | --- | --- |
| `Authorization` | Route có `[Authorize]` | `Bearer <accessToken>` |
| `Content-Type` | Body JSON | `application/json` |
| `Content-Type` | Upload ảnh venue | `multipart/form-data` |
| `X-API-Key` | SePay webhook | API key cấu hình trong SePay settings |

Response bọc phổ biến:

```json
{
  "success": true,
  "message": "OK",
  "data": {},
  "errors": []
}
```

Response lỗi từ middleware:

```json
{
  "message": "The requested resource was not found.",
  "details": "Venue with ID ... was not found.",
  "timestamp": "2026-06-04T00:00:00Z"
}
```

### API Xác Thực

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Register | `POST` | `/api/v1/auth/register` | Công khai | `RegisterRequestDto` | `200 AuthResponseDto` | `400 AuthResponseDto`, `429` | Tạo user active, gán role `User`, trả token. |
| Login | `POST` | `/api/v1/auth/login` | Công khai | `LoginRequestDto` | `200 AuthResponseDto` | `401 AuthResponseDto`, `429` | User phải active và password đúng. |
| Refresh token | `POST` | `/api/v1/auth/refresh-token` | Công khai nhưng cần old access token | Header `Authorization`, body `RefreshTokenRequestDto` | `200 AuthResponseDto` | `400` thiếu access token, `401` invalid | Backend validate expired access token và refresh token đang lưu. |
| Logout | `POST` | `/api/v1/auth/logout` | Đã đăng nhập | Không body | `200 AuthResponseDto` | `400`, `401` | Xóa refresh token của user. |

Validation xác thực:

- `FullName`: required, max 200.
- `Email`: required, email format.
- `PhoneNumber` hoặc `Phone`: required, regex `^\+?[1-9]\d{1,14}$`.
- `Password`: required, min 8, có uppercase, lowercase, number, special char.
- `ConfirmPassword`: phải khớp password.

Action auth chưa expose:

| Action | Trạng thái |
| --- | --- |
| `GetMe` | `[NonAction]`, chưa có HTTP route |
| `ChangePassword` | `[NonAction]`, chưa có HTTP route |
| `ForgotPassword` | `[NonAction]`, chưa có HTTP route |
| `ResetPassword` | `[NonAction]`, chưa có HTTP route |

### API User

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Update profile | `PUT` | `/api/v1/users/profile` | Đã đăng nhập | `UpdateProfileRequestDto` | Response bọc `UserDto` | `401`, `400`, lỗi global | Không update email/password. Handler sync `Phone` sang `PhoneNumber`. |

### API Venue

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| List venues | `GET` | `/api/v1/Venues` | Công khai | Query `q`, `fieldType`, `amenityIds`, `minRating`, `priceMin`, `priceMax`, `sort`, `page=1`, `pageSize=10` | Response bọc `PagedResult<VenueDto>` | Lỗi global | Handler hiện truyền `q`, price range, min rating, paging vào repository; `amenityIds` được parse nhưng không thấy truyền vào repository. |
| Get venue by ID | `GET` | `/api/v1/Venues/{id}` | Công khai | Route `id` | Response bọc venue detail | `404 { success:false, message:"Venue not found.", errors:["VENUE_NOT_FOUND"] }` | Nếu query result null thì trả 404. |
| Get venue fields | `GET` | `/api/v1/Venues/{id}/fields` | Công khai | Route `id` | Response bọc `IEnumerable<FootballFieldDto>` | Lỗi global | Lấy fields của venue. |
| Get venue amenities | `GET` | `/api/v1/Venues/{id}/amenities` | Công khai | Route `id` | Response bọc `IEnumerable<AmenityDto>` | Lỗi global | Lấy amenities của venue. |
| Get venue images | `GET` | `/api/v1/Venues/{id}/images` | Công khai | Route `id` | Response bọc `IEnumerable<VenueImageDto>` | Lỗi global | Lấy images của venue. |
| Search venues | `GET` | `/api/v1/Venues/search` | Công khai | Query `q`, `page=1`, `pageSize=10` | Response bọc `PagedResult<VenueDto>` | Lỗi global | Build `GetVenuesQuery` với `Q`, `Page`, `PageSize`. |
| Nearby venues | `GET` | `/api/v1/Venues/map/nearby` | Công khai | Query `lat`, `lng`, `radius=5.0` | Response bọc `IEnumerable<VenueDto>` | Lỗi global | `distance` được round 2 chữ số. |

Ví dụ lấy danh sách venue:

```http
GET /api/v1/Venues?page=1&pageSize=10
```

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

### API Amenity

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Get all amenities | `GET` | `/api/v1/amenities` | Công khai | Không | Response bọc `IEnumerable<AmenityDto>` | Lỗi global | Dùng `GetAllAmenitiesQuery`. |

### API Field Và Slot

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Get field by ID | `GET` | `/api/v1/fields/{id}` | Công khai | Route `id` | Response bọc `FootballFieldDto` | `404 { success:false, message:"Field not found", errors:[...] }` | Controller catch mọi exception thành field not found. |
| Get field slots | `GET` | `/api/v1/fields/{id}/slots` | Công khai | Route `id`, query `date?` | Response bọc available slots | `400 { success:false, message:"Failed to get field slots" }` | Nếu không có date thì lấy `DateTime.UtcNow.Date`. |
| Get slots by field | `GET` | `/api/v1/slots` | Công khai | Query `fieldId` | Response bọc `IEnumerable<TimeSlotDto>` | Lỗi global | Lấy tất cả slots theo field. |
| Get slot by ID | `GET` | `/api/v1/slots/{id}` | Công khai | Route `id` | Response bọc `TimeSlotDto` | `404 { success:false, message }` | Slot không tồn tại trả not found. |
| Get available slots | `GET` | `/api/v1/slots/available` | Công khai | Query `fieldId`, `date` | Response bọc `IEnumerable<TimeSlotDto>` | Lỗi global | Date set kind UTC. |
| Lock slot | `POST` | `/api/v1/slots/{id}/lock` | Đã đăng nhập | Route `id` | Response bọc data rỗng | `400`, `404`, `401` | Slot phải tồn tại, chưa bắt đầu, available hoặc lock đã hết hạn. Set `LockedBy`, `LockedUntil +15 phút`. |
| Unlock slot | `POST` | `/api/v1/slots/{id}/unlock` | Đã đăng nhập | Route `id` | Response bọc data rỗng | `400`, `404`, `401` | Chỉ user đã lock slot mới được unlock. |
| Create slot | `POST` | `/api/v1/slots` | Owner, Admin | `TimeSlotDto` | `201` response bọc `TimeSlotDto` | `400`, `401`, lỗi global | Field phải tồn tại, caller phải là owner venue, `EndTime > StartTime`. |
| Update slot | `PUT` | `/api/v1/slots/{id}` | Owner, Admin | Route `id`, body `TimeSlotDto` | Response bọc `TimeSlotDto` | `400`, `404`, `401` | Caller phải own parent venue. |
| Delete slot | `DELETE` | `/api/v1/slots/{id}` | Owner, Admin | Route `id` | `{ success, message }` | `404`, `401` | Soft delete. |

Slot status hợp lệ:

- `Available`
- `Locked`
- `Booked`

### API Booking

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Create booking | `POST` | `/api/v1/bookings` | Đã đăng nhập | `CreateBookingCommand`: `slotIds`, `discountCode?`, `note?` | `201` response bọc `BookingDto` | `400`, `404`, `401` | `UserId` bị ignore trong JSON và được set từ JWT. |
| Get booking by ID | `GET` | `/api/v1/bookings/{id}` | Đã đăng nhập | Route `id` | `BookingDto` trực tiếp | `403`, lỗi global `400/404` | User phải là customer booking, owner venue hoặc admin. |
| Booking history | `GET` | `/api/v1/bookings/history` | Đã đăng nhập | Query `status?`, `from?`, `to?`, `page=1`, `pageSize=20` | `IEnumerable<BookingDto>` trực tiếp | Lỗi global | Lấy booking của current user. |
| Get booking review | `GET` | `/api/v1/bookings/{id}/review` | Đã đăng nhập | Route `id` | Response bọc `ReviewDto` hoặc `null` | Lỗi global | Handler không check current user trong source. |
| Cancel booking | `PUT` | `/api/v1/bookings/{id}/cancel` | Đã đăng nhập | Route `id`, query `cancellationReason?` | `{ success, message }` | Lỗi global `400/404`, `401` | Chỉ customer booking cancel; status phải `Pending` hoặc `Accepted`. |
| Lock time slot for booking | `PUT` | `/api/v1/bookings/slots/{slotId}/lock` | Đã đăng nhập | Route `slotId`, query `bookingId` | `{ success, message }` | Lỗi global | Slot phải `Available`; set `Locked`, `LockedUntil +15 phút`, `LockedBy`. |
| Unlock time slot | `PUT` | `/api/v1/bookings/slots/{slotId}/unlock` | Đã đăng nhập | Route `slotId`, query `unlockReason=ManualUnlock` | `{ success, message }` | Lỗi global | Slot phải đang `Locked`; set về `Available`. |
| Health | `GET` | `/api/v1/bookings/health` | Công khai | Không | `{ status:"API is running" }` | Không thấy trong source | Health check đơn giản. |

Business rule tạo booking:

- `slotIds` không được rỗng.
- Slot không được ở quá khứ.
- Slot phải available hoặc locked bởi chính user.
- Tất cả slot phải thuộc cùng một venue.
- Nếu có discount thì discount phải active, trong thời gian hiệu lực, chưa vượt usage limit, đạt min booking amount.
- Booking mới có status `Pending`.
- Deposit amount = 50% total sau discount.
- Slot được lock 15 phút.
- Owner nhận notification `New booking request`.

### API Owner

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| Owner stats | `GET` | `/api/v1/owner/stats` | Owner, Admin | Không | `OwnerStatsDto` | Tính venue, field, pending/accepted/completed booking, revenue. |
| Owner revenue | `GET` | `/api/v1/owner/revenue` | Owner, Admin | Query `from?`, `to?`, `groupBy=day` | `IEnumerable<OwnerRevenueDto>` | `groupBy`: `day`, `month`, `venue`. |
| Add venue amenities | `POST` | `/api/v1/owner/venues/{id}/amenities` | Owner, Admin | `VenueAmenityRequestDto` | `IEnumerable<string>` | Dùng `amenityId` hoặc `amenityIds`. |
| Delete venue amenity | `DELETE` | `/api/v1/owner/venues/{id}/amenities/{amenityId}` | Owner, Admin | Route params | `{ success }` | Xóa relation venue-amenity. |
| Get fields by venue | `GET` | `/api/v1/owner/venues/{venueId}/fields` | Owner, Admin | Route `venueId` | `IEnumerable<FootballFieldDto>` | Handler không thấy ownership check. |
| Create field | `POST` | `/api/v1/owner/venues/{venueId}/fields` | Owner, Admin | `FootballFieldDto` | `201 FootballFieldDto` | Venue phải thuộc owner; field type hợp lệ. |
| Bulk create slots | `POST` | `/api/v1/owner/fields/{id}/slots/bulk` | Owner, Admin | `BulkCreateSlotsDto` | `BulkCreateSlotsResultDto` | Parse `StartTime`, `EndTime`; duration > 0. |
| Update owner slot | `PUT` | `/api/v1/owner/slots/{id}` | Owner, Admin | `TimeSlotDto` | `TimeSlotDto` | Caller phải own venue. |
| Update owner slot status | `PUT` | `/api/v1/owner/slots/{id}/status` | Owner, Admin | `UpdateSlotStatusDto` | `StatusResultDto` | Status phải `Available`, `Locked`, `Booked`. |
| Delete owner slot | `DELETE` | `/api/v1/owner/slots/{id}` | Owner, Admin | Route `id` | `{ success }` | Dùng `DeleteTimeSlotCommand`. |
| Pending bookings | `GET` | `/api/v1/owner/bookings/pending` | Owner, Admin | Không | `IEnumerable<BookingDto>` | Lấy booking pending của owner. |
| Owner bookings | `GET` | `/api/v1/owner/bookings` | Owner, Admin | Không | `IEnumerable<BookingDto>` | Lấy booking của owner. |
| Owner booking by ID | `GET` | `/api/v1/owner/bookings/{id}` | Owner, Admin | Route `id` | `BookingDto` hoặc `404` | Tìm trong list owner bookings. |
| Accept booking | `PUT` | `/api/v1/owner/bookings/{id}/accept` | Owner, Admin | Route `id` | `{ success }` | Booking phải thuộc owner venue và status `Pending`. |
| Reject booking | `PUT` | `/api/v1/owner/bookings/{id}/reject` | Owner, Admin | Route `id`, query `rejectionReason?` | `{ success }` | Reject booking pending, trả slot về `Available`. |
| Complete booking | `PUT` | `/api/v1/owner/bookings/{id}/complete` | Owner, Admin | Route `id` | `StatusResultDto` | Set booking `Completed`, slot `Booked`. |
| Owner discounts | `GET` | `/api/v1/owner/discounts` | Owner, Admin | Không | `IEnumerable<DiscountDto>` | Lấy discount của owner. |
| Create discount | `POST` | `/api/v1/owner/discounts` | Owner, Admin | `DiscountDto` | `201 DiscountDto` | Validate code/value/date/usage/type. |
| Update discount | `PUT` | `/api/v1/owner/discounts/{id}` | Owner, Admin | `DiscountDto` | `DiscountDto` | Discount phải thuộc owner. |
| Update discount status | `PUT` | `/api/v1/owner/discounts/{id}/status` | Owner, Admin | `UpdateStatusDto` | `StatusResultDto` | Toggle active. |
| Delete discount | `DELETE` | `/api/v1/owner/discounts/{id}` | Owner, Admin | Route `id` | `{ success }` | Discount phải thuộc owner. |

### API Owner Venue

Các route này nằm trong `OwnerVenuesController` và chỉ cho role `Owner`.

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| Get my venues | `GET` | `/api/v1/owner/venues` | Owner | Query `isActive?`, `page=1`, `pageSize=10` | Response bọc result | Lấy venue của current owner. |
| Create venue | `POST` | `/api/v1/owner/venues` | Owner | `CreateVenueRequestDto` | Response bọc `VenueDto` | Handler set `IsActive=true`. |
| Upload venue images | `POST` | `/api/v1/owner/venues/{id}/images` | Owner | Multipart `images` | Response bọc `List<string>` URLs | Không thấy giới hạn file type/size trong source. |
| Update venue | `PUT` | `/api/v1/owner/venues/{id}` | Owner | `UpdateVenueRequestDto` | Response bọc `VenueDto` | User phải own venue. |
| Update venue status | `PUT` | `/api/v1/owner/venues/{id}/status` | Owner | `UpdateVenueStatusRequestDto` | Response bọc `{ isActive }` | Không cho deactivate nếu còn active bookings. |
| Delete venue image | `DELETE` | `/api/v1/owner/venues/{id}/images/{imageId}` | Owner | Route params | Response bọc data rỗng | Soft delete image. |

### API Owner Field

Các route này nằm trong `OwnerFieldsController` và chỉ cho role `Owner`.

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| Update field | `PUT` | `/api/v1/owner/fields/{id}` | Owner | `UpdateFieldRequestDto` | Response bọc `FootballFieldDto` | Update `FieldName`, `FieldType`, `PricePerHour`. |
| Update field status | `PUT` | `/api/v1/owner/fields/{id}/status` | Owner | `UpdateFieldStatusRequestDto` | Response bọc `{ isActive }` | Toggle active. |

### API Discount

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Lỗi | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Validate discount | `POST` | `/api/v1/discounts/validate` | Đã đăng nhập | `ValidateDiscountRequestDto` | `ValidateDiscountResponseDto` | Discount không hợp lệ vẫn trả `200` với `isValid:false` | Nếu có `slotIds`, backend tính lại total từ giá slot. |

Action trong `DiscountsController` nhưng không expose:

- `GetMyDiscounts`
- `GetDiscount`
- `CreateDiscount`
- `UpdateDiscount`
- `DeleteDiscount`

Dùng route `/api/v1/owner/discounts` thay thế.

### API Payment

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| Payments by booking | `GET` | `/api/v1/payments/booking/{bookingId}` | Đã đăng nhập | Route `bookingId` | `IEnumerable<PaymentDto>` | Non-owner/admin chỉ xem payment booking của mình. |
| Payment by ID | `GET` | `/api/v1/payments/{id}` | Đã đăng nhập | Route `id` | `PaymentDto` | Non-owner/admin chỉ xem payment của booking mình. |
| Payment history | `GET` | `/api/v1/payments/history` | Đã đăng nhập | Query `pageNumber=1`, `pageSize=10` | `IEnumerable<PaymentDto>` | Theo current user. |
| Process deposit | `POST` | `/api/v1/payments/deposit` | Đã đăng nhập | `ProcessPaymentRequestDto` | `201 PaymentDto` | Chỉ booking customer; booking phải `Accepted`; deposit chưa success. |
| Process final | `POST` | `/api/v1/payments/final` | Đã đăng nhập | `ProcessPaymentRequestDto` | `201 PaymentDto` | Booking phải `Deposited`; customer/owner/admin được thanh toán. |
| Refund | `POST` | `/api/v1/payments/{id}/refund` | Đã đăng nhập | Route `id` | `PaymentDto` | Chỉ refund payment `Success`. |
| Gateway callback | `POST` | `/api/v1/payments/callback/{gateway}` | Công khai | Route `gateway`, body `PaymentGatewayCallbackDto` | `PaymentGatewayCallbackResultDto` | Set status theo `callback.Success`. |
| SePay webhook | `POST` | `/api/v1/payments/webhook/sepay` | Công khai + `X-API-Key` | `SePayWebhookDto` | `PaymentGatewayCallbackResultDto` | Extract `CM{TransactionCode}` từ content, amount phải khớp. |
| SePay QR | `GET` | `/api/v1/payments/{paymentId}/sepay-qr` | Đã đăng nhập | Route `paymentId` | `SePayQrResponseDto` | Tạo QR URL từ config SePay. |
| SePay checkout | `GET` | `/api/v1/payments/{paymentId}/sepay-checkout` | Đã đăng nhập | Route `paymentId` | `SePayCheckoutFormDto` | Tạo payload checkout và HMAC-SHA256 signature. |

Payment method hợp lệ:

- `Cash`
- `MoMo`
- `VNPay`
- `SePay`

Payment status:

- `Pending`
- `Success`
- `Failed`
- `Refunded`

Payment type:

- `Deposit`
- `Final`
- `Refund`

### API Review

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| Venue reviews | `GET` | `/api/v1/reviews/venue/{id}` | Công khai | Route `id`, query `page=1`, `pageSize=10` | Response bọc `VenueReviewsResponseDto` | Trả reviews, total count, average rating. |
| Review by ID | `GET` | `/api/v1/reviews/{id}` | Công khai | Route `id` | Response bọc `ReviewDto` | Not found trả `{ success:false, message }`. |
| Field average rating | `GET` | `/api/v1/reviews/field/{fieldId}/average-rating` | Công khai | Route `fieldId` | Response bọc `{ fieldId, averageRating, totalReviews }` | Tính từ tối đa 1000 review theo field. |
| My reviews | `GET` | `/api/v1/reviews/my-reviews` | Đã đăng nhập | Không | Response bọc `IEnumerable<ReviewDto>` | Reviews của current user. |
| Create review | `POST` | `/api/v1/reviews` | Đã đăng nhập | `CreateReviewRequestDto` | `201` response bọc `ReviewDto` | Rating 1-5, booking completed, đúng user, chưa review. |
| Update review | `PUT` | `/api/v1/reviews/{id}` | Đã đăng nhập | `UpdateReviewRequestDto` | Response bọc `ReviewDto` | Chỉ tác giả review được sửa. |
| Delete review | `DELETE` | `/api/v1/reviews/{id}` | Admin, Owner | Route `id` | `{ success, message }` | Soft delete. |

### API Chat

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| Get chat rooms | `GET` | `/api/v1/chats/rooms` | Đã đăng nhập | Query `pageNumber=1`, `pageSize=10` | `IEnumerable<ChatRoomDto>` | Có last message preview/time. |
| Create/get room | `POST` | `/api/v1/chats/rooms` | Đã đăng nhập | `CreateChatRoomRequestDto` | `ChatRoomDto` | Nếu có `venueId`, tạo room với owner venue. |
| Get messages | `GET` | `/api/v1/chats/rooms/{roomId}/messages` | Đã đăng nhập và là participant | Route `roomId`, query `pageNumber=1`, `pageSize=20` | `IEnumerable<MessageDto>` | Đọc messages sẽ mark room messages as read. |
| Send message | `POST` | `/api/v1/chats/rooms/{roomId}/messages` | Đã đăng nhập và là participant | Body `MessageDto`, dùng `messageText` | `201 MessageDto` | Tạo notification `Chat` cho người nhận, publish realtime event. |
| Mark room read | `PUT` | `/api/v1/chats/rooms/{roomId}/read` | Đã đăng nhập và là participant | Route `roomId` | `{ unreadCount }` | Publish `chat.messagesRead`. |

Action chat chưa expose:

- `GetOrCreateChatRoom`
- `GetOrCreateVenueChatRoom`
- `SendMessageByBody`
- `DeleteMessage`
- `CloseChatRoom`

### API Notification

| Tên | Phương thức | Đường dẫn | Quyền | Body/Tham số | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- | --- |
| List notifications | `GET` | `/api/v1/notifications` | Đã đăng nhập | Query `unreadOnly=false`, `pageNumber=1`, `pageSize=10` | `IEnumerable<NotificationDto>` | Lấy notification của current user. |
| Unread count | `GET` | `/api/v1/notifications/unread-count` | Đã đăng nhập | Không | `{ unreadCount }` | Count unread của current user. |
| Mark notification read | `PUT` | `/api/v1/notifications/{id}/read` | Đã đăng nhập | Route `id` | `{ success }` | Publish `notification.read` và count. |
| Mark all read | `PUT` | `/api/v1/notifications/read-all` | Đã đăng nhập | Không | `{ success }` | Publish `notification.readAll` và count. |

Action notification chưa expose:

- `GetNotificationById`
- `DeleteNotification`

### API Admin

Các API hiện tại trong `AdminController` là placeholder/static.

| Tên | Phương thức | Đường dẫn | Quyền | Kết quả thành công | Ghi chú |
| --- | --- | --- | --- | --- | --- |
| Admin stats | `GET` | `/api/Admin/stats` | Admin | `{ message, totalUsers:150, totalBookings:500 }` | Hardcoded trong controller. |
| Admin courts | `GET` | `/api/Admin/courts` | Admin, Owner | `{ message, courts:["Court 1","Court 2"] }` | Hardcoded trong controller. |
| Test role | `POST` | `/api/Admin/test-role` | Đã đăng nhập, tự check Admin trong action | `{ message:"You are an Admin!" }` | Nếu role claim không phải Admin thì `Forbid()`. |

Admin handler có trong `Features/Admin/AdminRequests.cs` nhưng chưa có route expose:

- `GetAdminUsersQuery`
- `UpdateAdminUserRoleCommand`
- `GetAdminVenuesQuery`
- `UpdateAdminVenueStatusCommand`
- `BroadcastNotificationCommand`

## PHẦN 5 - THAM CHIẾU DTO

Ghi chú:

- Kiểu dữ liệu ghi theo source C#.
- Field JSON thường là camelCase khi serialize.
- Các string có default `string.Empty` vẫn nên xem là required ở frontend nếu validator/handler yêu cầu.

### DTO Xác Thực

| DTO | Fields |
| --- | --- |
| `RegisterRequestDto` | `fullName: string`, `email: string`, `phoneNumber: string`, `password: string`, `confirmPassword: string` |
| `LoginRequestDto` | `email: string`, `password: string` |
| `RefreshTokenRequestDto` | `refreshToken: string` |
| `AuthResponseDto` | `success: bool`, `message: string`, `accessToken?: string`, `refreshToken?: string`, `user?: UserAuthDto` |
| `UserAuthDto` | `id: Guid`, `fullName: string`, `email: string`, `phoneNumber: string`, `roles: IEnumerable<string>` |
| `ChangePasswordRequestDto` | `currentPassword: string`, `newPassword: string`, `confirmNewPassword: string` |
| `ForgotPasswordRequestDto` | `email: string` |
| `ResetPasswordRequestDto` | `email: string`, `token: string`, `newPassword: string`, `confirmNewPassword: string` |

### DTO User

| DTO | Fields |
| --- | --- |
| `UserDto` | `id: Guid`, `fullName: string`, `email: string`, `phoneNumber: string`, `avatarUrl?: string`, `loyaltyPoints: int`, `isActive: bool`, `roles: IEnumerable<string>` |
| `UpdateProfileRequestDto` | `fullName: string`, `phone: string`, `avatarUrl?: string` |
| `UpdateUserProfileDto` | `fullName: string`, `phone: string`, `phoneNumber: string`, `avatarUrl?: string` |

### DTO Venue

| DTO | Fields |
| --- | --- |
| `VenueDto` | `venueId: Guid`, `venueName: string`, `address: string`, `latitude: decimal`, `longitude: decimal`, `distance?: double`, `description: string`, `openingHours: string`, `phoneContact: string`, `ownerName: string`, `averageRating: double`, `totalReviews: int`, `minPrice: decimal`, `maxPrice: decimal` |
| `VenueDetailDto` | Kế thừa `VenueDto`, thêm `images: List<VenueImageDto>`, `amenities: List<AmenityDto>`, `fields: List<FootballFieldDto>` |
| `CreateVenueRequestDto` | `venueName: string`, `address: string`, `latitude: decimal`, `longitude: decimal`, `description: string`, `openingHours: string`, `phoneContact: string` |
| `UpdateVenueRequestDto` | `venueName: string`, `address: string`, `latitude: decimal`, `longitude: decimal`, `description: string`, `openingHours: string`, `phoneContact: string` |
| `UpdateVenueStatusRequestDto` | `isActive: bool` |
| `VenueImageDto` | `imageId: Guid`, `imageUrl: string`, `isPrimary: bool` |
| `VenueImageRequestDto` | `imageUrl: string`, `isPrimary: bool` |
| `VenueAmenityRequestDto` | `amenityId?: Guid`, `amenityIds: List<Guid>` |

### DTO Amenity

| DTO | Fields |
| --- | --- |
| `AmenityDto` | `amenityId: Guid`, `name: string`, `icon: string` |

### DTO Field Và Slot

| DTO | Fields |
| --- | --- |
| `FootballFieldDto` | `id: Guid`, `ownerId: Guid`, `fieldName: string`, `description: string`, `fieldType: string`, `location: string`, `latitude: decimal`, `longitude: decimal`, `pricePerHour: decimal`, `isActive: bool` |
| `UpdateFieldRequestDto` | `fieldName: string`, `fieldType: FieldType`, `pricePerHour: decimal` |
| `UpdateFieldStatusRequestDto` | `isActive: bool` |
| `FieldImageDto` | `imageId: Guid`, `fieldId: Guid`, `imageUrl: string` |
| `TimeSlotDto` | `slotId: Guid`, `fieldId: Guid`, `startTime: DateTime`, `endTime: DateTime`, `price: decimal`, `slotStatus: string`, `createdAt: DateTime`, `updatedAt?: DateTime` |
| `UpdateSlotStatusDto` | `slotStatus: string` |
| `BulkCreateSlotsDto` | `fromDate: DateTime`, `toDate: DateTime`, `startTime: string`, `endTime: string`, `slotDurationMinutes: int`, `price: decimal` |
| `BulkCreateSlotsResultDto` | `createdSlots: int` |

### DTO Booking

| DTO | Fields |
| --- | --- |
| `BookingDto` | `id: Guid`, `userId: Guid`, `fieldId: Guid`, `startTime: DateTime`, `endTime: DateTime`, `totalPrice: decimal`, `depositAmount: decimal`, `discountAmount: decimal`, `bookingStatus: string`, `note?: string`, `createdAt: DateTime`, `items: IEnumerable<BookingItemDto>`, `payments: IEnumerable<PaymentDto>` |
| `BookingItemDto` | `bookingItemId: Guid`, `slotId: Guid`, `fieldId: Guid`, `fieldName?: string`, `venueId: Guid`, `venueName?: string`, `startTime: DateTime`, `endTime: DateTime`, `price: decimal` |
| `CreateBookingDto` | `userId: Guid`, `fieldId: Guid`, `startTime: DateTime`, `endTime: DateTime` |
| `BookingRequestDto` | `fieldId: Guid`, `slotIds: Guid[]`, `note?: string` |
| `CreateBookingCommand` | `slotIds: Guid[]`, `discountCode?: string`, `note?: string`; `userId` bị `[JsonIgnore]` |
| `BookingHistoryDto` | Kế thừa `BookingDto`, thêm `fieldName?: string`, `fieldLocation?: string`, `ownerName?: string`, `statusDisplay?: string`, `canCancel: bool`, `canPayment: bool`, `timeSlots?: List<TimeSlotDto>` |

### DTO Discount

| DTO | Fields |
| --- | --- |
| `DiscountDto` | `discountId: Guid`, `ownerId: Guid`, `fieldId?: Guid`, `code: string`, `name: string`, `discountType: string`, `value: decimal`, `minBookingAmount: decimal`, `maxDiscountAmount: decimal`, `usageLimit: int`, `usedCount: int`, `startDate: DateTime`, `endDate: DateTime`, `isActive: bool` |
| `ValidateDiscountRequestDto` | `code: string`, `fieldId?: Guid`, `slotIds: Guid[]`, `totalAmount: decimal` |
| `ValidateDiscountResponseDto` | `isValid: bool`, `message: string`, `discountId?: Guid`, `discountAmount: decimal`, `finalAmount: decimal` |

### DTO Payment

| DTO | Fields |
| --- | --- |
| `PaymentDto` | `id: Guid`, `bookingId: Guid`, `amount: decimal`, `paymentStatus: string`, `paymentType: string`, `paymentMethod: PaymentMethod`, `transactionCode: string`, `paidAt?: DateTime`, `paymentUrl?: string`, `bookingStatus?: string` |
| `ProcessPaymentRequestDto` | `bookingId: Guid`, `paymentMethod: PaymentMethod`, `transactionCode?: string` |
| `PaymentGatewayCallbackDto` | `transactionCode: string`, `success: bool` |
| `PaymentGatewayCallbackResultDto` | `statusCode: int`, `success: bool`, `message: string`, `paymentId?: Guid`, `paymentStatus?: string` |
| `SePayQrResponseDto` | `qrUrl: string`, `amount: decimal`, `description: string`, `paymentId: Guid`, `status: string`, `bankInfo: BankInfoDto` |
| `BankInfoDto` | `bankId: string`, `accountNo: string`, `accountName: string` |
| `SePayWebhookDto` | `id: long`, `gateway?: string`, `transactionDate?: string`, `accountNumber?: string`, `subAccount?: string`, `transferType?: string`, `transferAmount: decimal`, `accumulatedBalance: decimal`, `content: string`, `referenceCode?: string`, `description?: string` |
| `SePayCheckoutFormDto` | `payUrl`, `merchant`, `operation`, `payment_method`, `order_amount`, `currency`, `order_invoice_number`, `order_description`, `customer_id`, `success_url`, `error_url`, `cancel_url`, `signature` |
| `SePayCheckoutResponseDto` | `checkoutUrl: string`, `formFields?: Dictionary<string,string>`, `success: bool`, `message?: string` |

### DTO Review

| DTO | Fields |
| --- | --- |
| `ReviewDto` | `reviewId: Guid`, `userId: Guid`, `userName?: string`, `venueId: Guid`, `bookingId: Guid`, `rating: int`, `comment?: string`, `venueName?: string`, `createdAt: DateTime` |
| `CreateReviewRequestDto` | `venueId: Guid`, `bookingId: Guid`, `rating: int`, `comment?: string` |
| `UpdateReviewRequestDto` | `rating: int`, `comment?: string` |
| `VenueReviewsResponseDto` | `reviews: IEnumerable<ReviewDto>`, `totalCount: int`, `averageRating: decimal`, `page: int`, `pageSize: int` |

### DTO Chat Và Notification

| DTO | Fields |
| --- | --- |
| `ChatRoomDto` | `roomId: Guid`, `customerId: Guid`, `hostId: Guid`, `createdAt: DateTime`, `customerName?: string`, `hostName?: string`, `lastMessagePreview?: string`, `lastMessageTime?: DateTime` |
| `CreateChatRoomRequestDto` | `customerId: Guid`, `ownerId: Guid`, `venueId?: Guid`, `bookingId?: Guid` |
| `MessageDto` | `messageId: Guid`, `roomId: Guid`, `senderId: Guid`, `senderName?: string`, `messageText: string`, `isRead: bool`, `sentAt: DateTime` |
| `NotificationDto` | `notificationId: Guid`, `userId: Guid`, `title: string`, `message: string`, `isRead: bool`, `type: string`, `refId: string`, `readAt?: DateTime`, `createdAt: DateTime` |

### DTO Owner/Admin/Utility

| DTO | Fields |
| --- | --- |
| `OwnerStatsDto` | `totalVenues`, `totalFields`, `pendingBookings`, `acceptedBookings`, `completedBookings`, `totalRevenue`, `depositRevenue`, `finalPaymentRevenue` |
| `OwnerRevenueDto` | `key: string`, `revenue: decimal`, `payments: int` |
| `UpdateStatusDto` | `isActive: bool` |
| `StatusResultDto` | `id: Guid`, `status?: string`, `isActive?: bool` |
| `AdminUserDto` | `id`, `fullName`, `email?`, `phone?`, `isActive`, `loyaltyPoints` |
| `UpdateUserRoleDto` | `role: string` |
| `UserRoleResultDto` | `userId: Guid`, `role: string` |
| `BroadcastNotificationDto` | `title: string`, `message: string`, `refId?: string` |
| `BroadcastNotificationResultDto` | `notificationId: Guid`, `recipients: int` |
| `PagedResult<T>` | `items: IEnumerable<T>`, `page: int`, `pageSize: int`, `totalItems: int`, `totalPages: int` |
| `FileUploadDto` | `content: Stream`, `fileName: string`, `contentType: string` |

### Enum Values

| Enum | Giá trị |
| --- | --- |
| `BookingStatus` | `Pending`, `Accepted`, `Rejected`, `Deposited`, `Completed`, `Cancelled` |
| `DiscountType` | `Percentage`, `Fixed` |
| `FieldType` | `FiveASide`, `SevenASide`, `ElevenASide` |
| `NotificationType` | `Booking`, `Payment`, `Chat`, `System`, `Broadcast` |
| `PaymentMethod` | `Cash`, `MoMo`, `VNPay`, `SePay` |
| `PaymentStatus` | `Pending`, `Success`, `Failed`, `Refunded` |
| `PaymentType` | `Deposit`, `Final`, `Refund` |
| `RoleType` | `Guest`, `User`, `Owner`, `Admin` |
| `SlotStatus` | `Available`, `Locked`, `Booked` |

## PHẦN 6 - XÁC THỰC

### Luồng đăng nhập

1. Frontend gọi `POST /api/v1/auth/login`.
2. Backend kiểm tra email, active status, password.
3. Backend trả `AuthResponseDto`.
4. Frontend lưu `accessToken`, `refreshToken`, `user.roles`.
5. Với protected API, gửi header:

```http
Authorization: Bearer <accessToken>
```

### Luồng refresh token

1. Khi access token hết hạn, gọi `POST /api/v1/auth/refresh-token`.
2. Header dùng old access token.
3. Body dùng refresh token.
4. Backend trả access token và refresh token mới.
5. Frontend thay thế cả hai token.

### Luồng logout

1. Gọi `POST /api/v1/auth/logout`.
2. Backend xóa refresh token.
3. Frontend clear local auth state.

### Luồng reset password

Chưa tích hợp frontend được vì controller action là `[NonAction]`.

| Flow | Trạng thái |
| --- | --- |
| Forgot password | Có handler, chưa có route |
| Reset password | Có handler, chưa có route |
| Change password | Có handler, chưa có route |

### Cấu trúc JWT

Access token claims:

| Claim | Giá trị |
| --- | --- |
| `ClaimTypes.NameIdentifier` | `user.Id` |
| `ClaimTypes.Email` | `user.Email` |
| `ClaimTypes.Name` | `user.FullName` |
| `PhoneNumber` | `user.PhoneNumber` |
| `ClaimTypes.Role` | Một claim cho mỗi role |

Refresh token claims:

| Claim | Giá trị |
| --- | --- |
| `ClaimTypes.NameIdentifier` | `user.Id` |
| `ClaimTypes.Email` | `user.Email` |
| `ClaimTypes.Name` | `user.FullName` |
| `token_type` | `refresh` |
| `ClaimTypes.Role` | Một claim cho mỗi role |

JWT validation:

- Validate issuer.
- Validate audience.
- Validate lifetime.
- Validate signing key.
- `ClockSkew = TimeSpan.Zero`.
- Algorithm HMAC-SHA256.

SignalR dùng token qua query string:

```text
/hubs/chat?access_token=<accessToken>
/hubs/notifications?access_token=<accessToken>
```

## PHẦN 7 - XỬ LÝ LỖI

### Lỗi từ global middleware

| HTTP status | Exception trong source | Format | Frontend nên xử lý |
| --- | --- | --- | --- |
| 400 | `ArgumentException`, `InvalidOperationException` | `{ message, details, timestamp }` | Hiển thị `details` nếu có. |
| 400 | `ValidationException` | `{ message, details, timestamp }` | Hiển thị validation message. |
| 401 | `UnauthorizedAccessException` | `{ message, details, timestamp }` | Clear token hoặc redirect login. |
| 403 | `ForbiddenException` | `{ message, details, timestamp }` | Ẩn action không đủ quyền. |
| 404 | `NotFoundException`, `KeyNotFoundException` | `{ message, details, timestamp }` | Hiển thị not found state. |
| 500 | Exception khác | `{ message, details, timestamp }` | Hiển thị lỗi hệ thống. |

Ví dụ:

```json
{
  "message": "The requested resource was not found.",
  "details": "Venue with ID ... was not found.",
  "timestamp": "2026-06-04T00:00:00Z"
}
```

### Lỗi riêng từ controller

| Khu vực | Format |
| --- | --- |
| Auth | `AuthResponseDto`: `{ success, message, accessToken?, refreshToken?, user? }` |
| Wrapped controllers | `{ success, message, data?, errors? }` |
| Venue not found | `{ success:false, message:"Venue not found.", errors:["VENUE_NOT_FOUND"] }` |
| SePay invalid key | `{ success:false, message:"Invalid API key" }` |
| Payment callback | `PaymentGatewayCallbackResultDto` |
| Rate limit | `429`; body không customize trong source |

Ghi chú validation:

- Validators được register bằng `AddValidatorsFromAssembly`.
- Không thấy `IPipelineBehavior` validation behavior trong source.
- Vì vậy frontend không nên giả định mọi FluentValidation rule luôn tự chạy nếu chưa có pipeline; nhưng controller/handler rules chắc chắn có chạy theo source.

## PHẦN 8 - UPLOAD FILE

### Upload ảnh venue

| Item | Giá trị |
| --- | --- |
| Endpoint | `POST /api/v1/owner/venues/{id}/images` |
| Auth | Owner |
| Content-Type | `multipart/form-data` |
| Field form | `images` |
| Type | `List<IFormFile>` |
| Loại file được chấp nhận | Không định nghĩa trong source |
| Dung lượng tối đa | Không định nghĩa trong source |
| Validation | `images` không null/rỗng; stream không rỗng; user phải own venue |
| Storage | Cloudflare R2 qua `CloudflareR2StorageService` |
| Folder | `venues/{venueId}` |
| Response | Response bọc `List<string>` URLs |

Ví dụ:

```text
POST /api/v1/owner/venues/{id}/images
Authorization: Bearer <accessToken>
Content-Type: multipart/form-data

images: court-1.jpg
images: court-2.jpg
```

## PHẦN 9 - CHECKLIST FRONTEND THỰC TẾ

| Screen/Page | API cần dùng | Quyền | Data cần | Action | Response mong đợi |
| --- | --- | --- | --- | --- | --- |
| Register | `POST /api/v1/auth/register` | Guest | fullName, email, phone, password | Gửi form đăng ký | Token + user |
| Login | `POST /api/v1/auth/login` | Guest | email, password | Gửi form đăng nhập | Token + roles |
| Token refresh | `POST /api/v1/auth/refresh-token` | Có token | old access token, refresh token | Refresh ngầm | Token mới |
| Profile | `PUT /api/v1/users/profile` | Đã đăng nhập | fullName, phone, avatarUrl | Cập nhật profile | `UserDto` |
| Home venue list | `GET /api/v1/Venues`, `GET /api/v1/amenities` | Công khai | filters, paging | List/search/filter; muốn lấy hết thì loop `page` tới `data.totalPages` và merge `data.items` | `PagedResult<VenueDto>` |
| Venue search | `GET /api/v1/Venues/search` | Công khai | q, page, pageSize | Search | Paged venues |
| Map nearby | `GET /api/v1/Venues/map/nearby` | Công khai | lat, lng, radius | Xem venue gần đây | Venue list có distance |
| Venue detail | Venue detail/fields/amenities/images/reviews APIs | Công khai | venueId | Xem detail | Venue, fields, amenities, images, reviews |
| Field detail | `GET /api/v1/fields/{id}`, slot APIs | Công khai | fieldId, date | Xem lịch trống | Field + slots |
| Booking checkout | `POST /api/v1/discounts/validate`, `POST /api/v1/bookings` | Đã đăng nhập | slotIds, discountCode, note | Validate, create booking | Discount result, booking |
| My bookings | Booking history/detail/cancel APIs | Đã đăng nhập | filters, bookingId | Xem/cancel booking | Booking list/detail |
| Payment | Payment deposit/final/QR/checkout APIs | Đã đăng nhập | bookingId, paymentId, method | Tạo payment, hiển thị QR | Payment DTO, QR DTO |
| Reviews | Review APIs | Công khai/đăng nhập | venueId, bookingId, rating | Xem/tạo/sửa review | Review DTO |
| Chat list | Chat rooms API, `/hubs/chat` | Đã đăng nhập | paging | Xem rooms | Rooms realtime |
| Chat room | Message APIs, SignalR methods | Participant | roomId, messageText | Chat, typing, mark read | Message events |
| Notifications | Notification APIs, `/hubs/notifications` | Đã đăng nhập | paging, notificationId | Xem/mark read | Count/list/events |
| Owner dashboard | Owner stats/revenue/bookings | Owner/Admin | date, groupBy | Xem dashboard | Stats/revenue/bookings |
| Owner venues | Owner venue APIs | Owner | venue fields/files | CRUD venue | Venue DTO |
| Owner amenities | Amenity APIs | Owner/Admin | venueId, amenityIds | Add/remove amenities | Names/success |
| Owner fields | Owner field APIs | Owner/Admin | field data | CRUD/status field | Field DTO |
| Owner slots | Owner slot APIs | Owner/Admin | slot data | CRUD/status/bulk create | Slot/status DTO |
| Owner discounts | Owner discount APIs | Owner/Admin | DiscountDto | CRUD/status discount | Discount DTO |
| Admin placeholder | `/api/Admin/*` | Admin/Owner tùy route | none | Xem/test | Static data |

## PHẦN 10 - THỨ TỰ TRIỂN KHAI FRONTEND

### Phase 1 - Nền tảng

1. API client.
2. Inject bearer token.
3. Refresh-token retry.
4. Logout cleanup.
5. Error parser cho `AuthResponseDto`, response bọc `{ success, message, errors }`, middleware `{ message, details, timestamp }`.
6. Role guard từ `user.roles`.

### Phase 2 - Browse công khai

1. Venue list.
2. Get all venues bằng pagination nếu cần.
3. Venue search/filter.
4. Venue detail.
5. Field detail.
6. Available slots.

### Phase 3 - Customer flow

1. Booking từ selected slots.
2. Discount validation.
3. Booking history/detail/cancel.
4. Deposit/final payment.
5. SePay QR/checkout.
6. Review booking.

### Phase 4 - Realtime

1. Notification list/count.
2. Connect NotificationHub.
3. Chat room list/messages.
4. Connect ChatHub.
5. Typing/read/message events.

### Phase 5 - Owner

1. Owner dashboard.
2. Owner booking approval.
3. Owner venue CRUD/status/images.
4. Owner amenities.
5. Owner fields.
6. Owner slots.
7. Owner discounts.

### Phase 6 - Admin

1. Chỉ tích hợp các endpoint admin placeholder đang expose nếu cần.
2. Không build admin user/venue/broadcast screen từ handler cho tới khi backend expose route.

## THAM CHIẾU REALTIME

### URL SignalR hub

| Hub | URL | Auth |
| --- | --- | --- |
| ChatHub | `/hubs/chat?access_token=<jwt>` | `[Authorize]` |
| NotificationHub | `/hubs/notifications?access_token=<jwt>` | `[Authorize]` |

### Method từ client gọi lên ChatHub

| Method | Tham số | Hành vi |
| --- | --- | --- |
| `JoinRoom` | `roomId: Guid` | Validate participant, add connection vào group `chat-room:{roomId}`, gửi `chat.roomJoined` cho caller. |
| `LeaveRoom` | `roomId: Guid` | Remove connection khỏi room group. |
| `StartTyping` | `roomId: Guid` | Validate participant, gửi `chat.typingStarted` cho others trong room. |
| `StopTyping` | `roomId: Guid` | Validate participant, gửi `chat.typingStopped` cho others trong room. |
| `SendMessage` | `roomId: Guid`, `messageText: string` | Tạo message, publish message created, room updated, notification unread count. |
| `MarkRoomAsRead` | `roomId: Guid` | Mark read và publish `chat.messagesRead`. |

### Method từ client gọi lên NotificationHub

| Method | Tham số | Hành vi |
| --- | --- | --- |
| `GetUnreadCount` | none | Gửi `notification.unreadCountChanged` cho caller. |
| `MarkNotificationAsRead` | `notificationId: Guid` | Mark notification read, publish read/count events. |
| `MarkAllNotificationsAsRead` | none | Mark all read, publish read-all/count events. |

### Event từ server gửi xuống client

| Sự kiện | Payload |
| --- | --- |
| `chat.roomJoined` | `ChatRoomDto` |
| `chat.messageCreated` | `MessageDto` |
| `chat.roomUpdated` | `ChatRoomDto` |
| `chat.messagesRead` | `{ roomId, readerUserId, readAt, unreadCount }` |
| `chat.typingStarted` | `{ roomId, userId, connectionId, at }` |
| `chat.typingStopped` | `{ roomId, userId, connectionId, at }` |
| `chat.error` | `{ message }` |
| `notification.created` | `NotificationDto` |
| `notification.read` | `{ notificationId, readAt, unreadCount }` |
| `notification.readAll` | `{ readAt, unreadCount }` |
| `notification.unreadCountChanged` | `{ unreadCount }` |
| `notification.error` | `{ message }` |
