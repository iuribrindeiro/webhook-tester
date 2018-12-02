FROM microsoft/dotnet:2.1-sdk AS builder

WORKDIR /src

COPY WebhookTester.Presentation.csproj WebhookTester.Presentation/

WORKDIR /src/WebhookTester.Presentation

RUN dotnet restore WebhookTester.Presentation.csproj

COPY . .

RUN dotnet build WebhookTester.Presentation.csproj --no-restore -c Release -o /app

RUN dotnet publish WebhookTester.Presentation.csproj --no-restore -c Release -o /app/

FROM microsoft/aspnetcore as runtime

WORKDIR /app

COPY --from=builder /app .

CMD ["dotnet", "WebhookTester.Presentation.dll"]