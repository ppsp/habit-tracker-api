FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["HabitTrackerFirebase/HabitTrackerWebApi.csproj", "HabitTrackerFirebase/"]
COPY ["HabitTrackerCore/HabitTrackerCore.csproj", "HabitTrackerCore/"]
COPY ["HabitTrackerServices/HabitTrackerServices.csproj", "HabitTrackerServices/"]
COPY ["HabitTrackerTools/HabitTrackerTools.csproj", "HabitTrackerTools/"]

RUN dotnet restore "HabitTrackerFirebase/HabitTrackerWebApi.csproj"
COPY . .
WORKDIR /src/HabitTrackerFirebase
RUN dotnet build "HabitTrackerWebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HabitTrackerWebApi.csproj" -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=p
ublish /app .
#ENTRYPOINT ["dotnet", "HabitTrackerWebApi.dll"]

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "HabitTrackerWebApi.dll"]
