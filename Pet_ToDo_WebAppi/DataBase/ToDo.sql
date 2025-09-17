PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "HashPasswordId" INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS "HashPasswords" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HashPasswords" PRIMARY KEY,
    "HashPassword" TEXT NOT NULL,
    "HashSalt" TEXT NOT NULL,
    CONSTRAINT "FK_HashPasswords_Users_Id" FOREIGN KEY ("Id") REFERENCES "Users" ("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "Tasks" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tasks" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "IsCompleted" INTEGER NOT NULL,
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "FK_Tasks_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_Tasks_UserId" ON "Tasks" ("UserId");
CREATE UNIQUE INDEX "IX_Users_Name" ON "Users" ("Name");
COMMIT;
