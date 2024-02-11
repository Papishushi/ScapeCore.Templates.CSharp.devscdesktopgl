using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScapeCore.Core.Batching.Resources;
using ScapeCore.Core.Engine;
using ScapeCore.Core.Engine.Components;
using ScapeCore.Core.Physics2D;
using ScapeCore.Core.Serialization;
using Serilog;
using System;
using System.Threading.Tasks;


[ResourceLoad(typeof(Texture2D), "ball")]
public class Ball : MonoBehaviour
{
    static bool isFirst = true;
    public SpriteRenderer? sRenderer = null;
    private float xDirection = Random.Shared.Next(-1, 2) >= 0 ? Random.Shared.NextSingle() : -Random.Shared.NextSingle();
    private float yDirection = Random.Shared.Next(-1, 2) >= 0 ? Random.Shared.NextSingle() : -Random.Shared.NextSingle();
    private float xIntensity = 100f;
    private float yIntensity = 100f;

    protected override void Start()
    {
        while (gameObject == null)
        {
            Task.Delay(100).Wait();
            if (gameObject != null)
                break;
        }

        sRenderer = gameObject.AddBehaviour<SpriteRenderer>();

        while (sRenderer == null)
        {
            Task.Delay(100).Wait();
            sRenderer = gameObject.AddBehaviour<SpriteRenderer>();
            if (sRenderer != null)
                break;
        }

        sRenderer.texture = ResourceManager.GetResource<Texture2D>("ball").Value;
        var size = new Point(sRenderer.texture!.Width, sRenderer.texture.Height);
        var center = Point.Zero;
        sRenderer.rtransform = new RectTransform(size, center, Vector2.Zero, Vector2.Zero, Vector2.One);
        transform!.Position = new(Random.Shared.Next(game!.Graphics.PreferredBackBufferWidth),
                                         Random.Shared.Next(game!.Graphics.PreferredBackBufferHeight));
    }

    protected override void Update()
    {
        while (gameObject == null)
        {
            Task.Delay(100).Wait();
            if (gameObject != null)
                break;
        }
        while (sRenderer == null)
        {
            Task.Delay(100).Wait();
            sRenderer = gameObject.AddBehaviour<SpriteRenderer>();
            sRenderer.texture = ResourceManager.GetResource<Texture2D>("ball").Value;
            if (sRenderer != null)
                break;
        }
        if (transform == null)
            return;

        transform.Position += new Vector2(xDirection * xIntensity , yDirection * yIntensity) * GetDeltaTime(Time);

        if (transform.Position.X >= game?.Graphics.PreferredBackBufferWidth - sRenderer.texture?.Width * 0.5f ||
            transform.Position.X <= 0 + sRenderer.texture?.Width * 0.5f)
        {
            // Reverse the X component of velocity to simulate reflection
            xDirection = -xDirection;

            // Move the object back inside the boundary
            transform.Position = new Vector2(MathHelper.Clamp(
                transform.Position.X,
                0 + sRenderer.texture.Width * 0.5f,
                game.Graphics.PreferredBackBufferWidth - sRenderer.texture.Width * 0.5f),transform.Position.Y);
        }

        if (transform.Position.Y >= game?.Graphics.PreferredBackBufferHeight - sRenderer.texture?.Height * 0.5f ||
            transform.Position.Y <= 0 + sRenderer.texture?.Height * 0.5f)
        {
            // Reverse the Y component of velocity to simulate reflection
            yDirection = -yDirection;

            // Move the object back inside the boundary
            transform.Position = new Vector2(transform.Position.X, MathHelper.Clamp(
                transform.Position.Y,
                0 + sRenderer.texture.Height * 0.5f,
                game.Graphics.PreferredBackBufferHeight - sRenderer.texture.Height * 0.5f));
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        if (isFirst)
        {
            isFirst = false;
            SerializationManager.AddType(typeof(Ball));
        }

    }

    protected override void OnDestroy()
    {
        gameObject?.RemoveBehaviour(sRenderer!)?.Destroy();
        base.OnDestroy();
    }

    public float GetDeltaTime(GameTime gameTime)
    {
        double seconds, milliseconds;
        seconds = gameTime.ElapsedGameTime.TotalSeconds;
        milliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;
        return (float)seconds + ((float)milliseconds / 1000);
    }
}
