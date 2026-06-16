# 📡 API Documentation - Court Manager Backend

**Base URL:** `http://localhost:5234/api/v1`  
**Authentication:** Bearer Token (JWT)  
**Content-Type:** `application/json`

---

## 📋 Mục Lục

1. [Auth API](#-auth-api) - Đăng nhập, đăng ký
2. [Users API](#-users-api) - Thông tin user
3. [Venues API](#-venues-api) - Sân bóng, tìm kiếm
4. [Fields API](#-fields-api) - Field, lịch sân
5. [Slots API](#-slots-api) - Time slots, khóa slot
6. [Bookings API](#-bookings-api) - Đặt sân
7. [Payments API](#-payments-api) - Thanh toán
8. [Reviews API](#-reviews-api) - Đánh giá
9. [Notifications API](#-notifications-api) - Thông báo
10. [Owner Wallet API](#-owner-wallet-api) - Ví chủ sân
11. [Owner Withdrawal API](#-owner-withdrawal-api) - Rút tiền
12. [Admin Withdrawal API](#-admin-withdrawal-api) - Duyệt rút tiền
13. [Owner Venues API](#-owner-venues-api) - Quản lý sân
14. [Owner Fields API](#-owner-fields-api) - Quản lý field
15. [Owner Bookings API](#-owner-bookings-api) - Quản lý booking

---

## 🔐 Auth API

**Base Path:** `/auth`

### 1. Đăng ký Customer

**Endpoint:** `POST /auth/register/customer`

**Request:**
```json
{
  "fullName": "Nguyen Van A",
  "email": "user@example.com",
  "phoneNumber": "0912345678",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Registration successful. Please verify your email.",
  "data": {
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "email": "user@example.com"
  }
}
```

---

### 2. Đăng ký Owner

**Endpoint:** `POST /auth/register/owner`

**Request:** (same as customer)

**Response 200:** (same as customer)

---

### 3. Đăng nhập

**Endpoint:** `POST /auth/login`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "expiresIn": 3600,
    "tokenType": "Bearer",
    "user": {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "email": "user@example.com",
      "fullName": "Nguyen Van A",
      "role": "User"
    }
  }
}
```

---

### 4. Verify OTP

**Endpoint:** `POST /auth/verify-otp`

**Request:**
```json
{
  "email": "user@example.com",
  "otp": "123456"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Email verified successfully"
}
```

---

### 5. Refresh Token

**Endpoint:** `POST /auth/refresh-token`

**Headers:** `Authorization: Bearer <access_token>`

**Request:**
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "bmV3IHJlZnJlc2ggdG9rZW4...",
    "expiresIn": 3600,
    "tokenType": "Bearer"
  }
}
```

---

### 6. Logout

**Endpoint:** `POST /auth/logout`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

---

## 👤 Users API

**Base Path:** `/users`

### 7. Update Profile

**Endpoint:** `PUT /users/profile`  
**Auth:** Bearer Token

**Request:**
```json
{
  "fullName": "Nguyen Van B",
  "phone": "0987654321",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Profile updated successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "fullName": "Nguyen Van B",
    "email": "user@example.com",
    "phoneNumber": "0987654321",
    "avatarUrl": "https://example.com/avatar.jpg"
  }
}
```

---

## 🏟️ Venues API

**Base Path:** `/venues`

### 8. Get All Venues

**Endpoint:** `GET /venues`

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| q | string | Search query (name, address) |
| page | int | Page number (default: 1) |
| pageSize | int | Items per page (default: 10) |

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "items": [
      {
        "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
        "venueName": "Sân Bóng Đẹp",
        "address": "123 Đường ABC, Quận 1, TP.HCM",
        "latitude": 10.762917,
        "longitude": 106.68214,
        "description": "Sân bóng chất lượng cao",
        "openingHours": "06:00 - 22:00",
        "phoneContact": "0901234567",
        "averageRating": 4.5,
        "reviewCount": 120,
        "isActive": true,
        "images": ["https://example.com/img1.jpg"],
        "ownerId": "c3d4e5f6-a7b8-9012-cdef-123456789012"
      }
    ],
    "totalCount": 50,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5
  }
}
```

---

### 9. Get Venue By ID

**Endpoint:** `GET /venues/{id}`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "venueName": "Sân Bóng Đẹp",
    "address": "123 Đường ABC, Quận 1, TP.HCM",
    "latitude": 10.762917,
    "longitude": 106.68214,
    "description": "Sân bóng chất lượng cao",
    "openingHours": "06:00 - 22:00",
    "phoneContact": "0901234567",
    "averageRating": 4.5,
    "reviewCount": 120,
    "isActive": true,
    "images": [...],
    "fields": [...],
    "amenities": [...]
  }
}
```

---

### 10. Get Venue Fields

**Endpoint:** `GET /venues/{id}/fields`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "id": "d4e5f6a7-b8c9-0123-defa-234567890123",
      "fieldName": "Sân 1",
      "fieldType": "5vs5",
      "pricePerHour": 150000,
      "isActive": true,
      "venueId": "b2c3d4e5-f6a7-8901-bcde-f12345678901"
    }
  ]
}
```

---

### 11. Search Venues

**Endpoint:** `GET /venues/search?q=san bong&page=1&pageSize=10`

**Response 200:** (same as Get All Venues)

---

### 12. Get Nearby Venues

**Endpoint:** `GET /venues/map/nearby?lat=10.762917&lng=106.68214&radius=5`

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| lat | double | Latitude |
| lng | double | Longitude |
| radius | double | Radius in km (default: 5) |

**Response 200:** (same as Get All Venues)

---

## ⚽ Fields API

**Base Path:** `/fields`

### 13. Get Field By ID

**Endpoint:** `GET /fields/{id}`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "id": "d4e5f6a7-b8c9-0123-defa-234567890123",
    "fieldName": "Sân 1",
    "fieldType": "5vs5",
    "pricePerHour": 150000,
    "isActive": true,
    "venue": {
      "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "venueName": "Sân Bóng Đẹp",
      "address": "123 Đường ABC"
    }
  }
}
```

---

### 14. Get Field Slots (Available times)

**Endpoint:** `GET /fields/{id}/slots?date=2026-06-20`

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| date | DateTime | Date to check (format: yyyy-MM-dd) |

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "startTime": "2026-06-20T07:00:00",
      "endTime": "2026-06-20T08:00:00",
      "price": 150000,
      "status": "Available",
      "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123"
    },
    {
      "id": "f6a7b8c9-d0e1-2345-fabc-456789012345",
      "startTime": "2026-06-20T08:00:00",
      "endTime": "2026-06-20T09:00:00",
      "price": 150000,
      "status": "Booked",
      "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123"
    }
  ]
}
```

---

### 15. Get Field Week Schedule

**Endpoint:** `GET /fields/{id}/week-schedule`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123",
    "schedules": [
      {
        "dayOfWeek": "Monday",
        "isOpen": true,
        "openTime": "07:00",
        "closeTime": "22:00"
      },
      {
        "dayOfWeek": "Tuesday",
        "isOpen": true,
        "openTime": "07:00",
        "closeTime": "22:00"
      }
    ]
  }
}
```

---

## 🕐 Slots API

**Base Path:** `/slots`

### 16. Lock Slot (Checkout Flow)

**Endpoint:** `POST /slots/lock`  
**Auth:** Bearer Token

**Request:**
```json
{
  "slotId": "e5f6a7b8-c9d0-1234-efab-345678901234",
  "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123",
  "startTime": "2026-06-20T07:00:00",
  "endTime": "2026-06-20T08:00:00",
  "selectedDate": "2026-06-20"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Slot locked successfully.",
  "data": {
    "slotId": "e5f6a7b8-c9d0-1234-efab-345678901234",
    "lockedUntil": "2026-06-16T20:15:00Z",
    "isNewSlot": false
  }
}
```

**Error 400:**
```json
{
  "success": false,
  "message": "Slot is already booked"
}
```

---

### 17. Unlock Slot

**Endpoint:** `POST /slots/{id}/unlock`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "Slot unlocked successfully.",
  "data": {}
}
```

---

### 18. Create Slot (Owner/Admin)

**Endpoint:** `POST /slots`  
**Auth:** Bearer Token (Owner, Admin)

**Request:**
```json
{
  "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123",
  "startTime": "2026-06-20T07:00:00",
  "endTime": "2026-06-20T08:00:00",
  "price": 150000
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "Slot created successfully",
  "data": {
    "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
    "startTime": "2026-06-20T07:00:00",
    "endTime": "2026-06-20T08:00:00",
    "price": 150000
  }
}
```

---

### 19. Update Slot (Owner/Admin)

**Endpoint:** `PUT /slots/{id}`  
**Auth:** Bearer Token (Owner, Admin)

**Request:**
```json
{
  "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123",
  "startTime": "2026-06-20T07:00:00",
  "endTime": "2026-06-20T08:00:00",
  "price": 180000
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Slot updated successfully",
  "data": { ... }
}
```

---

### 20. Delete Slot (Owner/Admin)

**Endpoint:** `DELETE /slots/{id}`  
**Auth:** Bearer Token (Owner, Admin)

**Response 200:**
```json
{
  "success": true,
  "message": "Time slot deleted successfully"
}
```

---

## 📅 Bookings API

**Base Path:** `/bookings`

### 21. Create Booking

**Endpoint:** `POST /bookings`  
**Auth:** Bearer Token

**Request:**
```json
{
  "slotIds": [
    "e5f6a7b8-c9d0-1234-efab-345678901234",
    "f6a7b8c9-d0e1-2345-fabc-456789012345"
  ],
  "notes": "Cần thuê đồ bóng đá"
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "Booking created successfully",
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "bookingStatus": "Pending",
    "totalAmount": 300000,
    "bookingItems": [
      {
        "slotId": "e5f6a7b8-c9d0-1234-efab-345678901234",
        "price": 150000,
        "slot": {
          "startTime": "2026-06-20T07:00:00",
          "endTime": "2026-06-20T08:00:00",
          "field": {
            "fieldName": "Sân 1",
            "venue": {
              "venueName": "Sân Bóng Đẹp"
            }
          }
        }
      }
    ],
    "createdAt": "2026-06-16T14:00:00Z"
  }
}
```

---

### 22. Get Booking By ID

**Endpoint:** `GET /bookings/{id}`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "bookingStatus": "Pending",
    "totalAmount": 300000,
    "user": {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "fullName": "Nguyen Van A"
    },
    "bookingItems": [...],
    "payments": [...],
    "createdAt": "2026-06-16T14:00:00Z"
  }
}
```

---

### 23. Get Booking History

**Endpoint:** `GET /bookings/history?status=Pending&page=1&pageSize=20`  
**Auth:** Bearer Token

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| status | string | Filter: Pending, Accepted, Deposited, Completed, Cancelled |
| from | DateTime | Start date |
| to | DateTime | End date |
| page | int | Page number |
| pageSize | int | Items per page |

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "items": [...],
    "totalCount": 10,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
}
```

---

### 24. Get Booking Review

**Endpoint:** `GET /bookings/{id}/review`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "reviewId": "7890abcd-ef12-3456-7890-abcd12345678",
    "rating": 5,
    "comment": "Sân rất đẹp và chất lượng",
    "createdAt": "2026-06-21T10:00:00Z"
  }
}
```

---

### 25. Cancel Booking

**Endpoint:** `PUT /bookings/{id}/cancel?cancellationReason=Không có thời gian`  
**Auth:** Bearer Token

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| cancellationReason | string | Lý do hủy (optional) |

**Response 200:**
```json
{
  "success": true,
  "message": "Booking cancelled successfully"
}
```

---

## 💳 Payments API

**Base Path:** `/payments`

### 26. Get Payments By Booking

**Endpoint:** `GET /payments/booking/{bookingId}`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
      "bookingId": "123e4567-e89b-12d3-a456-426614174000",
      "amount": 150000,
      "paymentStatus": "Success",
      "paymentType": "Deposit",
      "transactionCode": "ABC123XYZ",
      "gateway": "SePay",
      "createdAt": "2026-06-16T15:00:00Z"
    }
  ]
}
```

---

### 27. Create Deposit Payment

**Endpoint:** `POST /payments/deposit`  
**Auth:** Bearer Token

**Request:**
```json
{
  "bookingId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "Payment created successfully",
  "data": {
    "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
    "bookingId": "123e4567-e89b-12d3-a456-426614174000",
    "amount": 150000,
    "paymentStatus": "Pending",
    "paymentType": "Deposit",
    "transactionCode": "ABC123XYZ",
    "gateway": "SePay",
    "createdAt": "2026-06-16T15:00:00Z"
  }
}
```

---

### 28. Create Final Payment

**Endpoint:** `POST /payments/final`  
**Auth:** Bearer Token

**Request:**
```json
{
  "bookingId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Response 201:** (same as deposit)

---

### 29. Get SePay QR Info

**Endpoint:** `GET /payments/{paymentId}/sepay-qr`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "paymentId": "e5f6a7b8-c9d0-1234-efab-345678901234",
    "amount": 150000,
    "description": "CMABC123XYZ",
    "status": "Pending",
    "qrUrl": "https://qr.sepay.vn/img?acc=0000000001&bank=tpb&amount=150000&des=CMABC123XYZ",
    "bankInfo": {
      "bankId": "tpb",
      "accountNo": "0000000001",
      "accountName": "NGUYEN VAN A"
    }
  }
}
```

---

### 30. SePay Webhook

**Endpoint:** `POST /payments/webhook/sepay`  
**Auth:** `Authorization: Apikey sepay_webhook_local_2026`

**Request:**
```json
{
  "id": 12345,
  "gateway": "SePay",
  "transactionDate": "2026-06-16T19:03:00",
  "accountNumber": "0000000001",
  "subAccount": "",
  "code": "ABC123XYZ",
  "transferType": "in",
  "transferAmount": 150000,
  "accumulated": 150000,
  "accumulatedBalance": 150000,
  "content": "CT TNHH San Bong Da Thanh Pho",
  "referenceCode": "REF123456",
  "description": "Thanh toan dat coc san bong"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Payment processed successfully",
  "paymentId": "e5f6a7b8-c9d0-1234-efab-345678901234",
  "status": "Success"
}
```

---

## ⭐ Reviews API

**Base Path:** `/reviews`

### 31. Get Venue Reviews

**Endpoint:** `GET /reviews/venue/{venueId}?page=1&pageSize=10`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "averageRating": 4.5,
    "totalReviews": 120,
    "reviews": [
      {
        "id": "7890abcd-ef12-3456-7890-abcd12345678",
        "rating": 5,
        "comment": "Sân rất đẹp và chất lượng",
        "user": {
          "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
          "fullName": "Nguyen Van A",
          "avatarUrl": "https://example.com/avatar.jpg"
        },
        "createdAt": "2026-06-21T10:00:00Z"
      }
    ],
    "totalCount": 120,
    "page": 1,
    "pageSize": 10,
    "totalPages": 12
  }
}
```

---

### 32. Get Field Average Rating

**Endpoint:** `GET /reviews/field/{fieldId}/average-rating`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "fieldId": "d4e5f6a7-b8c9-0123-defa-234567890123",
    "averageRating": 4.5,
    "totalReviews": 120
  }
}
```

---

### 33. Create Review

**Endpoint:** `POST /reviews`  
**Auth:** Bearer Token

**Request:**
```json
{
  "venueId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
  "bookingId": "123e4567-e89b-12d3-a456-426614174000",
  "rating": 5,
  "comment": "Sân rất đẹp và chất lượng"
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "Review created successfully",
  "data": {
    "reviewId": "7890abcd-ef12-3456-7890-abcd12345678",
    "rating": 5,
    "comment": "Sân rất đẹp và chất lượng",
    "createdAt": "2026-06-21T10:00:00Z"
  }
}
```

---

### 34. Get My Reviews

**Endpoint:** `GET /reviews/my-reviews`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [...]
}
```

---

### 35. Update Review

**Endpoint:** `PUT /reviews/{id}`  
**Auth:** Bearer Token

**Request:**
```json
{
  "rating": 4,
  "comment": "Cập nhật đánh giá"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Review updated successfully",
  "data": { ... }
}
```

---

### 36. Delete Review

**Endpoint:** `DELETE /reviews/{id}`  
**Auth:** Bearer Token (Admin, Owner)

**Response 200:**
```json
{
  "success": true,
  "message": "Review deleted successfully"
}
```

---

## 🔔 Notifications API

**Base Path:** `/notifications`

### 37. Get Notifications

**Endpoint:** `GET /notifications?unreadOnly=false&pageNumber=1&pageSize=10`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "items": [
      {
        "notificationId": "12345678-90ab-cdef-1234-567890abcdef",
        "title": "Thanh toán thành công",
        "message": "Bạn đã nhận được 150,000 VND từ đặt sân",
        "type": "Payment",
        "refId": "123e4567-e89b-12d3-a456-426614174000",
        "isRead": false,
        "createdAt": "2026-06-16T15:30:00Z"
      }
    ],
    "totalCount": 25,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 3
  }
}
```

---

### 38. Get Unread Count

**Endpoint:** `GET /notifications/unread-count`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "unreadCount": 5
  }
}
```

---

### 39. Mark Notification As Read

**Endpoint:** `PUT /notifications/{id}/read`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK"
}
```

---

### 40. Mark All As Read

**Endpoint:** `PUT /notifications/read-all`  
**Auth:** Bearer Token

**Response 200:**
```json
{
  "success": true,
  "message": "OK"
}
```

---

## 💰 Owner Wallet API

**Base Path:** `/owner/wallet`  
**Auth:** Bearer Token (Owner)

### 41. Get Owner's Wallet Info

**Endpoint:** `GET /owner/wallet`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "balance": 900000,
    "pendingWithdrawalCount": 1,
    "pendingWithdrawalAmount": 500000
  }
}
```

---

### 42. Get Wallet Transaction History

**Endpoint:** `GET /owner/wallet-history?page=1&pageSize=20`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "transactions": [
      {
        "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "ownerId": "c3d4e5f6-a7b8-9012-cdef-123456789012",
        "type": "Deposit",
        "amount": 450000,
        "description": "Deposit payment from booking 123e4567 (Transaction: ABC123)",
        "relatedBookingId": "123e4567-e89b-12d3-a456-426614174000",
        "relatedWithdrawalId": null,
        "createdAt": "2026-06-16T10:30:00Z"
      }
    ],
    "totalCount": 15,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
}
```

---

## 💸 Owner Withdrawal API

**Base Path:** `/owner/withdrawal-requests`  
**Auth:** Bearer Token (Owner)

### 43. Create Withdrawal Request

**Endpoint:** `POST /owner/withdrawal-requests`

**Request:**
```json
{
  "amount": 500000,
  "bankName": "Vietcombank",
  "bankAccountNumber": "1234567890",
  "bankAccountHolderName": "NGUYEN VAN A"
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "id": "d4e5f6a7-b8c9-0123-defa-234567890123",
    "status": "Pending",
    "amount": 500000,
    "createdAt": "2026-06-16T12:00:00Z",
    "message": "Withdrawal request created successfully"
  }
}
```

---

### 44. Get Owner's Withdrawal Requests

**Endpoint:** `GET /owner/withdrawal-requests`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "id": "d4e5f6a7-b8c9-0123-defa-234567890123",
      "ownerId": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "ownerName": "Nguyen Van A",
      "amount": 500000,
      "bankName": "Vietcombank",
      "bankAccountNumber": "1234567890",
      "bankAccountHolderName": "NGUYEN VAN A",
      "status": "Pending",
      "approvedByAdminId": null,
      "approvedByAdminName": null,
      "rejectionReason": null,
      "createdAt": "2026-06-16T12:00:00Z",
      "approvedAt": null
    }
  ]
}
```

---

### 45. Get Specific Withdrawal Request

**Endpoint:** `GET /owner/withdrawal-requests/{id}`

**Response 200:** (same structure as above)

---

## 🔐 Admin Withdrawal API

**Base Path:** `/admin/withdrawal-requests`  
**Auth:** Bearer Token (Admin)

### 46. Get All Withdrawal Requests

**Endpoint:** `GET /admin/withdrawal-requests?status=Pending`

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| status | string | Filter: Pending, Approved, Rejected |

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [...]
}
```

---

### 47. Get Specific Withdrawal Request

**Endpoint:** `GET /admin/withdrawal-requests/{id}`

**Response 200:** (same structure as owner)

---

### 48. Approve Withdrawal Request

**Endpoint:** `PUT /admin/withdrawal-requests/{id}/approve`

**Response 200:**
```json
{
  "success": true,
  "message": "Withdrawal request approved successfully"
}
```

---

### 49. Reject Withdrawal Request

**Endpoint:** `PUT /admin/withdrawal-requests/{id}/reject`

**Request:**
```json
{
  "reason": "Số tài khoản không hợp lệ"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Withdrawal request rejected"
}
```

---

### 50. Get Wallet Statistics

**Endpoint:** `GET /admin/withdrawal-requests/stats`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "balance": 15000000,
    "pendingWithdrawal": 2500000,
    "totalEarnings": 50000000,
    "totalWithdrawn": 32500000,
    "transactionCount": 150
  }
}
```

---

## 🏢 Owner Venues API

**Base Path:** `/owner/venues`  
**Auth:** Bearer Token (Owner)

### 51. Get My Venues

**Endpoint:** `GET /owner/venues?isActive=true&page=1&pageSize=10`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "items": [...],
    "totalCount": 2,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

---

### 52. Create Venue

**Endpoint:** `POST /owner/venues`

**Request:**
```json
{
  "venueName": "Sân Bóng Mới",
  "address": "456 Đường XYZ, Quận 2, TP.HCM",
  "latitude": 10.780000,
  "longitude": 106.700000,
  "description": "Sân bóng mới xây",
  "openingHours": "06:00 - 22:00",
  "phoneContact": "0909876543"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Venue created successfully",
  "data": {
    "id": "e5f6a7b8-c9d0-1234-efab-345678901234",
    "venueName": "Sân Bóng Mới",
    ...
  }
}
```

---

### 53. Update Venue

**Endpoint:** `PUT /owner/venues/{id}`

**Request:** (same as create)

**Response 200:**
```json
{
  "success": true,
  "message": "Venue updated successfully",
  "data": { ... }
}
```

---

### 54. Update Venue Status

**Endpoint:** `PUT /owner/venues/{id}/status`

**Request:**
```json
{
  "isActive": false
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Venue status updated successfully",
  "data": {
    "isActive": false
  }
}
```

---

### 55. Upload Venue Images

**Endpoint:** `POST /owner/venues/{id}/images`  
**Content-Type:** `multipart/form-data`

**Form Data:**
- images: File[] (multiple images)

**Response 200:**
```json
{
  "success": true,
  "message": "Images uploaded successfully",
  "data": [
    "https://pub-10243289791141549f3caec0328fcab3.r2.dev/venues/abc123.jpg"
  ]
}
```

---

### 56. Delete Venue Image

**Endpoint:** `DELETE /owner/venues/{id}/images/{imageId}`

**Response 200:**
```json
{
  "success": true,
  "message": "Image deleted successfully",
  "data": {}
}
```

---

## ⚽ Owner Fields API

**Base Path:** `/owner/fields`  
**Auth:** Bearer Token (Owner)

### 57. Update Field

**Endpoint:** `PUT /owner/fields/{id}`

**Request:**
```json
{
  "fieldName": "Sân 1 (Mới)",
  "fieldType": "7vs7",
  "pricePerHour": 200000
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Field updated successfully",
  "data": { ... }
}
```

---

### 58. Update Field Status

**Endpoint:** `PUT /owner/fields/{id}/status`

**Request:**
```json
{
  "isActive": false
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Field status updated successfully",
  "data": {
    "isActive": false
  }
}
```

---

## 📋 Owner Bookings API

**Base Path:** `/owner`  
**Auth:** Bearer Token (Owner, Admin)

### 59. Get Owner Stats

**Endpoint:** `GET /owner/stats`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "totalBookings": 150,
    "pendingBookings": 5,
    "todayBookings": 3,
    "totalRevenue": 50000000,
    "monthlyRevenue": 15000000
  }
}
```

---

### 60. Get Revenue

**Endpoint:** `GET /owner/revenue?from=2026-06-01&to=2026-06-30&groupBy=day`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "totalRevenue": 5000000,
    "averagePerDay": 250000,
    "breakdown": [
      { "date": "2026-06-01", "revenue": 500000 },
      { "date": "2026-06-02", "revenue": 300000 }
    ]
  }
}
```

---

### 61. Get Pending Bookings

**Endpoint:** `GET /owner/bookings/pending`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [...]
}
```

---

### 62. Get All Owner Bookings

**Endpoint:** `GET /owner/bookings`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [...]
}
```

---

### 63. Accept Booking

**Endpoint:** `PUT /owner/bookings/{id}/accept`

**Response 200:**
```json
{
  "success": true,
  "message": "Booking accepted successfully"
}
```

---

### 64. Reject Booking

**Endpoint:** `PUT /owner/bookings/{id}/reject?rejectionReason=Không có sân trống`

**Query Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| rejectionReason | string | Lý do từ chối |

**Response 200:**
```json
{
  "success": true,
  "message": "Booking rejected successfully"
}
```

---

### 65. Complete Booking

**Endpoint:** `PUT /owner/bookings/{id}/complete`

**Response 200:**
```json
{
  "success": true,
  "message": "Booking completed successfully"
}
```

---

### 66. Get Fields By Venue

**Endpoint:** `GET /owner/venues/{venueId}/fields`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [...]
}
```

---

### 67. Create Field

**Endpoint:** `POST /owner/venues/{venueId}/fields`

**Request:**
```json
{
  "fieldName": "Sân 3",
  "fieldType": "5vs5",
  "pricePerHour": 150000,
  "isActive": true
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "id": "f6a7b8c9-d0e1-2345-fabc-456789012345",
    "fieldName": "Sân 3",
    "fieldType": "5vs5",
    "pricePerHour": 150000,
    "isActive": true
  }
}
```

---

### 68. Get Field Schedule

**Endpoint:** `GET /owner/fields/{id}/schedule`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "dayOfWeek": "Monday",
      "isOpen": true,
      "openTime": "07:00",
      "closeTime": "22:00",
      "pricePerHour": 150000
    }
  ]
}
```

---

### 69. Upsert Field Schedule

**Endpoint:** `PUT /owner/fields/{id}/schedule`

**Request:**
```json
{
  "schedules": [
    {
      "dayOfWeek": "Monday",
      "isOpen": true,
      "openTime": "07:00",
      "closeTime": "22:00",
      "pricePerHour": 150000
    }
  ]
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [...]
}
```

---

### 70. Bulk Create Slots

**Endpoint:** `POST /owner/fields/{id}/slots/bulk`

**Request:**
```json
{
  "date": "2026-06-20",
  "startTime": "07:00",
  "endTime": "22:00",
  "slotDurationMinutes": 60,
  "pricePerHour": 150000
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "createdCount": 15
  }
}
```

---

### 71. Add Venue Amenities

**Endpoint:** `POST /owner/venues/{id}/amenities`

**Request:**
```json
{
  "amenityIds": [
    "11111111-1111-1111-1111-111111111111",
    "22222222-2222-2222-2222-222222222222"
  ]
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "addedCount": 2
  }
}
```

---

### 72. Delete Venue Amenity

**Endpoint:** `DELETE /owner/venues/{id}/amenities/{amenityId}`

**Response 200:**
```json
{
  "success": true,
  "message": "OK"
}
```

---

### 73. Get Owner Discounts

**Endpoint:** `GET /owner/discounts`

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "discountId": "aaaabbbb-cccc-dddd-eeee-ffff12345678",
      "discountCode": "SUMMER20",
      "discountType": "Percentage",
      "discountValue": 20,
      "minimumBookingAmount": 500000,
      "maxDiscountAmount": 100000,
      "startDate": "2026-06-01",
      "endDate": "2026-08-31",
      "isActive": true
    }
  ]
}
```

---

### 74. Create Discount

**Endpoint:** `POST /owner/discounts`

**Request:**
```json
{
  "discountCode": "SUMMER20",
  "discountType": "Percentage",
  "discountValue": 20,
  "minimumBookingAmount": 500000,
  "maxDiscountAmount": 100000,
  "startDate": "2026-06-01",
  "endDate": "2026-08-31",
  "isActive": true
}
```

**Response 201:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "discountId": "aaaabbbb-cccc-dddd-eeee-ffff12345678",
    "discountCode": "SUMMER20",
    ...
  }
}
```

---

### 75. Update Discount

**Endpoint:** `PUT /owner/discounts/{id}`

**Request:** (same as create)

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": { ... }
}
```

---

### 76. Update Discount Status

**Endpoint:** `PUT /owner/discounts/{id}/status`

**Request:**
```json
{
  "isActive": false
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "isActive": false
  }
}
```

---

### 77. Delete Discount

**Endpoint:** `DELETE /owner/discounts/{id}`

**Response 200:**
```json
{
  "success": true,
  "message": "OK"
}
```

---

## 🔑 Booking Status Flow

```
Pending → Accepted → Deposited → Completed
   ↓          ↓
Cancelled  Cancelled
```

| Status | Description |
|--------|-------------|
| Pending | Chờ owner chấp nhận |
| Accepted | Owner đã chấp nhận, chờ thanh toán cọc |
| Deposited | Đã thanh toán cọc |
| Completed | Đã hoàn tất |
| Cancelled | Đã hủy |

---

## 💰 Payment Status Flow

```
Pending → Success / Failed
```

| Status | Description |
|--------|-------------|
| Pending | Chờ thanh toán |
| Success | Thanh toán thành công |
| Failed | Thanh toán thất bại |

---

## 💵 Wallet Transaction Types

| Type | Description |
|------|-------------|
| Deposit | Tiền vào ví từ thanh toán booking |
| Withdrawal | Tiền ra khỏi ví khi rút |
| Refund | Hoàn tiền khi hủy booking |

---

## ⚠️ Error Codes

| HTTP Code | Success | Message | Description |
|-----------|---------|---------|-------------|
| 200 | true | OK | Thành công |
| 200 | false | Error message | Lỗi nghiệp vụ |
| 400 | false | Bad Request | Dữ liệu không hợp lệ |
| 401 | - | Unauthorized | Chưa đăng nhập |
| 403 | - | Forbidden | Không có quyền |
| 404 | false | Not Found | Không tìm thấy |
| 422 | false | Validation Error | Lỗi validation |

---

## 📝 Notes

1. **Port chính xác:** `http://localhost:5234`
2. **Webhook API Key:** `sepay_webhook_local_2026`
3. **JWT Token:** Gửi trong header `Authorization: Bearer <token>`
4. **Role-based Access:**
   - `User` - Customer thông thường
   - `Owner` - Chủ sân
   - `Admin` - Quản trị viên
5. **10% Commission:** Admin giữ 10% mỗi payment làm phí dịch vụ
6. **Wallet Balance:** Owner nhận 90% tổng payment vào ví
