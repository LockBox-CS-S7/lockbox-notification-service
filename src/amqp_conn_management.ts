import amqp from 'amqplib/callback_api.js';

let channel: amqp.Channel | null = null;
export const QUEUE_NAME = 'file-queue';

export function initializeRabbitMQ() {
    amqp.connect('amqp://localhost', (error0, connection) => {
        if (error0) {
            throw error0;
        }
        console.log('Connected to RabbitMQ');
        connection.createChannel((error1, ch) => {
            if (error1) {
                throw error1;
            }
            console.log('Channel created');
            
            ch.assertQueue(QUEUE_NAME, {
                durable: true
            });
            ch.prefetch(1);
            channel = ch;
        });
    });
};

export const getChannel = () => channel;