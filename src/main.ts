import express from 'express';
import { initializeRabbitMQ } from './amqp_conn_management.ts';

const app = express();
const port = 4040;

app.use(express.json());

try {
    initializeRabbitMQ();
} catch (err) {
    console.error('Failed to initialize RabbitMQ:', err);
}

app.get('/', (req, res) => {
    res.send('Hello world!');
});

app.listen(port, () => {
    console.log(`App is listening at "http://localhost:${port}"`);
});