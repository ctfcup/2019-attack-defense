FROM ubuntu:18.04

RUN apt-get update && apt-get install -y \
  python3.7-minimal python3-numpy python3-protobuf python3-pip \
  && apt-get clean

COPY requirements.txt .
RUN pip3 install -r requirements.txt

COPY /server /server
WORKDIR /server

EXPOSE 16962