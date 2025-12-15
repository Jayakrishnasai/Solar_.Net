using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SolarSystem.Shared.Models;

namespace SolarSystem.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(SolarSystemDbContext context)
    {
        if (await context.CelestialBodies.AnyAsync())
        {
            return; // Already seeded
        }

        var celestialBodies = GetFallbackData();
        
        context.CelestialBodies.AddRange(celestialBodies);
        await context.SaveChangesAsync();
    }

    public static List<CelestialBody> GetFallbackData()
    {
        return new List<CelestialBody>
        {
            // Sun
            new CelestialBody
            {
                ApiId = "soleil",
                Name = "Soleil",
                EnglishName = "Sun",
                BodyType = "Star",
                Mass = 1.989,
                MassExponent = 30,
                MeanRadius = 696340,
                EquatorialRadius = 696340,
                Density = 1.408,
                Gravity = 274,
                EscapeSpeed = 617700,
                MeanTemperature = 5778,
                SiderealRotation = 609.12,
                IsPlanet = false,
                TextureUrl = "textures/sun.jpg",
                Description = "The Sun is the star at the center of the Solar System. It is a nearly perfect sphere of hot plasma, with internal convective motion that generates a magnetic field via a dynamo process.",
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 174000, Temperature = 15000000, Composition = "Hydrogen, Helium", State = "Plasma", Color = "#FFFFFF" },
                    new() { LayerName = "Radiative Zone", LayerOrder = 2, InnerRadius = 174000, OuterRadius = 487000, Temperature = 7000000, Composition = "Hydrogen, Helium", State = "Plasma", Color = "#FFF5E0" },
                    new() { LayerName = "Convective Zone", LayerOrder = 3, InnerRadius = 487000, OuterRadius = 696340, Temperature = 2000000, Composition = "Hydrogen, Helium", State = "Plasma", Color = "#FFE4B5" }
                }
            },

            // Mercury
            new CelestialBody
            {
                ApiId = "mercure",
                Name = "Mercure",
                EnglishName = "Mercury",
                BodyType = "Planet",
                Mass = 3.3011,
                MassExponent = 23,
                MeanRadius = 2439.7,
                EquatorialRadius = 2439.7,
                PolarRadius = 2439.7,
                Density = 5.427,
                Gravity = 3.7,
                EscapeSpeed = 4300,
                MeanTemperature = 440,
                SiderealRotation = 1407.6,
                AxialTilt = 0.034,
                IsPlanet = true,
                TextureUrl = "textures/mercury.jpg",
                Description = "Mercury is the smallest and innermost planet in the Solar System. It has no atmosphere to retain heat, resulting in extreme temperature variations.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 57909050,
                    Perihelion = 46001200,
                    Aphelion = 69816900,
                    Eccentricity = 0.2056,
                    Inclination = 7.005,
                    OrbitalPeriod = 87.969,
                    SiderealOrbit = 87.969,
                    MeanAnomaly = 174.796
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Inner Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 1000, Temperature = 1800, Composition = "Iron", State = "Solid", Color = "#8B4513" },
                    new() { LayerName = "Outer Core", LayerOrder = 2, InnerRadius = 1000, OuterRadius = 1800, Temperature = 2200, Composition = "Iron, Sulfur", State = "Liquid", Color = "#CD853F" },
                    new() { LayerName = "Mantle", LayerOrder = 3, InnerRadius = 1800, OuterRadius = 2340, Temperature = 1200, Composition = "Silicates", State = "Solid", Color = "#A0522D" },
                    new() { LayerName = "Crust", LayerOrder = 4, InnerRadius = 2340, OuterRadius = 2440, Temperature = 440, Composition = "Silicates", State = "Solid", Color = "#808080" }
                }
            },

            // Venus
            new CelestialBody
            {
                ApiId = "venus",
                Name = "Vénus",
                EnglishName = "Venus",
                BodyType = "Planet",
                Mass = 4.8675,
                MassExponent = 24,
                MeanRadius = 6051.8,
                EquatorialRadius = 6051.8,
                PolarRadius = 6051.8,
                Density = 5.243,
                Gravity = 8.87,
                EscapeSpeed = 10360,
                MeanTemperature = 737,
                SiderealRotation = -5832.5,
                AxialTilt = 177.36,
                IsPlanet = true,
                TextureUrl = "textures/venus.jpg",
                Description = "Venus is the second planet from the Sun. It has the densest atmosphere of the four terrestrial planets and is the hottest planet in the Solar System.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 108208000,
                    Perihelion = 107477000,
                    Aphelion = 108939000,
                    Eccentricity = 0.0067,
                    Inclination = 3.39,
                    OrbitalPeriod = 224.701,
                    SiderealOrbit = 224.701,
                    MeanAnomaly = 50.115
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "96.5% Carbon Dioxide, 3.5% Nitrogen",
                    SurfacePressure = 92,
                    Color = "#FFA500",
                    Description = "Venus has a thick, toxic atmosphere filled with carbon dioxide and clouds of sulfuric acid."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 3200, Temperature = 5000, Composition = "Iron, Nickel", State = "Solid/Liquid", Color = "#DAA520" },
                    new() { LayerName = "Mantle", LayerOrder = 2, InnerRadius = 3200, OuterRadius = 6000, Temperature = 3000, Composition = "Silicates", State = "Solid", Color = "#CD853F" },
                    new() { LayerName = "Crust", LayerOrder = 3, InnerRadius = 6000, OuterRadius = 6052, Temperature = 737, Composition = "Basalt", State = "Solid", Color = "#8B4513" }
                }
            },

            // Earth
            new CelestialBody
            {
                ApiId = "terre",
                Name = "Terre",
                EnglishName = "Earth",
                BodyType = "Planet",
                Mass = 5.97237,
                MassExponent = 24,
                MeanRadius = 6371,
                EquatorialRadius = 6378.1,
                PolarRadius = 6356.8,
                Density = 5.514,
                Gravity = 9.807,
                EscapeSpeed = 11186,
                MeanTemperature = 288,
                SiderealRotation = 23.9345,
                AxialTilt = 23.44,
                IsPlanet = true,
                TextureUrl = "textures/earth.jpg",
                Description = "Earth is the third planet from the Sun and the only astronomical object known to harbor life. About 71% of Earth's surface is water-covered.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 149598023,
                    Perihelion = 147092000,
                    Aphelion = 152099000,
                    Eccentricity = 0.0167,
                    Inclination = 0,
                    OrbitalPeriod = 365.256,
                    SiderealOrbit = 365.256,
                    MeanAnomaly = 358.617
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "78% Nitrogen, 21% Oxygen, 1% Argon",
                    SurfacePressure = 1,
                    Color = "#87CEEB",
                    Description = "Earth's atmosphere protects life by absorbing ultraviolet radiation and moderating temperatures."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Inner Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 1220, Temperature = 5400, Composition = "Iron, Nickel", State = "Solid", Color = "#FFD700" },
                    new() { LayerName = "Outer Core", LayerOrder = 2, InnerRadius = 1220, OuterRadius = 3400, Temperature = 4500, Composition = "Iron, Nickel, Sulfur", State = "Liquid", Color = "#FFA500" },
                    new() { LayerName = "Mantle", LayerOrder = 3, InnerRadius = 3400, OuterRadius = 6335, Temperature = 3000, Composition = "Silicates, Magnesium, Iron", State = "Solid/Plastic", Color = "#CD853F" },
                    new() { LayerName = "Crust", LayerOrder = 4, InnerRadius = 6335, OuterRadius = 6371, Temperature = 288, Composition = "Silicates, Aluminum", State = "Solid", Color = "#228B22" }
                },
                Moons = new List<Moon>
                {
                    new Moon
                    {
                        ApiId = "lune",
                        Name = "Moon",
                        Radius = 1737.4,
                        Mass = 7.342,
                        MassExponent = 22,
                        OrbitalRadius = 384400,
                        OrbitalPeriod = 27.3,
                        Eccentricity = 0.0549,
                        Inclination = 5.145,
                        Description = "The Moon is Earth's only natural satellite. It is the fifth largest moon in the Solar System."
                    }
                }
            },

            // Mars
            new CelestialBody
            {
                ApiId = "mars",
                Name = "Mars",
                EnglishName = "Mars",
                BodyType = "Planet",
                Mass = 6.4171,
                MassExponent = 23,
                MeanRadius = 3389.5,
                EquatorialRadius = 3396.2,
                PolarRadius = 3376.2,
                Density = 3.9335,
                Gravity = 3.711,
                EscapeSpeed = 5027,
                MeanTemperature = 210,
                SiderealRotation = 24.6229,
                AxialTilt = 25.19,
                IsPlanet = true,
                TextureUrl = "textures/mars.jpg",
                Description = "Mars is the fourth planet from the Sun and the second-smallest planet in the Solar System. It is often referred to as the 'Red Planet'.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 227939200,
                    Perihelion = 206700000,
                    Aphelion = 249200000,
                    Eccentricity = 0.0934,
                    Inclination = 1.85,
                    OrbitalPeriod = 686.98,
                    SiderealOrbit = 686.98,
                    MeanAnomaly = 19.412
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "95.3% Carbon Dioxide, 2.7% Nitrogen, 1.6% Argon",
                    SurfacePressure = 0.00636,
                    Color = "#FFA07A",
                    Description = "Mars has a thin atmosphere composed mainly of carbon dioxide."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 1700, Temperature = 1500, Composition = "Iron, Nickel, Sulfur", State = "Liquid", Color = "#8B0000" },
                    new() { LayerName = "Mantle", LayerOrder = 2, InnerRadius = 1700, OuterRadius = 3350, Temperature = 1500, Composition = "Silicates", State = "Solid", Color = "#CD5C5C" },
                    new() { LayerName = "Crust", LayerOrder = 3, InnerRadius = 3350, OuterRadius = 3390, Temperature = 210, Composition = "Iron Oxide, Basalt", State = "Solid", Color = "#B22222" }
                },
                Moons = new List<Moon>
                {
                    new Moon { ApiId = "phobos", Name = "Phobos", Radius = 11.1, Mass = 1.0659, MassExponent = 16, OrbitalRadius = 9376, OrbitalPeriod = 0.319 },
                    new Moon { ApiId = "deimos", Name = "Deimos", Radius = 6.2, Mass = 1.4762, MassExponent = 15, OrbitalRadius = 23458, OrbitalPeriod = 1.263 }
                }
            },

            // Jupiter
            new CelestialBody
            {
                ApiId = "jupiter",
                Name = "Jupiter",
                EnglishName = "Jupiter",
                BodyType = "Planet",
                Mass = 1.8982,
                MassExponent = 27,
                MeanRadius = 69911,
                EquatorialRadius = 71492,
                PolarRadius = 66854,
                Density = 1.326,
                Gravity = 24.79,
                EscapeSpeed = 59500,
                MeanTemperature = 165,
                SiderealRotation = 9.925,
                AxialTilt = 3.13,
                IsPlanet = true,
                TextureUrl = "textures/jupiter.jpg",
                Description = "Jupiter is the fifth planet from the Sun and the largest in the Solar System. It is a gas giant with a mass more than two and a half times that of all other planets combined.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 778570000,
                    Perihelion = 740520000,
                    Aphelion = 816620000,
                    Eccentricity = 0.0489,
                    Inclination = 1.303,
                    OrbitalPeriod = 4332.59,
                    SiderealOrbit = 4332.59,
                    MeanAnomaly = 20.020
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "89.8% Hydrogen, 10.2% Helium",
                    SurfacePressure = 100,
                    Color = "#FFA07A",
                    Description = "Jupiter has a turbulent atmosphere with the famous Great Red Spot, a storm larger than Earth."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 20000, Temperature = 36000, Composition = "Rock, Metal, Hydrogen compounds", State = "Solid/Liquid", Color = "#8B4513" },
                    new() { LayerName = "Metallic Hydrogen", LayerOrder = 2, InnerRadius = 20000, OuterRadius = 50000, Temperature = 20000, Composition = "Metallic Hydrogen", State = "Liquid", Color = "#C0C0C0" },
                    new() { LayerName = "Molecular Hydrogen", LayerOrder = 3, InnerRadius = 50000, OuterRadius = 69911, Temperature = 165, Composition = "Hydrogen, Helium", State = "Gas", Color = "#FFE4C4" }
                },
                Moons = new List<Moon>
                {
                    new Moon { ApiId = "io", Name = "Io", Radius = 1821.6, Mass = 8.9319, MassExponent = 22, OrbitalRadius = 421700, OrbitalPeriod = 1.769 },
                    new Moon { ApiId = "europa", Name = "Europa", Radius = 1560.8, Mass = 4.7998, MassExponent = 22, OrbitalRadius = 671034, OrbitalPeriod = 3.551 },
                    new Moon { ApiId = "ganymede", Name = "Ganymede", Radius = 2634.1, Mass = 1.4819, MassExponent = 23, OrbitalRadius = 1070412, OrbitalPeriod = 7.155 },
                    new Moon { ApiId = "callisto", Name = "Callisto", Radius = 2410.3, Mass = 1.0759, MassExponent = 23, OrbitalRadius = 1882709, OrbitalPeriod = 16.689 }
                }
            },

            // Saturn
            new CelestialBody
            {
                ApiId = "saturne",
                Name = "Saturne",
                EnglishName = "Saturn",
                BodyType = "Planet",
                Mass = 5.6834,
                MassExponent = 26,
                MeanRadius = 58232,
                EquatorialRadius = 60268,
                PolarRadius = 54364,
                Density = 0.687,
                Gravity = 10.44,
                EscapeSpeed = 35500,
                MeanTemperature = 134,
                SiderealRotation = 10.656,
                AxialTilt = 26.73,
                IsPlanet = true,
                TextureUrl = "textures/saturn.jpg",
                Description = "Saturn is the sixth planet from the Sun and is known for its prominent ring system composed of ice and rock particles.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 1433530000,
                    Perihelion = 1352550000,
                    Aphelion = 1514500000,
                    Eccentricity = 0.0565,
                    Inclination = 2.485,
                    OrbitalPeriod = 10759.22,
                    SiderealOrbit = 10759.22,
                    MeanAnomaly = 317.020
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "96.3% Hydrogen, 3.25% Helium",
                    SurfacePressure = 140,
                    Color = "#F4A460",
                    Description = "Saturn's atmosphere is primarily hydrogen with helium, similar to Jupiter but less turbulent."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 15000, Temperature = 21000, Composition = "Rock, Metal, Ice", State = "Solid", Color = "#8B4513" },
                    new() { LayerName = "Metallic Hydrogen", LayerOrder = 2, InnerRadius = 15000, OuterRadius = 35000, Temperature = 15000, Composition = "Metallic Hydrogen", State = "Liquid", Color = "#C0C0C0" },
                    new() { LayerName = "Molecular Hydrogen", LayerOrder = 3, InnerRadius = 35000, OuterRadius = 58232, Temperature = 134, Composition = "Hydrogen, Helium", State = "Gas", Color = "#F5DEB3" }
                },
                Moons = new List<Moon>
                {
                    new Moon { ApiId = "titan", Name = "Titan", Radius = 2574.73, Mass = 1.3452, MassExponent = 23, OrbitalRadius = 1221870, OrbitalPeriod = 15.945 },
                    new Moon { ApiId = "enceladus", Name = "Enceladus", Radius = 252.1, Mass = 1.08, MassExponent = 20, OrbitalRadius = 237948, OrbitalPeriod = 1.370 }
                }
            },

            // Uranus
            new CelestialBody
            {
                ApiId = "uranus",
                Name = "Uranus",
                EnglishName = "Uranus",
                BodyType = "Planet",
                Mass = 8.6810,
                MassExponent = 25,
                MeanRadius = 25362,
                EquatorialRadius = 25559,
                PolarRadius = 24973,
                Density = 1.27,
                Gravity = 8.69,
                EscapeSpeed = 21300,
                MeanTemperature = 76,
                SiderealRotation = -17.24,
                AxialTilt = 97.77,
                IsPlanet = true,
                TextureUrl = "textures/uranus.jpg",
                Description = "Uranus is the seventh planet from the Sun. It has the third-largest planetary radius and rotates at a nearly 90-degree angle.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 2872460000,
                    Perihelion = 2741300000,
                    Aphelion = 3003620000,
                    Eccentricity = 0.0457,
                    Inclination = 0.773,
                    OrbitalPeriod = 30688.5,
                    SiderealOrbit = 30688.5,
                    MeanAnomaly = 142.238
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "82.5% Hydrogen, 15.2% Helium, 2.3% Methane",
                    SurfacePressure = 100,
                    Color = "#87CEEB",
                    Description = "Uranus has a blue-green color due to methane in its atmosphere which absorbs red light."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 7500, Temperature = 5000, Composition = "Rock, Ice", State = "Solid", Color = "#696969" },
                    new() { LayerName = "Mantle", LayerOrder = 2, InnerRadius = 7500, OuterRadius = 20000, Temperature = 3000, Composition = "Water, Ammonia, Methane Ice", State = "Liquid/Ice", Color = "#4682B4" },
                    new() { LayerName = "Atmosphere", LayerOrder = 3, InnerRadius = 20000, OuterRadius = 25362, Temperature = 76, Composition = "Hydrogen, Helium, Methane", State = "Gas", Color = "#87CEEB" }
                },
                Moons = new List<Moon>
                {
                    new Moon { ApiId = "miranda", Name = "Miranda", Radius = 235.8, Mass = 6.59, MassExponent = 19, OrbitalRadius = 129390, OrbitalPeriod = 1.413 },
                    new Moon { ApiId = "ariel", Name = "Ariel", Radius = 578.9, Mass = 1.35, MassExponent = 21, OrbitalRadius = 191020, OrbitalPeriod = 2.520 }
                }
            },

            // Neptune
            new CelestialBody
            {
                ApiId = "neptune",
                Name = "Neptune",
                EnglishName = "Neptune",
                BodyType = "Planet",
                Mass = 1.02413,
                MassExponent = 26,
                MeanRadius = 24622,
                EquatorialRadius = 24764,
                PolarRadius = 24341,
                Density = 1.638,
                Gravity = 11.15,
                EscapeSpeed = 23500,
                MeanTemperature = 72,
                SiderealRotation = 16.11,
                AxialTilt = 28.32,
                IsPlanet = true,
                TextureUrl = "textures/neptune.jpg",
                Description = "Neptune is the eighth and farthest known planet from the Sun. It is the fourth-largest planet by diameter and has the strongest sustained winds.",
                Orbit = new Orbit
                {
                    SemimajorAxis = 4495060000,
                    Perihelion = 4444450000,
                    Aphelion = 4545670000,
                    Eccentricity = 0.0113,
                    Inclination = 1.77,
                    OrbitalPeriod = 60182,
                    SiderealOrbit = 60182,
                    MeanAnomaly = 256.228
                },
                Atmosphere = new Atmosphere
                {
                    HasAtmosphere = true,
                    Composition = "80% Hydrogen, 19% Helium, 1.5% Methane",
                    SurfacePressure = 100,
                    Color = "#1E90FF",
                    Description = "Neptune's atmosphere gives it a vivid blue color due to methane and unknown chromophores."
                },
                Layers = new List<PlanetLayer>
                {
                    new() { LayerName = "Core", LayerOrder = 1, InnerRadius = 0, OuterRadius = 8000, Temperature = 7000, Composition = "Rock, Ice", State = "Solid", Color = "#696969" },
                    new() { LayerName = "Mantle", LayerOrder = 2, InnerRadius = 8000, OuterRadius = 20000, Temperature = 5000, Composition = "Water, Ammonia, Methane Ice", State = "Liquid", Color = "#4169E1" },
                    new() { LayerName = "Atmosphere", LayerOrder = 3, InnerRadius = 20000, OuterRadius = 24622, Temperature = 72, Composition = "Hydrogen, Helium, Methane", State = "Gas", Color = "#1E90FF" }
                },
                Moons = new List<Moon>
                {
                    new Moon { ApiId = "triton", Name = "Triton", Radius = 1353.4, Mass = 2.14, MassExponent = 22, OrbitalRadius = 354759, OrbitalPeriod = 5.877 }
                }
            }
        };
    }
}
