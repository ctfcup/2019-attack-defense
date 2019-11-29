FROM node:13-alpine

COPY ./src /app

WORKDIR /app/

RUN npm install --only=production

COPY ./src/tmp_libs/nodeorm ./node_modules/nodeorm

COPY ./src/tmp_libs/random ./node_modules/random

ETRYPOINT ./start.sh