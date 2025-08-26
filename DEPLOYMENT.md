# SignalR Blazor App - Deployment Guide

## Konfiguracja dla Azure Static Web Apps

### 1. Pliki konfiguracyjne

Aplikacja Blazor używa dynamicznej konfiguracji poprzez pliki JSON:

- `appsettings.json` - Konfiguracja domyślna
- `appsettings.Development.json` - Konfiguracja dla środowiska rozwojowego (`ASPNETCORE_ENVIRONMENT=Development`)
- `appsettings.Production.json` - Konfiguracja dla środowiska produkcyjnego (`ASPNETCORE_ENVIRONMENT=Production`)
- `appsettings.Local.json` - Konfiguracja dla środowiska lokalnego (`ASPNETCORE_ENVIRONMENT=Local`)

### 2. Zmienne środowiskowe

Aplikacja czyta konfigurację na podstawie zmiennej środowiskowej:
- `ASPNETCORE_ENVIRONMENT` - główna zmienna (Development/Production/Local)
- `AZURE_FUNCTIONS_ENVIRONMENT` - fallback dla Azure Static Web Apps

### 3. Aktualizacja konfiguracji dla Azure

W pliku `appsettings.Production.json` zaktualizuj:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-api-production.azurewebsites.net/",
    "SignalRHubUrl": "https://your-api-production.azurewebsites.net/messagehub"
  },
  "Environment": "Production"
}
```

Zamień `your-api-production.azurewebsites.net` na rzeczywisty URL Twojego API.

### 3. Deploy do Azure Static Web Apps

1. **Połącz z GitHub**:
   - Utwórz nowy Azure Static Web App
   - Połącz z Twoim repozytorium GitHub

2. **Konfiguracja Build**:
   - App location: `/src/BlazorApp1/BlazorApp1`
   - Output location: `wwwroot`
   - Api location: (pozostaw puste dla samej aplikacji Blazor)

3. **Automatyczny deploy**:
   - Plik `.github/workflows/azure-static-web-apps.yml` zostanie automatycznie utworzony
   - Każdy commit na branch `main` uruchomi automatyczny deploy

### 4. Konfiguracja API

API należy zdeployować oddzielnie jako Azure App Service i zaktualizować CORS:

```csharp
policy.WithOrigins(
    "http://localhost:5208", 
    "http://localhost:5209", 
    "https://your-blazor-app.azurestaticapps.net" // Dodaj URL Static Web App
)
```

### 5. Środowiska

- **Development**: `http://localhost:5236` (API) + `http://localhost:5208` (Blazor)
- **Production**: Azure App Service (API) + Azure Static Web Apps (Blazor)

### 6. Funkcje

- ✅ Dynamiczna konfiguracja URL-i API
- ✅ Automatyczny CI/CD przez GitHub Actions
- ✅ SignalR w czasie rzeczywistym
- ✅ Obsługa różnych środowisk
- ✅ CORS skonfigurowany dla Azure

### 7. Lokalny rozwój

```bash
# Uruchom API
cd src/WebApplication1
dotnet run

# Uruchom Blazor (w nowym terminalu)
cd src/BlazorApp1/BlazorApp1
dotnet run
```

### 8. Testowanie konfiguracji

Po uruchomieniu możesz:
- Sprawdzić stronę Weather - powinna pobierać dane z API
- Sprawdzić stronę Messages - powinna łączyć się przez SignalR i wyświetlać wiadomości co 3 sekundy
