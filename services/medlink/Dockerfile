FROM mcr.microsoft.com/dotnet/core/sdk:3.0

WORKDIR /app

COPY ./publish/ /app/
COPY ./start.sh /app/

ENV COMPlus_JitTelemetry=0 COMPlus_legacyCorruptedStateExceptionsPolicy=1 COMPlus_legacyUnhandledExceptionPolicy=1 LC_ALL=en_US.UTF-8 LANG=en_US.UTF-8
RUN adduser --system --home /app --shell /usr/sbin/nologin --no-create-home --disabled-password -u 10000 medlink

ENTRYPOINT ["./start.sh"]

