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
EXPOSE 5002
ENV ASPNETCORE_ENVIRONMENT="Docker"
ENTRYPOINT ["dotnet", "LaundryBooker.Sts/out/LaundryBooker.Sts.dll"]

# in *.sln current working directory run these!
# docker build -t lutando/laundry-sts:latest -f LaundryBooker.Sts/Dockerfile .
# docker push lutando/laundry-sts:latest