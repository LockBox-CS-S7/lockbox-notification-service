import amqp from 'amqplib/callback_api.js';

let channel: amqp.Channel | null = null;
export const QUEUE_NAME = 'file-queue';

export function initializeRabbitMQ(): Promise<void> {
    return new Promise((resolve, reject) => {
        amqp.connect('amqp://localhost', (error0, connection) => {
            if (error0) {
                return reject(error0);
            }
            console.log('Connected to RabbitMQ');
            connection.createChannel((error1, ch) => {
                if (error1) {
                    return reject(error1);
                }
                console.log('Channel created');
                ch.assertQueue(QUEUE_NAME, {
                    durable: true
                });
                ch.prefetch(1);
                channel = ch;
                resolve();
            });
        });
    });
}

export const getChannel = () => channel;