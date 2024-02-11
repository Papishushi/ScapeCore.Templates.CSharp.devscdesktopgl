using ScapeCore.Core.SceneManagement;
using ScapeCore.Core.Serialization;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

using var game = new ScapeCore.Core.Targets.LLAM(typeof(ScapeCore.Core.Engine.ResourceManager), typeof(SerializationManager), typeof(SceneManager));

using var scene = new Scene("My Scene", 0);
var i = SceneManager.AddScene(scene);
if (i == -1)
    Log.Error("Failed to add scene.");
SceneManager.SetCurrentScene(0);

var tasks = new List<Task>();
var balls = new List<Ball?>();
for (int index = 0; index < 1; index++)
{
    var a = async () =>
    {
        balls = await scene.AddToSceneMultipleAsync<Ball>(10);
    };

    tasks.Add(Task.Run(a));
}

var t = Task.WhenAll(tasks);

game.Run();