import { notificationTable } from './db/schema.ts';
import { drizzle } from 'drizzle-orm/mysql2';
import amqp from 'amqplib/callback_api.js';



interface FileServiceMessage {
    event_type: string;
    timestamp: string;
    source: string;
    user_id: string;
    file?: {
        id: string;
        name: string;
        timestamp: string;
    }
}


const receivedMessages: FileServiceMessage[] = [];
const db = drizzle(Deno.env.get('DATABASE_URL')!);

/**
 * Handles incomming messages from the RabbitMQ message broker.
 * @param msg The message that has just been received.
 */
export async function handleIncomingMessage(msg: amqp.Message | null) {
    if (msg) {
        const message = parseAmqpMessage(msg);
        receivedMessages.push(message);
        
        // TODO: These details should be fetched from the incomming message.
        await db.insert(notificationTable).values({
            title: `File uploaded (${message.file?.name})`,
            description: `A file has been uploaded to the user account with id: ${message.user_id}`,
            userId: message.user_id,
        });
    } else {
        console.log('Received message is null');
    }
}

function parseAmqpMessage(msg: amqp.Message): FileServiceMessage {
    return JSON.parse(msg.content.toString()) as FileServiceMessage;
}

export const getReceivedMessages = () => receivedMessages;
