import { int, mysqlTable as table, varchar } from 'drizzle-orm/mysql-core';

export const notificationTable = table('notification_table', {
    id: int().primaryKey().autoincrement(),
    title: varchar({ length: 255 }).notNull(),
    description: varchar({ length: 255 }),
    userId: varchar('user_id', { length: 255 }).notNull(),
});
