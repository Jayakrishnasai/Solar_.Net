# Supabase Database Setup Guide

## 1. Create Supabase Project

1. Go to [https://supabase.com](https://supabase.com)
2. Sign up / Login
3. Click **"New Project"**
4. Fill in:
   - **Name**: `solarsystem-db`
   - **Database Password**: (Save this! You'll need it)
   - **Region**: Choose closest to your K8s cluster
5. Click **"Create new project"**
6. Wait for project to initialize (~2 minutes)

---

## 2. Get Connection String

1. Go to **Settings** → **Database**
2. Scroll to **Connection string**
3. Select **URI** tab
4. Copy the connection string:

```
postgresql://postgres:[YOUR-PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
```

### For .NET / EF Core format:
```
Host=db.[PROJECT-REF].supabase.co;Port=5432;Database=postgres;Username=postgres;Password=[YOUR-PASSWORD];SSL Mode=Require;Trust Server Certificate=true
```

---

## 3. Configure Database Schema

### Option A: Let EF Core Create Tables (Recommended)

When your app starts, EF Core will auto-create tables. Just ensure:
1. Connection string is correct
2. App has `EnsureCreated()` or migrations applied

### Option B: Run SQL Manually

Go to **SQL Editor** in Supabase and run:

```sql
-- Celestial Bodies
CREATE TABLE IF NOT EXISTS "CelestialBodies" (
    "Id" SERIAL PRIMARY KEY,
    "ApiId" VARCHAR(100),
    "EnglishName" VARCHAR(200) NOT NULL,
    "BodyType" VARCHAR(50),
    "IsPlanet" BOOLEAN DEFAULT FALSE,
    "MeanRadius" DOUBLE PRECISION DEFAULT 0,
    "Mass" DOUBLE PRECISION DEFAULT 0,
    "Gravity" DOUBLE PRECISION DEFAULT 0,
    "Density" DOUBLE PRECISION DEFAULT 0,
    "EscapeVelocity" DOUBLE PRECISION DEFAULT 0,
    "RotationPeriod" DOUBLE PRECISION DEFAULT 0,
    "AxialTilt" DOUBLE PRECISION DEFAULT 0,
    "AverageTemperature" DOUBLE PRECISION DEFAULT 0,
    "SurfacePressure" DOUBLE PRECISION DEFAULT 0,
    "NumberOfMoons" INTEGER DEFAULT 0,
    "HasRingSystem" BOOLEAN DEFAULT FALSE,
    "HasGlobalMagneticField" BOOLEAN DEFAULT FALSE,
    "DiscoveryDate" VARCHAR(100),
    "DiscoveredBy" VARCHAR(200),
    "ImageUrl" VARCHAR(500),
    "Description" TEXT,
    "ParentBodyId" INTEGER REFERENCES "CelestialBodies"("Id"),
    "LastUpdated" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Unique index on ApiId
CREATE UNIQUE INDEX IF NOT EXISTS idx_celestial_apiid ON "CelestialBodies"("ApiId");

-- Orbits
CREATE TABLE IF NOT EXISTS "Orbits" (
    "Id" SERIAL PRIMARY KEY,
    "CelestialBodyId" INTEGER NOT NULL REFERENCES "CelestialBodies"("Id") ON DELETE CASCADE,
    "SemimajorAxis" DOUBLE PRECISION DEFAULT 0,
    "Perihelion" DOUBLE PRECISION DEFAULT 0,
    "Aphelion" DOUBLE PRECISION DEFAULT 0,
    "Eccentricity" DOUBLE PRECISION DEFAULT 0,
    "Inclination" DOUBLE PRECISION DEFAULT 0,
    "OrbitalPeriod" DOUBLE PRECISION DEFAULT 0,
    "ArgumentOfPerihelion" DOUBLE PRECISION DEFAULT 0,
    "LongitudeOfAscendingNode" DOUBLE PRECISION DEFAULT 0,
    "MeanAnomaly" DOUBLE PRECISION DEFAULT 0,
    "MeanMotion" DOUBLE PRECISION DEFAULT 0
);

-- Contact Submissions
CREATE TABLE IF NOT EXISTS "ContactSubmissions" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(200) NOT NULL,
    "Subject" VARCHAR(500),
    "Message" TEXT NOT NULL,
    "Category" VARCHAR(50),
    "SubmittedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsRead" BOOLEAN DEFAULT FALSE,
    "IsResolved" BOOLEAN DEFAULT FALSE,
    "AdminNotes" TEXT
);

-- API Snapshots
CREATE TABLE IF NOT EXISTS "ApiSnapshots" (
    "Id" SERIAL PRIMARY KEY,
    "Source" VARCHAR(100),
    "Endpoint" VARCHAR(500),
    "RawData" TEXT,
    "FetchedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsValid" BOOLEAN DEFAULT TRUE
);

-- Planet Layers
CREATE TABLE IF NOT EXISTS "PlanetLayers" (
    "Id" SERIAL PRIMARY KEY,
    "CelestialBodyId" INTEGER NOT NULL REFERENCES "CelestialBodies"("Id") ON DELETE CASCADE,
    "LayerName" VARCHAR(100),
    "Composition" TEXT,
    "DepthKm" DOUBLE PRECISION DEFAULT 0,
    "TemperatureK" DOUBLE PRECISION DEFAULT 0,
    "LayerOrder" INTEGER DEFAULT 0
);

-- Atmospheres
CREATE TABLE IF NOT EXISTS "Atmospheres" (
    "Id" SERIAL PRIMARY KEY,
    "CelestialBodyId" INTEGER NOT NULL REFERENCES "CelestialBodies"("Id") ON DELETE CASCADE,
    "SurfacePressure" DOUBLE PRECISION DEFAULT 0,
    "ScaleHeight" DOUBLE PRECISION DEFAULT 0,
    "Composition" TEXT,
    "Notes" TEXT
);

-- Moons
CREATE TABLE IF NOT EXISTS "Moons" (
    "Id" SERIAL PRIMARY KEY,
    "ApiId" VARCHAR(100),
    "Name" VARCHAR(200),
    "EnglishName" VARCHAR(200),
    "ParentBodyId" INTEGER NOT NULL REFERENCES "CelestialBodies"("Id") ON DELETE CASCADE,
    "MeanRadius" DOUBLE PRECISION DEFAULT 0,
    "SemimajorAxis" DOUBLE PRECISION DEFAULT 0,
    "OrbitalPeriod" DOUBLE PRECISION DEFAULT 0
);

-- Simulation States
CREATE TABLE IF NOT EXISTS "SimulationStates" (
    "Id" SERIAL PRIMARY KEY,
    "SessionId" VARCHAR(100),
    "IsPaused" BOOLEAN DEFAULT FALSE,
    "TimeMultiplier" DOUBLE PRECISION DEFAULT 1,
    "CurrentDateTime" TIMESTAMP,
    "CameraPositionX" DOUBLE PRECISION DEFAULT 0,
    "CameraPositionY" DOUBLE PRECISION DEFAULT 0,
    "CameraPositionZ" DOUBLE PRECISION DEFAULT 0,
    "SelectedBodyId" INTEGER,
    "LastUpdated" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

## 4. Seed Initial Data

```sql
-- Insert Sun
INSERT INTO "CelestialBodies" ("ApiId", "EnglishName", "BodyType", "IsPlanet", "MeanRadius", "Mass", "Gravity", "Density")
VALUES ('soleil', 'Sun', 'Star', FALSE, 696340, 1.989e30, 274, 1.41);

-- Insert Planets
INSERT INTO "CelestialBodies" ("ApiId", "EnglishName", "BodyType", "IsPlanet", "MeanRadius", "Mass", "Gravity", "NumberOfMoons") VALUES
('mercure', 'Mercury', 'Planet', TRUE, 2439.7, 3.3011e23, 3.7, 0),
('venus', 'Venus', 'Planet', TRUE, 6051.8, 4.8675e24, 8.87, 0),
('terre', 'Earth', 'Planet', TRUE, 6371, 5.97237e24, 9.807, 1),
('mars', 'Mars', 'Planet', TRUE, 3389.5, 6.4171e23, 3.721, 2),
('jupiter', 'Jupiter', 'Planet', TRUE, 69911, 1.8982e27, 24.79, 95),
('saturne', 'Saturn', 'Planet', TRUE, 58232, 5.6834e26, 10.44, 146),
('uranus', 'Uranus', 'Planet', TRUE, 25362, 8.6810e25, 8.87, 28),
('neptune', 'Neptune', 'Planet', TRUE, 24622, 1.02413e26, 11.15, 16);

-- Insert Orbits
INSERT INTO "Orbits" ("CelestialBodyId", "SemimajorAxis", "OrbitalPeriod", "Eccentricity") VALUES
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Mercury'), 57909050, 87.97, 0.2056),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Venus'), 108208000, 224.7, 0.0067),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Earth'), 149598023, 365.25, 0.0167),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Mars'), 227939200, 686.98, 0.0934),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Jupiter'), 778570000, 4332.59, 0.0489),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Saturn'), 1433530000, 10759.22, 0.0565),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Uranus'), 2872460000, 30688.5, 0.0457),
((SELECT "Id" FROM "CelestialBodies" WHERE "EnglishName"='Neptune'), 4495060000, 60182, 0.0113);
```

---

## 5. Configure Row Level Security (Optional)

```sql
-- Enable RLS on contact submissions
ALTER TABLE "ContactSubmissions" ENABLE ROW LEVEL SECURITY;

-- Allow all inserts (public contact form)
CREATE POLICY "Allow public inserts" ON "ContactSubmissions"
    FOR INSERT WITH CHECK (true);

-- Only allow admin reads
CREATE POLICY "Admin only reads" ON "ContactSubmissions"
    FOR SELECT USING (auth.role() = 'authenticated');
```

---

## 6. Get API Keys

For direct Supabase client access (optional):

1. Go to **Settings** → **API**
2. Copy:
   - **Project URL**: `https://[PROJECT-REF].supabase.co`
   - **anon public key**: For frontend (safe to expose)
   - **service_role key**: For backend (keep secret!)

---

## 7. Connection String for Kubernetes

Add this to your GitHub Secrets as `SUPABASE_CONNECTION_STRING`:

```
Host=db.[PROJECT-REF].supabase.co;Port=5432;Database=postgres;Username=postgres;Password=[YOUR-PASSWORD];SSL Mode=Require;Trust Server Certificate=true
```

Replace:
- `[PROJECT-REF]` - Your Supabase project reference
- `[YOUR-PASSWORD]` - Database password you set

---

## 8. Test Connection

```bash
# Using psql
psql "postgresql://postgres:[PASSWORD]@db.[REF].supabase.co:5432/postgres"

# Test query
SELECT * FROM "CelestialBodies";
```

---

## Troubleshooting

### Connection refused
- Check firewall: Supabase allows all IPs by default
- Verify password is correct
- Ensure SSL Mode is set to Require

### EF Core migration fails
- Run `dotnet ef database update` with connection string
- Check table names match (PostgreSQL is case-sensitive, use quotes)

### SSL certificate error
- Add `Trust Server Certificate=true` to connection string
