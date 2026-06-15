# TimeSlot Seed Update — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Update the TimeSlot seed so that each field gets 17 `TimeSlot` rows (06:00–23:00, 1-hour steps) on `baseDate` instead of only 4 rows (18:00–22:00), making the `GET /slots/available` endpoint return fully bookable slots across the entire operating window.

**Architecture:** One migration touches only the `TimeSlots` HasData block. All other entities remain untouched.

**Tech Stack:** .NET 8, EF Core 8, Npgsql.

---

## File Structure

- Modify: `CourtManager.Infrastructure/ApplicationDbContext.cs` — change TimeSlot seed from 4 slots (18-22h) to 17 slots (06-23h) per field

---

## Task 1: Update TimeSlot seed data

**Files:**
- Modify: `CourtManager.Infrastructure/ApplicationDbContext.cs` (around line 234-256)

- [ ] **Step 1: Read the current seed block**

Read lines 234-260 of `CourtManager.Infrastructure/ApplicationDbContext.cs`. Confirm the current seed creates slots from `hour = 18` to `hour < 22` (4 slots per field).

- [ ] **Step 2: Replace the TimeSlot seed block**

Find this block (lines 234-256):

```csharp
        var slots = new List<TimeSlot>();
        var slotIndex = 1;
        foreach (var field in fields)
        {
            for (var hour = 18; hour < 22; hour++)
            {
                var status = slotIndex % 9 == 0 ? SlotStatus.Booked : slotIndex % 7 == 0 ? SlotStatus.Locked : SlotStatus.Available;
                slots.Add(new TimeSlot
                {
                    SlotId = Id("slot", slotIndex),
                    FieldId = field.Id,
                    StartTime = baseDate.AddHours(hour),
                    EndTime = baseDate.AddHours(hour + 1),
                    Price = field.PricePerHour,
                    SlotStatus = status,
                    LockedUntil = status == SlotStatus.Locked ? now.AddMinutes(20) : null,
                    CreatedAt = now.AddDays(-14 + slotIndex % 5),
                    RowVersion = (uint)slotIndex
                });
                slotIndex++;
            }
        }
        modelBuilder.Entity<TimeSlot>().HasData(slots);
```

Replace the `for (var hour = 18; hour < 22; hour++)` line with `for (var hour = 6; hour < 23; hour++)`. Everything else stays identical.

The status distribution formula (`slotIndex % 9 == 0` = Booked, `slotIndex % 7 == 0` = Locked) remains the same — it naturally distributes statuses across the 17 slots per field. With 17 slots × 24 fields = 408 slots:
- Every 9th slot → Booked (approximately 45 Booked slots)
- Every 7th slot → Locked (approximately 58 Locked slots)
- Remaining → Available (approximately 305 Available slots)

- [ ] **Step 3: Build to verify**

Run: `dotnet build CourtManager.Infrastructure/CourtManager.Infrastructure.csproj`
Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`. (The pre-existing CS0472 warning from `TimeSlotRepository.cs` is fine to ignore.)

- [ ] **Step 4: Commit**

```bash
git add CourtManager.Infrastructure/ApplicationDbContext.cs
git commit -m "chore(seed): expand TimeSlot seed from 4 to 17 slots per field"
```

---

## Task 2: Regenerate migration for new TimeSlot seed

**Files:**
- Create: `CourtManager.Infrastructure/Migrations/<timestamp>_ExpandTimeSlotSeed.cs` (generated)
- Create: `CourtManager.Infrastructure/Migrations/<timestamp>_ExpandTimeSlotSeed.Designer.cs` (generated)
- Modify: `CourtManager.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs` (regenerated)

- [ ] **Step 1: Generate new migration**

Run from repo root `D:\GitHub\BE`:

```bash
cd D:\GitHub\BE
dotnet ef migrations add ExpandTimeSlotSeed --project CourtManager.Infrastructure --startup-project CourtManager.APIs --output-dir Migrations
```

Expected: tool reports `Done.` and creates `YYYYMMDDHHMMSS_ExpandTimeSlotSeed.cs`. The `Up()` method should contain `DeleteData` for all 96 old TimeSlot rows (4 slots × 24 fields) followed by `InsertData` for all 408 new TimeSlot rows (17 slots × 24 fields).

- [ ] **Step 2: Verify the migration content**

Open the new migration file and confirm:
- `DeleteData` removes all rows where `StartTime` falls between 18:00 and 22:00 on `baseDate`
- `InsertData` inserts all 408 rows where `StartTime` falls between 06:00 and 22:00 (hours 6 through 22 inclusive, i.e., `hour < 23`)
- Each row has a unique `SlotId`, correct `StartTime`/`EndTime`, and correct `SlotStatus`

- [ ] **Step 3: Build the migration**

Run: `dotnet build CourtManager.Infrastructure/CourtManager.Infrastructure.csproj`
Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`.

- [ ] **Step 4: Commit**

```bash
git add CourtManager.Infrastructure/Migrations/
git commit -m "feat(infrastructure): add ExpandTimeSlotSeed migration (96 → 408 rows)"
```

---

## Self-Review

- [x] `baseDate` is `2026-06-09` (Tuesday). Slots will be 06:00–22:00 on this date.
- [x] 17 slots × 24 fields = 408 `TimeSlot` rows in seed
- [x] Status distribution unchanged (only the `for` loop bound changed)
- [x] `GetSlotsForDateQuery` overlays real `TimeSlot` rows onto 17 virtual slots — now all 17 have a matching `SlotId`
- [x] No other entity's seed changed
