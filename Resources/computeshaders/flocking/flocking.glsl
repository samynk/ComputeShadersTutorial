#version 430 core

layout(local_size_x = 32) in; // Workgroup size

struct Boid {
        vec2 position;
        vec2 velocity;
};
// Input/output boid data
layout(std430, binding = 0) buffer BoidDataOld {
    Boid inputboids[];
};

layout(std430, binding = 1) buffer BoidDataNew {
    Boid outputboids[];
};

// Flocking parameters
uniform float deltaTime= 0.016;
uniform float perceptionRadius = 50.0;
uniform float cohesionFactor = 2.75;
uniform float separationFactor = 2.25;
uniform float alignmentFactor = 0.3;
uniform float maxSpeed = 4.0;

void main() {
    uint id = gl_GlobalInvocationID.x;  // Each thread processes one boid

    if (id >= inputboids.length()) return;

    vec2 currentPosition = inputboids[id].position;
    vec2 currentVelocity = inputboids[id].velocity;

    vec2 alignment = vec2(0.0);
    vec2 cohesion = vec2(0.0);
    vec2 separation = vec2(0.0);

    int countAlignment = 0;
    int countCohesion = 0;
    int countSeparation = 0;

    // Loop through all boids to calculate alignment, cohesion, and separation
    for (uint i = 0; i < inputboids.length(); ++i) {
        if (i == id) continue; // Skip self

        float distance = length(inputboids[i].position - currentPosition);
        if (distance < perceptionRadius) {
            // Alignment: match velocity with nearby boids
            alignment += inputboids[i].velocity;
            countAlignment++;

            // Cohesion: steer towards average position of neighbors
            cohesion += inputboids[i].position;
            countCohesion++;

            // Separation: steer to avoid crowding
            if (distance < perceptionRadius * 0.5) {
                separation -= (inputboids[i].position - currentPosition) / distance; // Weight by inverse distance
                countSeparation++;
            }
        }
    }

    // Average the values and apply weights if there are neighbors
    if (countAlignment > 0) {
        alignment /= countAlignment;
        alignment = normalize(alignment) *alignmentFactor + currentVelocity*(1-alignmentFactor);
    }

    if (countCohesion > 0) {
        cohesion /= countCohesion;
        cohesion = cohesionFactor*normalize(cohesion - currentPosition);
    }

    if (countSeparation > 0) {
        separation /= countSeparation;
        separation = separationFactor*normalize(separation);
    }

    // Update velocity with the combination of alignment, cohesion, and separation
    vec2 acceleration = alignment + cohesion + separation;
    vec2 newVelocity = currentVelocity + acceleration * deltaTime;
    newVelocity = normalize(newVelocity) * min(maxSpeed, length(newVelocity)); // Clamp speed to maxSpeed

    // Update position
    outputboids[id].velocity = newVelocity;
    outputboids[id].position = inputboids[id].position + newVelocity * deltaTime;
}
