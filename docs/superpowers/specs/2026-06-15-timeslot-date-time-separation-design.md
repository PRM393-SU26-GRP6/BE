# TimeSlot Date/Time Separation Design

**Date:** 2026-06-15  
**Status:** Approved  
**Author:** AI Assistant

---

## 1. Mục tiêu

Tách `Date` và `Time` trong `TimeSlot` entity:
- `StartTime`/`EndTime`: Chỉ lưu giờ (`TimeOnly`)
- `SelectedDate`: Lưu ngày riêng (`DateOnly`)

**Lý do:**
- Slot "09:00-10:00" có thể đặt được nhiều lần vào các ngày khác nhau
- `FieldSchedule` xác định khung giờ mở cửa, `TimeSlot` chỉ cần lưu ngày cụ thể khi có booking/lock
- Dễ query hơn: lọc theo ngày hoặc giờ riêng biệt

---

## 2. Các quyết định đã được duyệt

| Quyết định | Chọn |
|------------|------|
| Flow lock/book | Giữ SlotId |
| Tạo TimeSlot | Khi user chọn slot |
| Unlock behavior | Soft delete |
| Seed data | Xóa hoàn toàn |
| Design approach | Approach 1: Tách hẳn SelectedDate |

---

## 3. Entity Changes

### 3.1 TimeSlot Entity

**Trước:**
```csharp
public DateTime StartTime { get; set; }  // 2026-06-15 09:00:00
public DateTime EndTime { get; set; }    // 2026-06-15 10:00:00
```

**Sau:**
```csharp
public TimeOnly StartTime { get; set; }   // 09:00 (chỉ giờ)
public TimeOnly EndTime { get; set; }     // 10:00 (chỉ giờ)
public DateOnly SelectedDate { get; set; } // 2026-06-15 (ngày riêng)
```

### 3.2 FieldSchedule Entity

**Giữ nguyên** - đã đúng structure:
```csharp
public int DayOfWeek { get; set; }       // 0=Sun, 1=Mon, ..., 6=Sat
public TimeOnly OpenTime { get; set; }    // 06:00
public TimeOnly CloseTime { get; set; }   // 23:00
public int SlotDurationMinutes { get; set; } // 60
```

---

## 4. Database Migration

### 4.1 Thêm column SelectedDate
```sql
ALTER TABLE "TimeSlots" ADD COLUMN "SelectedDate" date NOT NULL;
```

### 4.2 Đổi StartTime/EndTime từ timestamp → time
```sql
ALTER TABLE "TimeSlots" ALTER COLUMN "StartTime" TYPE time USING "StartTime"::time;
ALTER TABLE "TimeSlots" ALTER COLUMN "EndTime" TYPE time USING "EndTime"::time;
```

### 4.3 Update data cũ
```sql
UPDATE "TimeSlots" SET "SelectedDate" = "StartTime"::date;
```

### 4.4 Thêm composite index
```sql
CREATE INDEX "IX_TimeSlots_FieldId_SelectedDate_StartTime" 
ON "TimeSlots" ("FieldId", "SelectedDate", "StartTime");
```

---

## 5. API Flow

### 5.1 GetAvailableSlots
```
GET /api/v1/fields/{id}/slots?date=2026-06-15

Flow:
1. Lấy FieldSchedule theo DayOfWeek (2026-06-15 là Monday → DayOfWeek = 1)
2. Generate virtual slots: 06:00-07:00, 07:00-08:00, ...
3. Query DB: tìm TimeSlot theo (FieldId, SelectedDate=2026-06-15)
4. Overlay status vào virtual slots
5. Trả về: [{ time: "06:00-07:00", status: "Available" }, ...]
```

### 5.2 LockSlot (tạo slot khi lock)
```
POST /api/v1/slots/lock
Body: { fieldId, selectedDate: "2026-06-15", startTime: "09:00", endTime: "10:00" }

Flow:
1. Check: đã có TimeSlot chưa?
   - Có → Lock nếu Available
   - Chưa → Tạo mới với Status=Locked
2. Trả về: { slotId, lockedUntil }
```

### 5.3 Booking
```
POST /api/v1/bookings
→ Tạo/verify TimeSlot → BookingItem
→ Update Status: Locked → Booked
```

---

## 6. Index Strategy

**TimeSlotConfiguration.cs:**
```csharp
builder.HasIndex(s => new { s.FieldId, s.SelectedDate, s.StartTime })
       .HasDatabaseName("IX_TimeSlots_FieldId_SelectedDate_StartTime");
```

---

## 7. Seed Data

- **Xóa** migration seed 408 slots hiện tại (20260615101643_ExpandTimeSlotSeed)
- **Không** seed slots mới - slots trống generate từ FieldSchedule

---

## 8. Affected Files

### Entity
- `CourtManager.Domain/Entities/TimeSlot.cs`

### Configuration
- `CourtManager.Infrastructure/Data/TimeSlotConfiguration.cs`

### DTOs
- `CourtManager.Application/DTOs/TimeSlotDto.cs`
- `CourtManager.Application/Features/FieldSchedules/FieldScheduleDtos.cs` (SlotForDateDto)

### Queries
- `CourtManager.Application/Features/TimeSlots/Queries/GetSlotsForDateQuery.cs`

### Commands
- `CourtManager.Application/Features/TimeSlots/Commands/LockSlotCommand.cs`
- `CourtManager.Application/Features/TimeSlots/Commands/LockSlotCommandHandler.cs`
- `CourtManager.Application/Features/TimeSlots/Commands/UnlockSlotCommand.cs`
- `CourtManager.Application/Features/TimeSlots/Commands/UnlockSlotCommandHandler.cs`
- `CourtManager.Application/Features/TimeSlots/Commands/UnlockExpiredSlotsCommandHandler.cs`

### Seed
- `CourtManager.Infrastructure/Migrations/20260615101643_ExpandTimeSlotSeed.cs` → Xóa

---

## 9. Backward Compatibility

- API response vẫn trả `DateTime StartTime`/`EndTime` cho backward compatibility với frontend
- Frontend tự parse `StartTime` thành `time` và `date` nếu cần
- Hoặc thêm fields mới: `StartTimeOfDay` (HH:mm) + `SelectedDate` (YYYY-MM-DD)

---

## 10. Rollback Plan

Nếu cần rollback:
1. Tạo migration đảo ngược: đổi `TimeOnly` → `DateTime`, xóa `SelectedDate`
2. Update data cũ: merge `SelectedDate` vào `StartTime`
