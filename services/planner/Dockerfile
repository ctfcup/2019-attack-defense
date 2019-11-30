FROM node:13-alpine

COPY ./src /app

WORKDIR /app/

RUN npm install --only=production

ENTRYPOINT ./start.sh