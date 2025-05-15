import express from 'express';
import { initializeRabbitMQ, getChannel, QUEUE_NAME } from './amqp_conn_management.ts';
import { handleIncomingMessage, getReceivedMessages } from "./message_handling.ts";

const app = express();
const port = 4040;

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
        handleIncomingMessage(msg);
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
    res.json(getReceivedMessages());
});

app.listen(port, () => {
    console.log(`App is listening at "http://localhost:${port}"`);
});
