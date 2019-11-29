FROM python:3.7-alpine

RUN apk add --no-cache g++ wget

COPY requirements.txt .
RUN pip install -r requirements.txt

COPY /server /server
WORKDIR /server

EXPOSE 16962