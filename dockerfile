FROM denoland/deno:latest

WORKDIR /app

COPY . .

RUN deno install
RUN chmod +x ./launch.sh
EXPOSE 4040

CMD ["./launch.sh"]
