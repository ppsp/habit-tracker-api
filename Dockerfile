FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
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

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HabitTrackerWebApi.dll"]