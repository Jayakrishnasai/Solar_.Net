-- =====================================================
-- COMPLETE SUPABASE SCHEMA FOR SOLAR SYSTEM EXPLORER
-- Run this in Supabase SQL Editor
-- =====================================================

-- =====================================================
-- 1. EXTENSIONS
-- =====================================================
CREATE EXTENSION IF NOT EXISTS postgis;

-- =====================================================
-- 2. CORE ASTRONOMY TABLES
-- =====================================================
CREATE TABLE IF NOT EXISTS celestial_bodies (
    id BIGSERIAL PRIMARY KEY,
    external_id TEXT UNIQUE,
    name TEXT NOT NULL,
    english_name TEXT,
    body_type TEXT NOT NULL,
    is_planet BOOLEAN DEFAULT FALSE,
    description TEXT,
    created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS planets (
    id BIGSERIAL PRIMARY KEY,
    celestial_body_id BIGINT NOT NULL REFERENCES celestial_bodies(id) ON DELETE CASCADE,
    mean_radius_km DOUBLE PRECISION,
    mass_value DOUBLE PRECISION,
    mass_exponent INTEGER,
    gravity DOUBLE PRECISION,
    escape_velocity DOUBLE PRECISION,
    avg_temperature DOUBLE PRECISION,
    axial_tilt DOUBLE PRECISION,
    has_rings BOOLEAN,
    has_atmosphere BOOLEAN
);

CREATE TABLE IF NOT EXISTS orbits (
    id BIGSERIAL PRIMARY KEY,
    celestial_body_id BIGINT NOT NULL REFERENCES celestial_bodies(id) ON DELETE CASCADE,
    orbiting_body_id BIGINT REFERENCES celestial_bodies(id),
    semi_major_axis_km DOUBLE PRECISION,
    eccentricity DOUBLE PRECISION,
    inclination DOUBLE PRECISION,
    orbital_period_days DOUBLE PRECISION,
    rotation_period_hours DOUBLE PRECISION
);

CREATE TABLE IF NOT EXISTS planet_layers (
    id BIGSERIAL PRIMARY KEY,
    planet_id BIGINT NOT NULL REFERENCES planets(id) ON DELETE CASCADE,
    layer_order INTEGER NOT NULL,
    layer_name TEXT NOT NULL,
    composition TEXT,
    thickness_km DOUBLE PRECISION,
    avg_temperature DOUBLE PRECISION
);

CREATE TABLE IF NOT EXISTS atmospheres (
    id BIGSERIAL PRIMARY KEY,
    planet_id BIGINT NOT NULL REFERENCES planets(id) ON DELETE CASCADE,
    primary_gases TEXT,
    pressure_bar DOUBLE PRECISION,
    density_kg_m3 DOUBLE PRECISION
);

CREATE TABLE IF NOT EXISTS moons (
    id BIGSERIAL PRIMARY KEY,
    celestial_body_id BIGINT NOT NULL REFERENCES celestial_bodies(id) ON DELETE CASCADE,
    planet_id BIGINT NOT NULL REFERENCES planets(id) ON DELETE CASCADE,
    mean_radius_km DOUBLE PRECISION,
    orbital_period_days DOUBLE PRECISION
);

CREATE TABLE IF NOT EXISTS api_snapshots (
    id BIGSERIAL PRIMARY KEY,
    source_name TEXT,
    endpoint TEXT,
    raw_json JSONB,
    fetched_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "SimulationStates" (
    "Id" SERIAL PRIMARY KEY,
    "SessionId" VARCHAR(100),
    "IsPaused" BOOLEAN DEFAULT FALSE,
    "TimeMultiplier" DOUBLE PRECISION DEFAULT 1,
    "SimulatedTime" TIMESTAMP,
    "LastUpdated" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- 3. CONTACT / FEEDBACK TABLES
-- =====================================================
CREATE TABLE IF NOT EXISTS "ContactSubmissions" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "Subject" VARCHAR(500),
    "Message" TEXT NOT NULL,
    "SubmittedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS "UserFeedbacks" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255),
    "Email" VARCHAR(255),
    "Rating" INTEGER,
    "Feedback" TEXT,
    "SubmittedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- 4. GEO / MAP TABLES
-- =====================================================
CREATE TABLE IF NOT EXISTS geo_countries (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    iso_code VARCHAR(3) UNIQUE,
    capital VARCHAR(255),
    population BIGINT,
    area_sq_km DOUBLE PRECISION,
    continent VARCHAR(100),
    geometry GEOMETRY(Polygon, 4326),
    center_lat DOUBLE PRECISION,
    center_lon DOUBLE PRECISION,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS geo_states (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    country_id INTEGER REFERENCES geo_countries(id),
    state_code VARCHAR(10),
    capital VARCHAR(255),
    population BIGINT,
    area_sq_km DOUBLE PRECISION,
    geometry GEOMETRY(Polygon, 4326),
    center_lat DOUBLE PRECISION,
    center_lon DOUBLE PRECISION,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS geo_cities (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    state_id INTEGER REFERENCES geo_states(id),
    country_id INTEGER REFERENCES geo_countries(id),
    population BIGINT,
    elevation_m INTEGER,
    timezone VARCHAR(100),
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    location GEOMETRY(Point, 4326),
    is_capital BOOLEAN DEFAULT FALSE,
    is_state_capital BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS geo_landmarks (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category VARCHAR(100),
    city_id INTEGER REFERENCES geo_cities(id),
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    location GEOMETRY(Point, 4326),
    image_url VARCHAR(500),
    rating DECIMAL(3,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- 5. INDEXES
-- =====================================================
CREATE INDEX IF NOT EXISTS idx_countries_geometry ON geo_countries USING GIST(geometry);
CREATE INDEX IF NOT EXISTS idx_states_geometry ON geo_states USING GIST(geometry);
CREATE INDEX IF NOT EXISTS idx_cities_location ON geo_cities USING GIST(location);
CREATE INDEX IF NOT EXISTS idx_landmarks_location ON geo_landmarks USING GIST(location);

-- =====================================================
-- 6. ENABLE RLS
-- =====================================================
ALTER TABLE celestial_bodies ENABLE ROW LEVEL SECURITY;
ALTER TABLE planets ENABLE ROW LEVEL SECURITY;
ALTER TABLE orbits ENABLE ROW LEVEL SECURITY;
ALTER TABLE planet_layers ENABLE ROW LEVEL SECURITY;
ALTER TABLE atmospheres ENABLE ROW LEVEL SECURITY;
ALTER TABLE moons ENABLE ROW LEVEL SECURITY;
ALTER TABLE api_snapshots ENABLE ROW LEVEL SECURITY;
ALTER TABLE "SimulationStates" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "ContactSubmissions" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "UserFeedbacks" ENABLE ROW LEVEL SECURITY;

ALTER TABLE geo_countries ENABLE ROW LEVEL SECURITY;
ALTER TABLE geo_states ENABLE ROW LEVEL SECURITY;
ALTER TABLE geo_cities ENABLE ROW LEVEL SECURITY;
ALTER TABLE geo_landmarks ENABLE ROW LEVEL SECURITY;

-- =====================================================
-- 7. RLS POLICIES (FINAL, VALID)
-- =====================================================
DO $$
DECLARE
  t RECORD;
  suffix TEXT;
BEGIN
  FOR t IN
    SELECT tablename
    FROM pg_tables
    WHERE schemaname = 'public'
      AND tablename <> 'spatial_ref_sys'
  LOOP
    suffix := lower(t.tablename);

    EXECUTE format(
      'DROP POLICY IF EXISTS public_read_%s ON %I',
      suffix, t.tablename
    );
    EXECUTE format(
      'CREATE POLICY public_read_%s ON %I FOR SELECT USING (true)',
      suffix, t.tablename
    );

    EXECUTE format(
      'DROP POLICY IF EXISTS service_write_%s ON %I',
      suffix, t.tablename
    );
    EXECUTE format(
      'CREATE POLICY service_write_%s ON %I
       FOR ALL
       USING (auth.role() = ''service_role'')
       WITH CHECK (auth.role() = ''service_role'')',
      suffix, t.tablename
    );
  END LOOP;
END $$;

-- =====================================================
-- 8. PERMISSIONS
-- =====================================================
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO anon, authenticated;
REVOKE ALL ON public.spatial_ref_sys FROM anon, authenticated;
GRANT SELECT ON public.spatial_ref_sys TO anon, authenticated;

-- =====================================================
-- 9. SEED DATA - India & Andhra Pradesh
-- =====================================================
INSERT INTO geo_countries (name, iso_code, capital, population, area_sq_km, continent, center_lat, center_lon)
VALUES ('India', 'IND', 'New Delhi', 1400000000, 3287263, 'Asia', 20.5937, 78.9629)
ON CONFLICT (iso_code) DO NOTHING;

INSERT INTO geo_states (name, country_id, state_code, capital, population, area_sq_km, center_lat, center_lon)
SELECT 'Andhra Pradesh', id, 'AP', 'Amaravati', 49500000, 162968, 15.9129, 79.7400 FROM geo_countries WHERE iso_code = 'IND'
ON CONFLICT DO NOTHING;

INSERT INTO geo_states (name, country_id, state_code, capital, population, area_sq_km, center_lat, center_lon)
SELECT 'Telangana', id, 'TG', 'Hyderabad', 35000000, 112077, 17.8495, 79.1151 FROM geo_countries WHERE iso_code = 'IND'
ON CONFLICT DO NOTHING;
