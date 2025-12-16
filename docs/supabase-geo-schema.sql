-- Supabase Geo Data Schema
-- Run this in Supabase SQL Editor

-- Enable PostGIS extension
CREATE EXTENSION IF NOT EXISTS postgis;

-- Countries table
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

-- States/Provinces table
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

-- Cities table
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

-- Landmarks table
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

-- Create spatial indexes
CREATE INDEX IF NOT EXISTS idx_countries_geometry ON geo_countries USING GIST(geometry);
CREATE INDEX IF NOT EXISTS idx_states_geometry ON geo_states USING GIST(geometry);
CREATE INDEX IF NOT EXISTS idx_cities_location ON geo_cities USING GIST(location);
CREATE INDEX IF NOT EXISTS idx_landmarks_location ON geo_landmarks USING GIST(location);

-- Enable RLS
ALTER TABLE geo_countries ENABLE ROW LEVEL SECURITY;
ALTER TABLE geo_states ENABLE ROW LEVEL SECURITY;
ALTER TABLE geo_cities ENABLE ROW LEVEL SECURITY;
ALTER TABLE geo_landmarks ENABLE ROW LEVEL SECURITY;

-- Public read policies
CREATE POLICY "Allow public read countries" ON geo_countries FOR SELECT USING (true);
CREATE POLICY "Allow public read states" ON geo_states FOR SELECT USING (true);
CREATE POLICY "Allow public read cities" ON geo_cities FOR SELECT USING (true);
CREATE POLICY "Allow public read landmarks" ON geo_landmarks FOR SELECT USING (true);

-- Insert India
INSERT INTO geo_countries (name, iso_code, capital, population, area_sq_km, continent, center_lat, center_lon)
VALUES ('India', 'IND', 'New Delhi', 1400000000, 3287263, 'Asia', 20.5937, 78.9629);

-- Insert Andhra Pradesh
INSERT INTO geo_states (name, country_id, state_code, capital, population, area_sq_km, center_lat, center_lon)
VALUES ('Andhra Pradesh', 1, 'AP', 'Amaravati', 49500000, 162968, 15.9129, 79.7400);

-- Insert Telangana
INSERT INTO geo_states (name, country_id, state_code, capital, population, area_sq_km, center_lat, center_lon)
VALUES ('Telangana', 1, 'TG', 'Hyderabad', 35000000, 112077, 17.8495, 79.1151);

-- Insert AP Cities
INSERT INTO geo_cities (name, state_id, country_id, population, latitude, longitude, is_state_capital) VALUES
('Amaravati', 1, 1, 100000, 16.5131, 80.5150, TRUE),
('Visakhapatnam', 1, 1, 2035000, 17.6868, 83.2185, FALSE),
('Vijayawada', 1, 1, 1500000, 16.5062, 80.6480, FALSE),
('Guntur', 1, 1, 750000, 16.3067, 80.4365, FALSE),
('Nellore', 1, 1, 600000, 14.4426, 79.9865, FALSE),
('Tirupati', 1, 1, 500000, 13.6288, 79.4192, FALSE),
('Rajahmundry', 1, 1, 500000, 17.0005, 81.8040, FALSE),
('Kakinada', 1, 1, 400000, 16.9891, 82.2475, FALSE),
('Kurnool', 1, 1, 480000, 15.8281, 78.0373, FALSE),
('Kadapa', 1, 1, 350000, 14.4674, 78.8241, FALSE),
('Anantapur', 1, 1, 340000, 14.6819, 77.6006, FALSE);

-- Insert Telangana Cities
INSERT INTO geo_cities (name, state_id, country_id, population, latitude, longitude, is_state_capital) VALUES
('Hyderabad', 2, 1, 10000000, 17.3850, 78.4867, TRUE),
('Warangal', 2, 1, 700000, 17.9689, 79.5941, FALSE),
('Nizamabad', 2, 1, 310000, 18.6725, 78.0940, FALSE),
('Karimnagar', 2, 1, 260000, 18.4386, 79.1288, FALSE),
('Khammam', 2, 1, 200000, 17.2473, 80.1514, FALSE);

-- Update geometry from lat/lon for cities
UPDATE geo_cities SET location = ST_SetSRID(ST_MakePoint(longitude, latitude), 4326);
