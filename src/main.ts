import express from 'express';
import { initializeRabbitMQ, getChannel, QUEUE_NAME } from './amqp_conn_management.ts';

const app = express();
const port = 4040;
const receivedMessages: string[] = [];

app.use(express.json());

try {
    await initializeRabbitMQ();
} catch (err) {
    console.error('Failed to initialize RabbitMQ:', err);
}

// Start consuming messages from the RabbitMQ queue
const channel = getChannel();
if (channel) {
    channel.consume(QUEUE_NAME, (msg) => {
        console.log(' [x] Received message: ' + msg?.content.toString());
        receivedMessages.push(msg!.content.toString());
    }, {
        noAck: true,
    });
} else {
    console.error('Failed to get RabbitMQ channel');
}

// Start REST api server
app.get('/', (_req, res) => {
    res.send('Hello world!');
});

app.get('/messages', (_req, res) => {
    res.json(receivedMessages);
});

app.listen(port, () => {
    console.log(`App is listening at "http://localhost:${port}"`);
});
