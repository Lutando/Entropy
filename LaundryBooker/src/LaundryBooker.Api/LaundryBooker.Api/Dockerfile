FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

# copy everything else and build app
COPY . ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp ./
EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT="Docker"
ENTRYPOINT ["dotnet", "LaundryBooker.Api/out/LaundryBooker.Api.dll"]

# in *.sln c
# docker build -t lutando/laundry-api:latest -f LaundryBooker.Api/Dockerfile .
# docker push lutando/laundry-api:latest