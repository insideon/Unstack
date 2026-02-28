using UnityEngine;

namespace Unstack.Animation
{
    public static class ParticleEffectFactory
    {
        public static void CreateBurstEffect(Vector3 position, Color color)
        {
            var go = new GameObject("ParticleBurst");
            go.transform.position = position;

            var ps = go.AddComponent<ParticleSystem>();

            // Stop auto-play to configure first
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.startLifetime = 0.6f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = color;
            main.gravityModifier = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 20;
            main.stopAction = ParticleSystemStopAction.Destroy;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 20)
            });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f,
                new AnimationCurve(
                    new Keyframe(0f, 1f),
                    new Keyframe(1f, 0f)));

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            colorOverLifetime.color = gradient;

            // Use default particle material
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            var shader = Shader.Find("Particles/Standard Unlit")
                      ?? Shader.Find("Universal Render Pipeline/Particles/Unlit")
                      ?? Shader.Find("Sprites/Default");
            if (shader != null)
            {
                renderer.material = new Material(shader);
                renderer.material.color = color;
            }

            ps.Play();
        }
    }
}
