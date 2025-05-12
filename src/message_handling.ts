import amqp from 'amqplib/callback_api.js';


interface FileServiceMessage {
    event_type: string;
    timestamp: string;
    source: string;
    user_id: string;
    data: Blob | null;
}


const receivedMessages: FileServiceMessage[] = [];

/**
 * Handles incomming messages from the RabbitMQ message broker.
 * @param msg The message that has just been received.
 */
export function handleIncomingMessage(msg: amqp.Message | null) {
    if (msg) {
        const message = parseAmqpMessage(msg);
        receivedMessages.push(message);
    } else {
        console.log('Received message is null');
    }
}

function parseAmqpMessage(msg: amqp.Message): FileServiceMessage {
    return JSON.parse(msg.content.toString()) as FileServiceMessage;
}

export const getReceivedMessages = () => receivedMessages;