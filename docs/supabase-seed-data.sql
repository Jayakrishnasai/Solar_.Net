-- =====================================================
-- COMPLETE SOLAR SYSTEM SEED (SUPABASE SAFE)
-- =====================================================

-- ---------- CLEAN EXISTING DATA (FK SAFE ORDER) ----------
DELETE FROM moons;
DELETE FROM planet_layers;
DELETE FROM atmospheres;
DELETE FROM orbits;
DELETE FROM planets;
DELETE FROM celestial_bodies;

-- =====================================================
-- 1. CELESTIAL BODIES (SUN + PLANETS)
-- =====================================================
INSERT INTO celestial_bodies (external_id, name, english_name, body_type, is_planet, description) VALUES
('sun', 'Soleil', 'Sun', 'Star', false, 'The Sun is the central star of the Solar System'),
('mercury', 'Mercure', 'Mercury', 'Planet', true, 'Smallest planet'),
('venus', 'Vénus', 'Venus', 'Planet', true, 'Hottest planet'),
('earth', 'Terre', 'Earth', 'Planet', true, 'Home planet'),
('mars', 'Mars', 'Mars', 'Planet', true, 'Red planet'),
('jupiter', 'Jupiter', 'Jupiter', 'Planet', true, 'Largest planet'),
('saturn', 'Saturne', 'Saturn', 'Planet', true, 'Ringed planet'),
('uranus', 'Uranus', 'Uranus', 'Planet', true, 'Tilted planet'),
('neptune', 'Neptune', 'Neptune', 'Planet', true, 'Farthest planet'),
('pluto', 'Pluton', 'Pluto', 'Dwarf Planet', false, 'Dwarf planet');

-- =====================================================
-- 2. PLANETS (PHYSICAL PROPERTIES)
-- =====================================================
INSERT INTO planets (celestial_body_id, mean_radius_km, gravity, has_rings, has_atmosphere)
SELECT id, r, g, rings, atm FROM (
  VALUES
  ('mercury', 2439.7, 3.7, false, false),
  ('venus', 6051.8, 8.87, false, true),
  ('earth', 6371, 9.807, false, true),
  ('mars', 3389.5, 3.71, false, true),
  ('jupiter', 69911, 24.79, true, true),
  ('saturn', 58232, 10.44, true, true),
  ('uranus', 25362, 8.69, true, true),
  ('neptune', 24622, 11.15, true, true)
) AS d(ext, r, g, rings, atm)
JOIN celestial_bodies c ON c.external_id = d.ext;

-- =====================================================
-- 3. ORBITS
-- =====================================================
INSERT INTO orbits (celestial_body_id, semi_major_axis_km, orbital_period_days)
SELECT id, dist, period FROM (
  VALUES
  ('mercury', 57909050, 87.97),
  ('venus', 108208000, 224.7),
  ('earth', 149598023, 365.25),
  ('mars', 227939200, 686.98),
  ('jupiter', 778570000, 4332.6),
  ('saturn', 1433530000, 10759),
  ('uranus', 2872460000, 30688),
  ('neptune', 4495060000, 60182)
) AS d(ext, dist, period)
JOIN celestial_bodies c ON c.external_id = d.ext;

-- =====================================================
-- 4. ATMOSPHERES
-- =====================================================
INSERT INTO atmospheres (planet_id, primary_gases)
SELECT p.id, gases FROM (
  VALUES
  ('venus', 'CO2'),
  ('earth', 'N2, O2'),
  ('mars', 'CO2'),
  ('jupiter', 'H2, He'),
  ('saturn', 'H2, He'),
  ('uranus', 'H2, He, CH4'),
  ('neptune', 'H2, He, CH4')
) AS d(ext, gases)
JOIN celestial_bodies c ON c.external_id = d.ext
JOIN planets p ON p.celestial_body_id = c.id;

-- =====================================================
-- 5. MOONS (PROPERLY MODELED)
-- =====================================================

-- Moon celestial bodies
INSERT INTO celestial_bodies (external_id, name, english_name, body_type, is_planet, description) VALUES
('moon', 'Lune', 'Moon', 'Moon', false, 'Earth''s moon'),
('phobos', 'Phobos', 'Phobos', 'Moon', false, 'Moon of Mars'),
('deimos', 'Deimos', 'Deimos', 'Moon', false, 'Moon of Mars'),
('io', 'Io', 'Io', 'Moon', false, 'Moon of Jupiter'),
('europa', 'Europa', 'Europa', 'Moon', false, 'Moon of Jupiter'),
('ganymede', 'Ganymede', 'Ganymede', 'Moon', false, 'Moon of Jupiter'),
('callisto', 'Callisto', 'Callisto', 'Moon', false, 'Moon of Jupiter'),
('titan', 'Titan', 'Titan', 'Moon', false, 'Moon of Saturn'),
('triton', 'Triton', 'Triton', 'Moon', false, 'Moon of Neptune');

-- Link moons to planets
INSERT INTO moons (celestial_body_id, planet_id, mean_radius_km, orbital_period_days)
SELECT cb.id, p.id, r, d FROM (
  VALUES
  ('moon', 'earth', 1737.4, 27.3),
  ('phobos', 'mars', 11.1, 0.319),
  ('deimos', 'mars', 6.2, 1.263),
  ('io', 'jupiter', 1821.6, 1.769),
  ('europa', 'jupiter', 1560.8, 3.551),
  ('ganymede', 'jupiter', 2634.1, 7.155),
  ('callisto', 'jupiter', 2410.3, 16.689),
  ('titan', 'saturn', 2574.7, 15.95),
  ('triton', 'neptune', 1353.4, 5.88)
) AS d(moon, planet, r, d)
JOIN celestial_bodies cb ON cb.external_id = d.moon
JOIN celestial_bodies cp ON cp.external_id = d.planet
JOIN planets p ON p.celestial_body_id = cp.id;

-- =====================================================
-- VERIFY
-- =====================================================
SELECT 'celestial_bodies' AS table, COUNT(*) FROM celestial_bodies
UNION ALL SELECT 'planets', COUNT(*) FROM planets
UNION ALL SELECT 'moons', COUNT(*) FROM moons;
