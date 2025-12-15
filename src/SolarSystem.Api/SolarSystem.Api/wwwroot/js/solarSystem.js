/**
 * Solar System 3D Visualization using Three.js
 */

let scene, camera, renderer, controls;
let planets = {};
let sun;
let animationId;
let simulationTime = new Date();
let timeMultiplier = 100;
let isPaused = false;

// Planet colors
const planetColors = {
    'Sun': 0xffff00,
    'Mercury': 0x8c8c8c,
    'Venus': 0xffc649,
    'Earth': 0x6b93d6,
    'Mars': 0xc1440e,
    'Jupiter': 0xd8ca9d,
    'Saturn': 0xf4d59e,
    'Uranus': 0xd1e7e7,
    'Neptune': 0x5b5ddf
};

// Fixed distances for visibility
const planetDistances = {
    'Mercury': 25,
    'Venus': 35,
    'Earth': 50,
    'Mars': 70,
    'Jupiter': 100,
    'Saturn': 130,
    'Uranus': 160,
    'Neptune': 190
};

// Planet sizes (relative)
const planetSizes = {
    'Sun': 12,
    'Mercury': 1.5,
    'Venus': 2.5,
    'Earth': 2.5,
    'Mars': 2,
    'Jupiter': 6,
    'Saturn': 5,
    'Uranus': 4,
    'Neptune': 4
};

/**
 * Initialize scene
 */
window.initSolarSystem = function (containerId, dotNetRef) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error('Container not found:', containerId);
        return;
    }

    window.dotNetRef = dotNetRef;

    // Scene
    scene = new THREE.Scene();
    scene.background = new THREE.Color(0x000510);

    // Camera
    camera = new THREE.PerspectiveCamera(60, container.clientWidth / container.clientHeight, 0.1, 10000);
    camera.position.set(0, 80, 200);

    // Renderer
    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(container.clientWidth, container.clientHeight);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    container.appendChild(renderer.domElement);

    // Controls
    controls = new THREE.OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;
    controls.dampingFactor = 0.05;
    controls.minDistance = 20;
    controls.maxDistance = 500;

    // Lights
    scene.add(new THREE.AmbientLight(0x222222));
    const sunLight = new THREE.PointLight(0xffffff, 2, 500);
    scene.add(sunLight);

    // Stars
    createStars();

    // Click handler
    renderer.domElement.addEventListener('click', onPlanetClick);

    // Resize
    window.addEventListener('resize', () => {
        camera.aspect = container.clientWidth / container.clientHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(container.clientWidth, container.clientHeight);
    });

    // Start animation
    animate();
    console.log('Solar System initialized');
};

function createStars() {
    const geometry = new THREE.BufferGeometry();
    const positions = [];
    for (let i = 0; i < 3000; i++) {
        positions.push((Math.random() - 0.5) * 2000);
        positions.push((Math.random() - 0.5) * 2000);
        positions.push((Math.random() - 0.5) * 2000);
    }
    geometry.setAttribute('position', new THREE.Float32BufferAttribute(positions, 3));
    scene.add(new THREE.Points(geometry, new THREE.PointsMaterial({ color: 0xffffff, size: 0.5 })));
}

/**
 * Load planets
 */
window.loadPlanets = function (celestialBodies) {
    // Clear existing
    Object.values(planets).forEach(p => {
        if (p.mesh) scene.remove(p.mesh);
        if (p.orbit) scene.remove(p.orbit);
    });
    planets = {};

    celestialBodies.forEach(body => {
        if (body.bodyType === 'Star') {
            createSun(body);
        } else if (body.isPlanet) {
            createPlanet(body);
        }
    });

    console.log('Loaded', Object.keys(planets).length, 'bodies');
};

function createSun(body) {
    const size = planetSizes['Sun'];
    const geometry = new THREE.SphereGeometry(size, 32, 32);
    const material = new THREE.MeshBasicMaterial({ color: 0xffff00 });
    const mesh = new THREE.Mesh(geometry, material);
    mesh.userData = { id: body.id, name: body.englishName };

    // Glow
    const glowGeo = new THREE.SphereGeometry(size * 1.3, 32, 32);
    const glowMat = new THREE.MeshBasicMaterial({ color: 0xffaa00, transparent: true, opacity: 0.3 });
    mesh.add(new THREE.Mesh(glowGeo, glowMat));

    scene.add(mesh);
    sun = mesh;
    planets['Sun'] = { mesh, data: body, distance: 0, angle: 0, speed: 0 };
}

function createPlanet(body) {
    const name = body.englishName;
    const distance = planetDistances[name] || 50;
    const size = planetSizes[name] || 2;
    const color = planetColors[name] || 0xcccccc;

    // Planet mesh
    const geometry = new THREE.SphereGeometry(size, 32, 32);
    const material = new THREE.MeshStandardMaterial({ color, roughness: 0.8, metalness: 0.2 });
    const mesh = new THREE.Mesh(geometry, material);
    mesh.userData = { id: body.id, name };

    // Random start angle
    const angle = Math.random() * Math.PI * 2;
    mesh.position.set(Math.cos(angle) * distance, 0, Math.sin(angle) * distance);
    scene.add(mesh);

    // Orbit ring
    const orbitGeo = new THREE.RingGeometry(distance - 0.2, distance + 0.2, 128);
    const orbitMat = new THREE.MeshBasicMaterial({ color: 0x333355, side: THREE.DoubleSide, transparent: true, opacity: 0.3 });
    const orbit = new THREE.Mesh(orbitGeo, orbitMat);
    orbit.rotation.x = Math.PI / 2;
    scene.add(orbit);

    // Speed based on orbital period (faster for inner planets)
    const orbitalPeriod = body.orbit?.orbitalPeriod || 365;
    const speed = (2 * Math.PI) / orbitalPeriod * 0.5;

    planets[name] = { mesh, orbit, data: body, distance, angle, speed };
}

/**
 * Animation loop
 */
function animate() {
    animationId = requestAnimationFrame(animate);

    if (!isPaused) {
        // Update planet positions
        Object.entries(planets).forEach(([name, planet]) => {
            if (planet.distance > 0) {
                planet.angle += planet.speed * timeMultiplier * 0.001;
                planet.mesh.position.x = Math.cos(planet.angle) * planet.distance;
                planet.mesh.position.z = Math.sin(planet.angle) * planet.distance;
                planet.mesh.rotation.y += 0.01;
            }
        });

        // Sun rotation
        if (sun) sun.rotation.y += 0.002;
    }

    controls.update();
    renderer.render(scene, camera);
}

/**
 * Planet click
 */
function onPlanetClick(event) {
    const rect = renderer.domElement.getBoundingClientRect();
    const mouse = new THREE.Vector2(
        ((event.clientX - rect.left) / rect.width) * 2 - 1,
        -((event.clientY - rect.top) / rect.height) * 2 + 1
    );

    const raycaster = new THREE.Raycaster();
    raycaster.setFromCamera(mouse, camera);

    const meshes = Object.values(planets).map(p => p.mesh);
    const hits = raycaster.intersectObjects(meshes);

    if (hits.length > 0) {
        const { id, name } = hits[0].object.userData;
        focusOnPlanet(name);
        if (window.dotNetRef) {
            window.dotNetRef.invokeMethodAsync('OnPlanetSelected', id, name);
        }
    }
}

function focusOnPlanet(name) {
    const planet = planets[name];
    if (!planet) return;

    const target = planet.mesh.position.clone();
    const offset = new THREE.Vector3(30, 15, 30);

    // Animate camera
    const start = camera.position.clone();
    const end = target.clone().add(offset);
    let t = 0;

    function animateCamera() {
        t += 0.03;
        if (t < 1) {
            camera.position.lerpVectors(start, end, t);
            controls.target.lerp(target, t);
            requestAnimationFrame(animateCamera);
        } else {
            controls.target.copy(target);
        }
    }
    animateCamera();
}

// External controls
window.togglePause = function (paused) {
    isPaused = paused;
    console.log('Paused:', paused);
};

window.setTimeMultiplier = function (mult) {
    timeMultiplier = mult;
    console.log('Speed:', mult);
};

window.focusPlanet = function (name) {
    focusOnPlanet(name);
};

window.resetView = function () {
    camera.position.set(0, 80, 200);
    controls.target.set(0, 0, 0);
};

window.disposeScene = function () {
    if (animationId) cancelAnimationFrame(animationId);
    if (renderer) renderer.dispose();
};
