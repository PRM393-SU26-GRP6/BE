UPDATE "TimeSlots" SET "IsDeleted" = false, "SlotStatus" = 'Available' WHERE "IsDeleted" = true AND "SlotStatus" = 'Locked';
UPDATE "TimeSlots" SET "IsDeleted" = false WHERE "IsDeleted" = true;
