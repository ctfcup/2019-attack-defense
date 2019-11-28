FROM python:3.7-slim

COPY requirements.txt .
RUN pip install -r requirements.txt

COPY /server /server
WORKDIR /server

EXPOSE 16962