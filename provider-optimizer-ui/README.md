# Provider Optimizer - Frontend (React)

This is a simple React frontend for the Provider Optimizer API.

## Features
- Mobile responsive simple UI to request assistance (grua, cerrajeria, bateria)
- Uses `REACT_APP_API_URL` (build-time) to call the API
- Dockerfile + nginx for production serving

## Local development
1. Copy `.env.example` to `.env` and set API URL if needed:
   ```
   REACT_APP_API_URL=http://localhost:5000
   ```
2. Install dependencies:
   ```
   npm ci
   npm start
   ```

## Build & Docker
```
docker build -t provider-optimizer-ui .
```

In Docker Compose set build ARG:
```
args:
  REACT_APP_API_URL: "http://api:5000"
```

## Notes
- The component will attempt to auto-fill coordinates using browser geolocation.
- If your API returns 201 Created without body, adjust the frontend to fetch the resource if needed.
